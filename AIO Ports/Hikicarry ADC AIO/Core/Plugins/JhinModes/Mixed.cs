using System.Linq;
using HikiCarry.Champions;
using HikiCarry.Core.Predictions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry.Core.Plugins.JhinModes
{
    static class Mixed
    {
        /// <summary>
        /// Execute Harass W
        /// </summary>
        private static void ExecuteW()
        {
            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Jhin.W.Range)))
            {
                Jhin.W.Do(enemy, Utilities.Utilities.HikiChance("hitchance"));
            }
        }

        /// <summary>
        /// Execute Harass
        /// </summary>
        public static void ExecuteHarass()
        {
            if (ObjectManager.Player.ManaPercent < Initializer.Config.Item("harass.mana",true).GetValue<Slider>().Value)
            {
                return;
            }

            if (Jhin.W.IsReady() && Initializer.Config.Item("w.harass",true).GetValue<bool>())
            {
                ExecuteW();
            }
        }
    }
}
