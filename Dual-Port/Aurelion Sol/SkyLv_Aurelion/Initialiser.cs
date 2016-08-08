using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_AurelionSol
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Initialiser
    {
        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName == "AurelionSol")
            {
                new SkyLv_AurelionSol();
            }
        }
    }
}