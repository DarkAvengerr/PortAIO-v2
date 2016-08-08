using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanLucian
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Program
    {
        public static Lucian ChampionLucian;

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName.ToLowerInvariant() == "lucian")
            {
                ChampionLucian = new Lucian();
            }
        }
    }
}