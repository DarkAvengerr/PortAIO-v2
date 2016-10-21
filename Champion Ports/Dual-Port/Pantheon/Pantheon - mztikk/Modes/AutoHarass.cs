using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkPantheon.Modes
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikkPantheon.Config;

    internal static class AutoHarass
    {
        #region Methods

        internal static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < Config.GetSliderValue("harass.auto.mana") || Events.IsAutoAttacking)
            {
                return;
            }

            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Physical);
            if (target == null || target.IsInvulnerable)
            {
                return;
            }

            if (Config.IsChecked("harass.auto.q") && Spells.Q.IsReady() && target.IsValidTarget(Spells.Q.Range))
            {
                Spells.Q.Cast(target);
            }
        }

        #endregion
    }
}