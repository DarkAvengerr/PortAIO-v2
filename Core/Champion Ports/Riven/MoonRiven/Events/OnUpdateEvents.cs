using EloBuddy; 
using LeagueSharp.Common; 
namespace MoonRiven
{
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    internal class OnUpdateEvents : Logic
    {
        internal static void Init()
        {
            Game.OnUpdate += OnUpdate;

            Game.OnUpdate += Args =>
            {
                if (TargetSelector.GetSelectedTarget().IsValidTarget())
                {
                    myTarget = TargetSelector.GetSelectedTarget();
                }
                else if (Orbwalker.GetTarget().IsValidTarget() && Orbwalker.GetTarget() is AIHeroClient)
                {
                    myTarget = (AIHeroClient) Orbwalker.GetTarget();
                }
                else
                {
                    myTarget = null;
                }

                if (W.Level > 0)
                {
                    W.Range = Me.HasBuff("RivenFengShuiEngine") ? 330 : 260;
                }

                if (Me.IsDead)
                {
                    qStack = 0;
                    return;
                }

                if (MenuInit.KeepQ && Me.HasBuff("RivenTriCleave"))
                {
                    if (Me.GetBuff("RivenTriCleave").EndTime - Game.Time < 0.3)
                    {
                        Q.Cast(Game.CursorPos, true);
                    }
                }

                if (qStack != 0 && Utils.TickCount - lastQTime > 3800)
                {   
                    qStack = 0;
                }
            };
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Burst:
                    Burst();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    Flee();
                    break;
            }
        }

        private static void Combo()
        {
            var target = myTarget.IsValidTarget()
                ? myTarget
                : TargetSelector.GetTarget(800f, TargetSelector.DamageType.Physical);

            if (target != null && target.IsValidTarget(800f))
            {
                if (MenuInit.ComboDot && IgniteSlot != SpellSlot.Unknown && Ignite.IsReady() &&
                    target.IsValidTarget(Ignite.Range) &&
                    (target.Health < DamageCalculate.GetComboDamage(target) ||
                     target.Health < DamageCalculate.GetIgniteDmage(target)) &&
                    Ignite.Cast(target, true) == Spell.CastStates.SuccessfullyCasted)
                {
                    return;
                }

                if (MenuInit.ComboYoumuu && Items.HasItem(3142) && Items.CanUseItem(3142) &&
                    target.IsValidTarget(580) && Items.UseItem(3142))
                {
                    return;
                }

                if (MenuInit.ComboR && R.IsReady() && !isRActive &&
                    target.Health <= DamageCalculate.GetComboDamage(target) &&
                    target.IsValidTarget(600f) && Riven.R1Logic(target))
                {
                    return;
                }

                if (MenuInit.ComboR2 != 3 && R.IsReady() && isRActive && Riven.R2Logic(target))
                {
                    return;
                }

                if (MenuInit.ComboQGap && Q.IsReady() && Utils.TickCount - lastQTime > 3600 && !Me.IsDashing() &&
                    target.IsValidTarget(480) && target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me))
                {
                    var pred = Prediction.GetPrediction(target, Q.Delay);

                    if (pred.UnitPosition != Vector3.Zero &&
                        (pred.UnitPosition.DistanceToPlayer() < target.DistanceToPlayer() ||
                         pred.UnitPosition.Distance(target.Position) <= target.DistanceToPlayer()) &&
                        Riven.CastQ(target))
                    {
                        return;
                    }
                }

                if (MenuInit.ComboEGap && E.IsReady() && target.IsValidTarget(600) &&
                    target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) + 50)
                {
                    E.Cast(target.Position, true);
                }

