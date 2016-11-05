using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Evelynn
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Initialiser
    {
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName == "Evelynn")
            {
                new SkyLv_Evelynn();
            }
        }
    }
}