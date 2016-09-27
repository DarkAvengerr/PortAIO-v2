using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanOlaf
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

        static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName.ToLowerInvariant() == "olaf")
            {
                var Olafium = new Olaf();
            }
        }
    }
}