                if (MenuInit.ComboWLogic && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    if (!Q.IsReady() && qStack == 0 && W.Cast(true))
                    {
                        return;

                    }

                    if (!target.IsFacing(Me) && W.Cast(true))
                    {
                        return;
                    }

                    if (Q.IsReady() && qStack > 0 && W.Cast(true))
                    {
                        return;
                    }

                    if (Me.HasBuff("RivenFeint"))
                    {
                        W.Cast(true);
                    }
                }
            }
        }

        private static void Burst()
        {
            var target = TargetSelector.GetSelectedTarget();

            if (target != null && target.IsValidTarget())
            {
                if (MenuInit.BurstMode == 0)
                {
                    ShyBurst(target);
                }
                else
                {
                    EQFlashBurst(target);
                }
            }
        }

        private static void ShyBurst(AIHeroClient target)
        {
            if (MenuInit.BurstDot && IgniteSlot != SpellSlot.Unknown && Ignite.IsReady() &&
                Ignite.Cast(target, true) == Spell.CastStates.SuccessfullyCasted)
            {
                return;
            }

            if (target.IsValidTarget(W.Range) && W.Cast(true))
            {
                return;
            }

            if (target.IsValidTarget(E.Range + Me.AttackRange + Me.BoundingRadius))
            {
                if (E.IsReady() && R.IsReady() && W.IsReady() && !isRActive)
                {
                    E.Cast(target.Position, true);
                    LeagueSharp.Common.Utility.DelayAction.Add(10 + Game.Ping, () => R.Cast(true));
                    LeagueSharp.Common.Utility.DelayAction.Add(60 + Game.Ping, () => W.Cast(true));
                    return;
                }

                if (E.IsReady() && R.IsReady() && !isRActive)
                {
                    E.Cast(target.Position, true);
                    LeagueSharp.Common.Utility.DelayAction.Add(10 + Game.Ping, () => R.Cast(true));
                }
            }
            //else if (target.IsValidTarget(E.Range + Q.Range + Me.AttackRange + Me.BoundingRadius - 100))
            //{
            //    if (E.IsReady() && R.IsReady() && W.IsReady() && Q.IsReady() && !isRActive)
            //    {
            //        E.Cast(target.Position, true);
            //        LeagueSharp.Common.Utility.DelayAction.Add(10 + Game.Ping, () => R.Cast(true));
            //        LeagueSharp.Common.Utility.DelayAction.Add(60 + Game.Ping, () => Q.Cast(target.Position, true));
            //        LeagueSharp.Common.Utility.DelayAction.Add(65 + Game.Ping, () => W.Cast(true));
            //        return;
            //    }

            //    if (E.IsReady() && R.IsReady() && Q.IsReady() && !isRActive)
            //    {
            //        E.Cast(target.Position, true);
            //        LeagueSharp.Common.Utility.DelayAction.Add(10 + Game.Ping, () => R.Cast(true));
            //        LeagueSharp.Common.Utility.DelayAction.Add(60 + Game.Ping, () => Q.Cast(target.Position, true));
            //    }
            //}
            else if (MenuInit.BurstFlash && FlashSlot != SpellSlot.Unknown && Flash.IsReady())
            {
                if (target.IsValidTarget(E.Range + Me.AttackRange + Me.BoundingRadius + Flash.Range))
                {
                    if (E.IsReady() && R.IsReady() && W.IsReady() && !isRActive)
                    {
                        E.Cast(target.Position, true);
                        LeagueSharp.Common.Utility.DelayAction.Add(10 + Game.Ping, () => R.Cast(true));
                        LeagueSharp.Common.Utility.DelayAction.Add(60 + Game.Ping, () => W.Cast(true));
                        LeagueSharp.Common.Utility.DelayAction.Add(61 + Game.Ping, () => Flash.Cast(target.Position, true));
                        return;
                    }

                    if (E.IsReady() && R.IsReady() && !isRActive)
                    {
                        E.Cast(target.Position, true);
                        LeagueSharp.Common.Utility.DelayAction.Add(10 + Game.Ping, () => R.Cast(true));
                        LeagueSharp.Common.Utility.DelayAction.Add(60 + Game.Ping, () => Flash.Cast(target.Position, true));
                    }
                }
                //else if (target.IsValidTarget(E.Range + Me.AttackRange + Me.BoundingRadius + Flash.Range + Q.Range - 50))
                //{
                //    if (E.IsReady() && R.IsReady() && W.IsReady() && Q.IsReady() && !isRActive)
                //    {
                //        E.Cast(target.Position, true);
                //        LeagueSharp.Common.Utility.DelayAction.Add(10 + Game.Ping, () => R.Cast(true));
                //        LeagueSharp.Common.Utility.DelayAction.Add(60 + Game.Ping, () => Q.Cast(target.Position, true));
                //        LeagueSharp.Common.Utility.DelayAction.Add(80 + Game.Ping, () => W.Cast(true));
                //        LeagueSharp.Common.Utility.DelayAction.Add(81 + Game.Ping, () => Flash.Cast(target.Position, true));
                //        return;
                //    }

                //    if (E.IsReady() && R.IsReady() && Q.IsReady() && !isRActive)
                //    {
                //        E.Cast(target.Position, true);
                //        LeagueSharp.Common.Utility.DelayAction.Add(10 + Game.Ping, () => R.Cast(true));
                //        LeagueSharp.Common.Utility.DelayAction.Add(60 + Game.Ping, () => Q.Cast(target.Position, true));
                //        LeagueSharp.Common.Utility.DelayAction.Add(80 + Game.Ping, () => Flash.Cast(target.Position, true));
                //    }
                //}
            }
        }

        private static void EQFlashBurst(AIHeroClient target)
        {
            if (MenuInit.BurstDot && IgniteSlot != SpellSlot.Unknown && Ignite.IsReady() &&
                Ignite.Cast(target, true) == Spell.CastStates.SuccessfullyCasted)
            {
                return;
            }

            if (target.IsValidTarget(W.Range) && W.Cast(true))
            {
                return;
            }

            if (MenuInit.BurstFlash && FlashSlot != SpellSlot.Unknown && Flash.IsReady())
            {
                if (qStack < 2 && Utils.TickCount - lastQTime >= 850 && Q.Cast(Game.CursorPos, true))
                {
                    return;
                }

                if (qStack == 2 && E.IsReady() && R.IsReady() && !isRActive && W.IsReady() &&
                    target.IsValidTarget(E.Range + Flash.Range + Q.Range - 100))
                {
                    E.Cast(target.Position, true);
                    LeagueSharp.Common.Utility.DelayAction.Add(10 + Game.Ping, () => R.Cast(true));
                    LeagueSharp.Common.Utility.DelayAction.Add(50 + Game.Ping, () => Flash.Cast(target.Position, true));
                    LeagueSharp.Common.Utility.DelayAction.Add(61 + Game.Ping, () => Riven.UseItem());
                    LeagueSharp.Common.Utility.DelayAction.Add(62 + Game.Ping, () => Q.Cast(target.Position, true));
                    LeagueSharp.Common.Utility.DelayAction.Add(70 + Game.Ping, () => W.Cast(true));
                    LeagueSharp.Common.Utility.DelayAction.Add(71 + Game.Ping, () => R.Cast(target.Position, true));
                }
            }
            else if (target.IsValidTarget(E.Range + Q.Range + Q.Range +Q.Range))
            {
                if (qStack < 2 && Utils.TickCount - lastQTime >= 850 && Q.Cast(Game.CursorPos, true))
                {
                    return;
                }

                if (qStack == 2 && E.IsReady() && R.IsReady() && !isRActive && W.IsReady() &&
                    target.IsValidTarget(E.Range + Q.Range - 100))
                {
                    E.Cast(target.Position, true);
                    LeagueSharp.Common.Utility.DelayAction.Add(10 + Game.Ping, () => R.Cast(true));
                    LeagueSharp.Common.Utility.DelayAction.Add(50 + Game.Ping, () => Q.Cast(target.Position, true));
                    LeagueSharp.Common.Utility.DelayAction.Add(61 + Game.Ping, () => Riven.UseItem());
                    LeagueSharp.Common.Utility.DelayAction.Add(62 + Game.Ping, () => W.Cast(true));
                    LeagueSharp.Common.Utility.DelayAction.Add(70 + Game.Ping, () => R.Cast(target.Position, true));
                }
            }
        }

        private static void Harass()
        {
            var target = myTarget ??
                TargetSelector.GetTarget(E.Range + Me.BoundingRadius, TargetSelector.DamageType.Physical);

            if (target.IsValidTarget())
            {
                if (MenuInit.HarassMode == 0)
                {
                    if (E.IsReady() && MenuInit.HarassE && qStack == 2)
                    {
                        var pos = Me.Position + (Me.Position - target.Position).Normalized() * E.Range;

                        E.Cast(Me.Position.Extend(pos, E.Range), true);
                    }

                    if (Q.IsReady() && MenuInit.HarassQ && qStack == 2)
                    {
                        var pos = Me.Position + (Me.Position - target.Position).Normalized() * E.Range;

                        LeagueSharp.Common.Utility.DelayAction.Add(100, () => Q.Cast(Me.Position.Extend(pos, Q.Range), true));
                    }

                    if (W.IsReady() && MenuInit.HarassW && target.IsValidTarget(W.Range) && qStack == 1)
                    {
                        W.Cast(true);
                    }

                    if (Q.IsReady() && MenuInit.HarassQ)
                    {
                        if (qStack == 0)
                        {
                            Riven.CastQ(target);
                            Orbwalker.ForceTarget(target);
                        }

                        if (qStack == 1 && Utils.TickCount - lastQTime > 600)
                        {
                            Riven.CastQ(target);
                            Orbwalker.ForceTarget(target);
                        }
                    }
                }
                else
                {
                    if (E.IsReady() && MenuInit.HarassE &&
                        target.DistanceToPlayer() <= E.Range + (Q.IsReady() ? Q.Range : Me.AttackRange))
                    {
                        E.Cast(target.Position, true);
                    }

                    if (Q.IsReady() && MenuInit.HarassQ && target.IsValidTarget(Q.Range) && qStack == 0 &&
                        Utils.TickCount - lastQTime > 500)
                    {
                        Riven.CastQ(target);
                        Orbwalker.ForceTarget(target);
                    }

                    if (W.IsReady() && MenuInit.HarassW && target.IsValidTarget(W.Range) && (!Q.IsReady() || qStack == 1))
                    {
                        W.Cast(true);
                    }
                }
            }
        }

        private static void LaneClear()
        {
            if (MenuInit.LaneClearQ && MenuInit.LaneClearQSmart && Q.IsReady())
            {
                var minions = MinionManager.GetMinions(Me.Position, 400);

                if (minions.Count >= 2)
                {
                    var qFarm =
                        MinionManager.GetBestCircularFarmLocation(minions.Select(x => x.Position.To2D()).ToList(),
                            Q.Width, Q.Range);

                    if (qFarm.MinionsHit >= 2)
                    {
                        if (Utils.TickCount - lastQTime > 1200)
                        {
                            Q.Cast(qFarm.Position, true);
                            return;
                        }
                    }
                }
            }

            if (MenuInit.LaneClearW && W.IsReady())
            {
                var minions = MinionManager.GetMinions(Me.Position, W.Range);

                if (minions.Count > 0 && minions.Count >= MenuInit.LaneClearWCount)
                {
                    W.Cast(true);
                }
            }
        }

        private static void JungleClear()
        {
            if (MenuInit.JungleClearW && W.IsReady() && qStack > 0)
            {
                var mobs = MinionManager.GetMinions(Me.Position, W.Range, MinionTypes.All, MinionTeam.Neutral);

                if (mobs.Any() && W.Cast(true))
                {
                    return;
                }
            }

            if (MenuInit.JungleClearE && E.IsReady())
            {
                var mobs = MinionManager.GetMinions(Me.Position, (float)(Orbwalking.GetRealAutoAttackRange(Me) * 2.5),
                    MinionTypes.All, MinionTeam.Neutral);

                if (mobs.Any() && ((!Q.IsReady() && !W.IsReady()) || !mobs.Any(x => x.DistanceToPlayer() <= 250)))
                {
                    E.Cast(Game.CursorPos, true);
                }
            }
        }

        private static void Flee()
        {
            if (MenuInit.FleeW && W.IsReady() && HeroManager.Enemies.Any(
                x => x.DistanceToPlayer() <= W.Range && !Riven.HaveShield(x)) && W.Cast(true))
            {
                return;
            }

            if (MenuInit.FleeE && E.IsReady() &&
                ((!Q.IsReady() && qStack == 0) || qStack == 2) &&
                E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true))
            {
                return;
            }

            if (MenuInit.FleeQ && Q.IsReady() && !Me.IsDashing())
            {
                Q.Cast(Me.Position.Extend(Game.CursorPos, 350f), true);
            }
        }
    }
}