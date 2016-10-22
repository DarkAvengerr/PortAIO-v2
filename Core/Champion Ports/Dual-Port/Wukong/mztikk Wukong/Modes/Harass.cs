using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Wukong.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Wukong.Extensions;

    using Config = Wukong.Config;

    internal static class Harass
    {
        #region Public Methods and Operators

        public static void OnPostAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Mainframe.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed
                || ObjectManager.Player.ManaPercent < Config.GetSliderValue("harass.mana")) return;
            var comboTarget = TargetSelector.GetTarget(Spells.E.Range - 100, TargetSelector.DamageType.Physical);
            if (!Spells.E.IsReady())
            {
                comboTarget = TargetSelector.GetTarget(175, TargetSelector.DamageType.Physical);
            }

            if (comboTarget == null || target.NetworkId != comboTarget.NetworkId || comboTarget.IsInvulnerable)
            {
                return;
            }

            if (Config.IsChecked("harass.q") && Spells.Q.IsReady())
            {
                Spells.Q.Cast();
            }
        }

        #endregion

        #region Methods

        internal static void AutoQ()
        {
            if (ObjectManager.Player.ManaPercent < Config.GetSliderValue("harass.mana") || !Spells.Q.IsReady()
                || ObjectManager.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            var target = TargetSelector.GetTarget(Spells.Q.Range - 15, TargetSelector.DamageType.Physical);
            if (target == null || target.IsInvulnerable)
            {
                return;
            }

            Spells.Q.Cast();
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, target);
        }

        internal static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < Config.GetSliderValue("harass.mana"))
            {
                return;
            }

            var target = TargetSelector.GetTarget(Spells.E.Range, TargetSelector.DamageType.Physical);
            var wantedTarget = TargetSelector.GetTarget(Spells.E.Range + 150, TargetSelector.DamageType.Physical);

            if (target == null || target.IsInvulnerable)
            {
                return;
            }

            if (Config.IsChecked("harass.q") && Spells.Q.IsReady() && !Orbwalking.InAutoAttackRange(target)
                && target.Distance(ObjectManager.Player) < Spells.Q.Range && target.Health < Spells.GetQDamage(target))
            {
                Spells.Q.Cast();
            }

            if (!Config.IsChecked("harass.e") || !Spells.E.CanCast(target))
            {
                return;
            }

            if (Config.IsChecked("harass.e.tower") && target.Position.UnderEnemyTurret())
            {
                return;
            }

            if (Config.GetStringListValue("harass.e.mode") == 0)
            {
                if (target != wantedTarget)
                {
                    return;
                }

                if (!target.Path.Any())
                {
                    return;
                }

                var path = target.Path[0];
                var distToTarget = ObjectManager.Player.Distance(target);
                var distToTargetPath = ObjectManager.Player.Distance(path);
                if (distToTargetPath < distToTarget)
                {
                    return;
                }

                if (target.MoveSpeed >= ObjectManager.Player.MoveSpeed
                    && distToTarget > Orbwalking.GetRealAutoAttackRange(ObjectManager.Player))
                {
                    Spells.E.Cast(target);
                }

                var modifier = target.MoveSpeed < ObjectManager.Player.MoveSpeed ? 0.75 : 0.55;

                if (distToTarget > Spells.E.Range * modifier && target.IsMoving)
                {
                    Spells.E.Cast(target);
                }
            }

            if (Config.GetStringListValue("harass.e.mode") == 1)
            {
                if (!Orbwalking.InAutoAttackRange(target))
                {
                    Spells.E.Cast(target);
                }
            }
        }

        #endregion
    }
}