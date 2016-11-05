using System;
using hYasuo.Champions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hYasuo
{
    internal static class Program
    {
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName == "Yasuo")
            {
                // ReSharper disable once ObjectCreationAsStatement
                new hYasuo.Champions.Yasuo();
            }
            else
            {
                return;
            }
        }
    }
}
