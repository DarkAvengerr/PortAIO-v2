using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkXinZhao.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using mztikkXinZhao.Extensions;

    using Config = mztikkXinZhao.Config;

    internal static class Combo
    {
        #region Public Methods and Operators

        public static void ComboOnPostAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var comboTarget = TargetSelector.GetTarget(Spells.E.Range - 100, TargetSelector.DamageType.Physical);
                if (!Spells.E.IsReady())
                {
                    comboTarget = TargetSelector.GetTarget(175, TargetSelector.DamageType.Physical);
                }

                if (comboTarget == null || target.NetworkId != comboTarget.NetworkId || comboTarget.IsInvulnerable)
                {
                    return;
                }

                if (Spells.W.IsReady() && Config.IsChecked("useWcombo"))
                {
                    Spells.W.Cast();
                }

                if (Spells.Q.IsReady() && Config.IsChecked("useQcombo"))
                {
                    Spells.Q.Cast();
                }
            }
        }

        #endregion

        #region Methods

        internal static void Execute()
        {
            var target = TargetSelector.GetTarget(Spells.E.Range, TargetSelector.DamageType.Physical);
            var wantedTarget = TargetSelector.GetTarget(Spells.E.Range + 150, TargetSelector.DamageType.Physical);
            if (target == null || target.IsInvulnerable)
            {
                return;
            }

            var enemiesAroundPlayer =
                HeroManager.Enemies.Where(en => en.Distance(ObjectManager.Player.Position) <= Spells.R.Range);
            if (enemiesAroundPlayer.Count() >= Config.GetSliderValue("comboMinR") && Spells.R.IsReady())
            {
                Spells.R.Cast();
            }

            if (Spells.W.IsReady() && target.IsValidTarget(175) && Config.IsChecked("useWcombo"))
            {
                Spells.W.Cast();
            }

            if (Config.IsChecked("combo.smite") && Spells.SmiteSlot != SpellSlot.Unknown && Spells.Smite.IsReady()
                && target.IsValidTarget(Spells.Smite.Range))
            {
                Spells.Smite.Cast(target);
            }

            if (!Config.IsChecked("useEcombo") || !Spells.E.CanCast(target))
            {
                return;
            }

            if (Config.IsChecked("comboETower") && target.Position.UnderEnemyTurret())
            {
                return;
            }

            if (Config.GetStringListValue("comboEmode") == 0)
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

                if (distToTarget > Spells.E.Range * 0.75 && target.IsMoving)
                {
                    Spells.E.Cast(target);
                }
            }

            if (Config.GetStringListValue("comboEmode") == 1)
            {
                if (target.Distance(ObjectManager.Player.Position) >= 225)
                {
                    Spells.E.Cast(target);
                }
            }
        }

        #endregion
    }
}