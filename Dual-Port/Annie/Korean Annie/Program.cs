using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanAnnie
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    class Program
    {
        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName.ToLowerInvariant() == "annie")
            {
                Annie annie = new Annie();
            }
        }
    }
}
