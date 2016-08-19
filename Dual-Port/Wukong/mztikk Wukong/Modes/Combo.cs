using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Wukong.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Wukong.Extensions;

    using Config = Wukong.Config;

    internal static class Combo
    {
        #region Public Methods and Operators

        public static void OnPostAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Mainframe.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo) return;
            var comboTarget = TargetSelector.GetTarget(Spells.E.Range - 100, TargetSelector.DamageType.Physical);
            if (!Spells.E.IsReady())
            {
                comboTarget = TargetSelector.GetTarget(175, TargetSelector.DamageType.Physical);
            }

            if (comboTarget == null || target.NetworkId != comboTarget.NetworkId || comboTarget.IsInvulnerable)
            {
                return;
            }

            if (Config.IsChecked("combo.q") && Spells.Q.IsReady())
            {
                Spells.Q.Cast();
            }
        }

        #endregion

        #region Methods

        internal static void Execute()
        {
            if (Config.IsChecked("combo.r") && Spells.R.IsReady()
                && !ObjectManager.Player.HasBuff("MonkeyKingSpinToWin"))
            {
                var validTargetsInUlt =
                    HeroManager.Enemies.Where(
                        x =>
                        !x.IsDead && x.IsValid && !x.IsInvulnerable && x.Distance(ObjectManager.Player) < Spells.R.Range);
                if (validTargetsInUlt.Count() >= Config.GetSliderValue("combo.r.targets"))
                {
                    Spells.R.Cast();
                }

                var forceTargets =
                    HeroManager.Enemies.Where(
                        x =>
                        !x.IsDead && x.IsValid && !x.IsInvulnerable && x.Distance(ObjectManager.Player) < Spells.R.Range
                        && Config.IsChecked("combo.r.force." + x.ChampionName)
                        && (!Config.IsChecked("combo.r.force.waitq") || x.HasBuff("monkeykingdoubleattackdebuff")));
                if (forceTargets.Any())
                {
                    Spells.R.Cast();
                }
            }

            var target = TargetSelector.GetTarget(Spells.E.Range, TargetSelector.DamageType.Physical);
            var wantedTarget = TargetSelector.GetTarget(Spells.E.Range + 150, TargetSelector.DamageType.Physical);

            if (target == null || target.IsInvulnerable)
            {
                return;
            }

            if (Config.IsChecked("combo.q") && Spells.Q.IsReady() && !Orbwalking.InAutoAttackRange(target)
                && target.Distance(ObjectManager.Player) < Spells.Q.Range && target.Health < Spells.GetQDamage(target))
            {
                Spells.Q.Cast();
            }

            if (!Config.IsChecked("combo.e") || !Spells.E.CanCast(target))
            {
                return;
            }

            if (Config.IsChecked("combo.e.tower") && target.Position.UnderEnemyTurret())
            {
                return;
            }

            if (Config.GetStringListValue("combo.e.mode") == 0)
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

            if (Config.GetStringListValue("combo.e.mode") == 1)
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