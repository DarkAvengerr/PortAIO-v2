using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Fiora.Manager.Events
{
    using Spells;
    using System;
    using System.Linq;
    using Passive;
    using LeagueSharp;
    using LeagueSharp.Common;
    using static Common.Common;
    using SharpDX;

    internal class UpdateManager : Logic
    {
        internal static void Init(EventArgs Args)
        {
            Orbwalker.SetOrbwalkingPoint(Vector3.Zero);

            if (Me.IsDead)
            {
                return;
            }

            KillSteal();

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (Menu.Item("FleeKey", true).GetValue<KeyBind>().Active)
                    {
                        Flee();
                    }
                    break;
            }
        }

        private static void KillSteal()
        {
            var KillStealQ =
                HeroManager.Enemies.FirstOrDefault(
                    x =>
                        x.IsValidTarget(Q.Range) && CheckTargetSureCanKill(x) &&
                        x.Health < Q.GetDamage(x) + SpellManager.GetPassiveDamage(x, 1));

            if (Menu.Item("KillStealQ", true).GetValue<bool>() && Q.IsReady() && KillStealQ != null &&
                W.CanCast(KillStealQ))
            {
                CastQ(KillStealQ);
            }

            var KillStealW =
                HeroManager.Enemies.FirstOrDefault(
                    x =>
                        x.IsValidTarget(W.Range) &&
                        x.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) + 100 && CheckTargetSureCanKill(x) &&
                        x.Health < W.GetDamage(x));

            if (Menu.Item("KillStealW", true).GetValue<bool>() && W.IsReady() && KillStealW != null &&
                W.CanCast(KillStealW))
            {
                W.Cast(KillStealW.Position);
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                         TargetSelector.GetTarget(800f, TargetSelector.DamageType.Physical);

            if (CheckTarget(target))
            {
                if (target.Health*1.2 < ComboDamage(target) && Menu.Item("ComboYoumuu", true).GetValue<bool>() &&
                    Items.HasItem(3142) && Items.CanUseItem(3142) &&
                    target.DistanceToPlayer() > 400)
                {
                    Items.UseItem(3142);
                }

                if (target.Health < ComboDamage(target) && Menu.Item("ComboIgnite", true).GetValue<bool>() &&
                    Ignite != SpellSlot.Unknown && Ignite.IsReady())
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                }

                ItemsUse(false, Menu.Item("ComboTiamat", true).GetValue<bool>(),
                         Menu.Item("ComboHydra", true).GetValue<bool>());


                if (Menu.Item("ComboPassive", true).GetValue<bool>())
                {
                    ForcusAttack(target);
                }

                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady())
                {
                    CastQ(target);
                }

                if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    if (Menu.Item("ComboRSolo", true).GetValue<bool>())
                    {
                        if (Me.CountEnemiesInRange(1000) <= 2)
                        {
                            foreach (
                                var x in
                                HeroManager.Enemies.Where(
                                    x =>
                                        x.IsValidTarget(R.Range) &&
                                        x.Health <= ComboDamage(x) + SpellManager.GetPassiveDamage(x, 4)))
                            {
                                R.CastOnUnit(x);
                            }
                        }
                    }

                    if (Menu.Item("ComboRTeam", true).GetValue<bool>())
                    {
                        if (Me.CountEnemiesInRange(1000) > 2 && Me.CountAlliesInRange(1000) > 1)
                        {
                            foreach (
                                var x in
                                HeroManager.Enemies.Where(
                                    x =>
                                        x.IsValidTarget(R.Range) &&
                                        x.Health <=
                                        ComboDamage(x) + SpellManager.GetPassiveDamage(x, 4) +
                                        Me.GetAutoAttackDamage(x)*3))
                            {
                                R.CastOnUnit(x);
                            }
                        }
                    }
                }
            }
        }

        private static void ForcusAttack(AIHeroClient target)
        {
            if (Me.Spellbook.IsAutoAttacking ||
                (target.DistanceToPlayer() <= Orbwalking.GetRealAutoAttackRange(Me) && Orbwalking.CanAttack()))
            {
                return;
            }

            if (Q.IsReady())
            {
                return;
            }

            var pos = PassiveManager.OrbwalkerPosition(target);
            var path = Me.GetPath(pos);

            Orbwalker.SetOrbwalkingPoint(target.IsMoving
                ? (path.Length < 3 ? pos : path.Skip(path.Length/2).FirstOrDefault()): pos);
        }

        private static void Harass()
        {
            if (Me.ManaPercent >= Menu.Item("HarassMana", true).GetValue<Slider>().Value)
            {
                var target = TargetSelector.GetTarget(800f, TargetSelector.DamageType.Physical);

                if (CheckTarget(target))
                {
                    ItemsUse(false, Menu.Item("HarassTiamat", true).GetValue<bool>(),
                        Menu.Item("HarassHydra", true).GetValue<bool>());

                    if (Menu.Item("HarassPassive", true).GetValue<bool>())
                    {
                        ForcusAttack(target);
                    }

                    if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady())
                    {
                        CastQ(target);
                    }
                }
            }
        }

        private static void LaneClear()
        {
            if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
            {
                var minions = MinionManager.GetMinions(Me.Position, Q.Range);

                if (minions.Any())
                {
                    if (Menu.Item("LaneClearQ", true).GetValue<bool>() && Q.IsReady())
                    {
                        var firstKill = minions.FirstOrDefault(x => x.Health < Q.GetDamage(x));

                        if (firstKill != null)
                        {
                            Q.Cast(firstKill);
                        }
                        else
                        {
                            var qMin = minions.MinOrDefault(x => x.Health);

                            Q.Cast(qMin);
                        }
                    }

                    if (Menu.Item("LaneClearE", true).GetValue<bool>() && E.IsReady() && Orbwalking.CanAttack())
                    {
                        var eMin =
                            minions.Where(x => Orbwalker.InAutoAttackRange(x))
                                .FirstOrDefault(x => x.Health < E.GetDamage(x));

                        if (eMin != null)
                        {
                            E.Cast();
                        }
                    }
                }
            }
        }

        private static void JungleClear()
        {
            if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
            {
                var minions = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (minions.Any())
                {
                    if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady())
                    {
                        var qMin = minions.MinOrDefault(x => x.Health);

                        Q.Cast(qMin);
                    }
                }
            }
        }

        private static void Flee()
        {
            Orbwalking.MoveTo(Game.CursorPos);

            if (Menu.Item("FleeQ", true).GetValue<bool>() && Q.IsReady())
            {
                var pos = Me.Position.Extend(Game.CursorPos, Q.Range);
                Q.Cast(pos);
            }
        }

        internal static bool CastQ(AIHeroClient target)
        {
            if (!Q.IsReady() || !target.IsValidTarget(Q.Range))
            {
                return false;
            }

            if (Q.IsReady())
            {
                if (PassiveManager.PassiveCount(target) > 0)
                {
                    var pos = PassiveManager.CastQPosition(target);

                    if (Menu.Item("QUnder", true).GetValue<bool>() && pos.UnderTurret(true))
                    {
                        return false;
                    }

                    if (Me.Distance(pos) > Q.Range)
                    {
                        return false;
                    }

                    if (Me.Distance(pos) < 80)
                    {
                        return false;
                    }

                    return Q.Cast(pos);
                }
                else
                {
                    var pos = target.ServerPosition;

                    if (Menu.Item("QUnder", true).GetValue<bool>() && pos.UnderTurret(true))
                    {
                        return false;
                    }

                    if (target.IsValidTarget(Q.Range) && Me.Distance(target) >= 100)
                    {
                        Q.Cast(target.ServerPosition);
                    }
                }
            }

            return false;
        }
    }
}