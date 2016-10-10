using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkSona.Modes
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using mztikkSona.Extensions;

    using Config = mztikkSona.Config;

    internal static class Harass
    {
        #region Public Methods and Operators

        public static void OnPreAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed
                && args.Target.Type == GameObjectType.obj_AI_Minion && Config.IsChecked("Harass.aaMins")
                && ObjectManager.Player.CountAlliesInRange(1500) > 0)
            {
                args.Process = false;
            }
        }

        #endregion

        #region Methods

        internal static void Execute()
        {
            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (Config.IsChecked("Harass.bQ") && Spells.Q.CanCast() && target.Distance(ObjectManager.Player) < Spells.Q.Range)
            {
                Spells.Q.Cast();
            }
        }

        #endregion
    }
}