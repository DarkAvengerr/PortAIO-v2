using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Yasuo
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Program
    {
        public static void Main()
        {
            Game_OnGameLoad();
        }

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Yasuo")
            {
                return;
            }

            Logic.LoadYasuo();
        }
    }
}