using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Tristana
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Initialiser
    {

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName == "Tristana")
            {
                new SkyLv_Tristana();
            }
        }

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
    }
}