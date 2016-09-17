using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkXinZhao.Modes
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using mztikkXinZhao.Extensions;

    using Config = mztikkXinZhao.Config;

    internal static class Harass
    {
        #region Public Methods and Operators

        public static void HarassOnPostAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Mainframe.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed
                || ObjectManager.Player.ManaPercent < Config.GetSliderValue("harassMana"))
            {
                return;
            }

            var comboTarget = TargetSelector.GetTarget(Spells.E.Range, TargetSelector.DamageType.Physical);
            if (comboTarget == null || target.NetworkId != comboTarget.NetworkId || comboTarget.IsInvulnerable)
            {
                return;
            }

            if (Spells.W.CanCast() && Config.IsChecked("useWharass"))
            {
                Spells.W.Cast();
            }

            if (Spells.Q.CanCast() && Config.IsChecked("useQharass"))
            {
                Spells.Q.Cast();
            }
        }

        #endregion

        #region Methods

        internal static void Execute()
        {
            var target = TargetSelector.GetTarget(Spells.E.Range, TargetSelector.DamageType.Physical);
            if (target == null || target.IsInvulnerable)
            {
                return;
            }

            if (Config.IsChecked("harassETower") && target.Position.UnderEnemyTurret())
            {
                return;
            }

            if (target.Distance(ObjectManager.Player.Position) >= 255
                && ObjectManager.Player.HasBuff("XenZhaoComboTarget") && Spells.E.CanCast(target)
                && Config.IsChecked("useEharass"))
            {
                Spells.E.Cast(target);
            }

            if ((target.Distance(ObjectManager.Player.Position) > Spells.E.Range * 0.55f
                 && target.MoveSpeed + 5 >= ObjectManager.Player.MoveSpeed)
                || (target.Distance(ObjectManager.Player.Position) > Spells.E.Range * 0.70f))
            {
                if (!Spells.E.CanCast(target))
                {
                    /*
                    target = TargetSelector.GetTarget(175, DamageType.Physical);
                    */
                }
                else
                {
                    if (Config.IsChecked("useEharass"))
                    {
                        Spells.E.Cast(target);
                    }
                }
            }
        }

        #endregion
    }
}