using System.Linq;
using Jhin___The_Virtuoso.Extensions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Jhin___The_Virtuoso.Modes
{
    static class Mixed
    {
        /// <summary>
        /// Execute Harass W
        /// </summary>
        private static void ExecuteW()
        {
            foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells.W.Range)))
            {
                var pred = Spells.W.GetPrediction(enemy);
                if (pred.Hitchance >= Menus.Config.HikiChance("w.hit.chance"))
                {
                    Spells.W.Cast(pred.CastPosition);
                }
            }
        }

        /// <summary>
        /// Execute Harass
        /// </summary>
        public static void ExecuteHarass()
        {
            if (ObjectManager.Player.ManaPercent < Menus.Config.Item("harass.mana").GetValue<Slider>().Value)
            {
                return;
            }

            if (Spells.W.LSIsReady() && Menus.Config.Item("w.harass").GetValue<bool>())
            {
                ExecuteW();
            }
        }
    }
}
