using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkSona
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class Program
    {
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName.ToLower() != "sona")
            {
                return;
            }

            Spells.LoadSpells();
            Config.BuildMenu();
            Mainframe.Init();
        }
    }
}
