namespace SkyLv_Taric
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class Initialiser
    {
        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName == "Taric")
            {
                new SkyLv_Taric();
            }
        }
    }
}