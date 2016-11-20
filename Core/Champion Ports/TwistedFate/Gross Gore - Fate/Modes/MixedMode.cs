using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate.Modes
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = GrossGoreTwistedFate.Config;

    internal static class MixedMode
    {
        #region Methods

        internal static void Execute()
        {
            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);

            if (!Config.IsChecked("wHarass") || target == null || !target.IsValidTarget(Spells.Q.Range))
            {
                return;
            }

            if (target.Distance(ObjectManager.Player) <= ObjectManager.Player.AttackRange + Config.GetSliderValue("wHRange")
                && Spells.W.IsReady() && ObjectManager.Player.ManaPercent >= Config.GetSliderValue("wHMana"))
            {
                CardSelector.ShuffleCards();
            }

            if(target.Distance(ObjectManager.Player) <= ObjectManager.Player.AttackRange + 100)
            {
                CardSelector.LockCard();
            }
        }

        #endregion
    }
}