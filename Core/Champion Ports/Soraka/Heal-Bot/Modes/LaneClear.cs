using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Soraka_HealBot.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = Soraka_HealBot.Config;

    internal static class LaneClear
    {
        #region Methods

        internal static void Execute()
        {
            if (!Spells.Q.IsReady() || !Config.IsChecked("useQInLC")
                || ObjectManager.Player.ManaPercent < Config.GetSliderValue("manaLaneClear"))
            {
                return;
            }

            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range);
            if (!minions.Any() || minions.Count < 1)
            {
                return;
            }

            var farmLoc = MinionManager.GetBestCircularFarmLocation(
                minions.Select(x => x.Position.To2D()).ToList(), 
                Spells.Q.Width, 
                Spells.Q.Range);
            if (farmLoc.MinionsHit >= Config.GetSliderValue("qTargets"))
            {
                Spells.Q.Cast(farmLoc.Position);
            }
        }

        #endregion
    }
}