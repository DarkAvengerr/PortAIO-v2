using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanAkali
{
    internal class Program
    {
        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName.ToLowerInvariant() == "akali")
            {
                var akali = new Akali();
            }
        }
    }
}