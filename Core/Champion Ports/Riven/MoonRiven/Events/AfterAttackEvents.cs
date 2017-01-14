using EloBuddy; 
using LeagueSharp.Common; 
namespace MoonRiven
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using System.Linq;

    internal class AfterAttackEvents : Logic
    {
        internal static void Init()
        {
            Orbwalking.AfterAttack += AfterAttack;
        }

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit == null || !unit.IsMe || target == null)
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo(target);
                    break;
                case Orbwalking.OrbwalkingMode.Burst:
                    Burst(target);
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass(target);
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    if (MenuInit.LaneClearQT && Q.IsReady() &&
                        (target.Type == GameObjectType.obj_AI_Turret || target.Type == GameObjectType.obj_Turret ||
                         target.Type == GameObjectType.obj_Barracks ||
                         target.Type == GameObjectType.obj_BarracksDampener ||
                         target.Type == GameObjectType.obj_HQ) &&
                        !HeroManager.Enemies.Exists(x => x.DistanceToPlayer() <= 800))
                    {
                        Q.Cast(target.Position, true);
                    }
                    else if (target.Type == GameObjectType.obj_AI_Minion)
                    {
                        if (target.Team == GameObjectTeam.Neutral)
                        {
                            JungleClear(target);
                        }
                        else if (target.Team != GameObjectTeam.Neutral && target.Team != Me.Team)
                        {
                            LaneClear(target);
                        }
                    }
                    break;
            }
        }

        private static void Combo(AttackableUnit tar)
        {
            AIHeroClient target = null;

            if (myTarget.IsValidTarget())
            {
                target = myTarget;
            }
            else if (tar is AIHeroClient)
            {
                target = (AIHeroClient)tar;
            }

            if (target != null && target.IsValidTarget(400))
            {
                if (MenuInit.ComboItem && Riven.UseItem())
                {
                    return;
                }

                if (MenuInit.ComboR2 != 3 && R.IsReady() && isRActive && qStack == 2 && Q.IsReady() && Riven.R2Logic(target))
                {
                    return;
                }

                if (Q.IsReady() && target.IsValidTarget(400))
                {
                    Riven.CastQ(target);
                    return;
                }

                if (MenuInit.ComboW && W.IsReady() && target.IsValidTarget(W.Range) && !Riven.HaveShield(target))
                {
                    W.Cast(true);
                    return;
                }

                if (MenuInit.ComboE && !Q.IsReady() && !W.IsReady() && E.IsReady() && target.IsValidTarget(400))
                {
                    E.Cast(target.Position, true);
                    return;
                }

                if (MenuInit.ComboR && R.IsReady() && !isRActive)
                {
                    Riven.R1Logic(target);
                }
            }
        }

        private static void Burst(AttackableUnit tar)
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
            if (Riven.UseItem())
            {
                return;
            }

            if (R.IsReady() && isRActive)
            {
                var rPred = R.GetPrediction(target, true);

                if (rPred.Hitchance >= HitChance.VeryHigh && R.Cast(rPred.CastPosition, true))
                {
                    return;
                }
            }

            if (Q.IsReady() && Riven.CastQ(target))
            {
                return;
            }

            if (W.IsReady() && target.IsValidTarget(W.Range) && W.Cast(true))
            {
                return;
            }

            if (E.IsReady())
            {
                E.Cast(target.Position, true);
            }
        }

        private static void EQFlashBurst(AIHeroClient target)
        {
            if (Riven.UseItem())
            {
                return;
            }

            if (R.IsReady() && isRActive)
            {
                var rPred = R.GetPrediction(target, true);

                if (rPred.Hitchance >= HitChance.VeryHigh && R.Cast(rPred.CastPosition, true))
                {
                    return;
                }
            }

            if (Q.IsReady() && Riven.CastQ(target))
            {
                return;
            }

            if (W.IsReady() && target.IsValidTarget(W.Range) && W.Cast(true))
            {
                return;
            }

            if (E.IsReady())
            {
                E.Cast(target.Position, true);
            }
        }

        private static void Harass(AttackableUnit tar)
        {
            AIHeroClient target = null;

            if (myTarget.IsValidTarget())
            {
                target = myTarget;
            }
            else if (tar is AIHeroClient)
            {
                target = (AIHeroClient) tar;
            }

            if (target != null && target.IsValidTarget())
            {
                if (MenuInit.HarassQ && Q.IsReady())
                {
                    if (MenuInit.HarassMode == 0)
                    {
                        if (qStack == 1)
                        {
                            Riven.CastQ(target);
                        }
                    }
                    else
                    {
                        Riven.CastQ(target);
                    }
                }
            }
        }

        private static void LaneClear(AttackableUnit tar)
        {
            var target = tar as Obj_AI_Minion;

            if (target != null && target.IsValidTarget())
            {
                if (MenuInit.LaneClearItem && MinionManager.GetMinions(Me.Position, 400).Count >= 2 && Riven.UseItem())
                {
                    return;
                }

                if (MenuInit.LaneClearQ && Q.IsReady())
                {
                    var minions = MinionManager.GetMinions(target.Position, 400);

                    if (minions.Count >= 2)
                    {
                        var qFarm =
                            MinionManager.GetBestCircularFarmLocation(minions.Select(x => x.Position.To2D()).ToList(),
                                Q.Width, Q.Range);

                        if (qFarm.MinionsHit >= 2)
                        {
                            Q.Cast(qFarm.Position, true);
                        }
                    }
                }
            }
        }

        private static void JungleClear(AttackableUnit tar)
        {
            var target = tar as Obj_AI_Minion;

            if (target != null && target.IsValidTarget(400) && target.Health > Me.GetAutoAttackDamage(target, true) &&
                !target.Name.Contains("Plant"))
            {
                if (MenuInit.JungleClearItem && Riven.UseItem())
                {
                    return;
                }

                if (MenuInit.JungleClearQ && Q.IsReady() && target.IsValidTarget(400) && Riven.CastQ(target))
                {
                    return;
                }

                if (MenuInit.JungleClearW && W.IsReady() && target.IsValidTarget(W.Range) && W.Cast(true))
                {
                    return;
                }

                if (MenuInit.JungleClearE && E.IsReady() && target.IsValidTarget(400))
                {
                    E.Cast(Game.CursorPos, true);
                }
            }
        }
    }
}