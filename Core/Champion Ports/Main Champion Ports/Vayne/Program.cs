using System;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Utility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace VayneHunter_Reborn
{
    class Program
    {
        private static string ChampionName = "Vayne";
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != ChampionName)
            {
                return;
            }
            VHRBootstrap.OnLoad();

            Chat.Print("<font color='#FF0000'><b>[VHR - Rewrite!]</b></font> By Asuna Loaded!");
            Chat.Print("Also try <font color='#66FF33'><b>DZAwareness</b></font> for a gamebreaking experience!");
        }
    }
}
