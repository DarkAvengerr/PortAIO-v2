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
            Game_OnGameLoad(new EventArgs());
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Yasuo")
            {
                return;
            }

            Logic.LoadYasuo();
        }
    }
}