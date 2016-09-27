using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Tristana
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Initialiser
    {

        private static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName == "Tristana")
            {
                new SkyLv_Tristana();
            }
        }

        public static void Main()
        {
            Game_OnGameLoad();
        }
    }
}