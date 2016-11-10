using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KingLazerViktor
{
    class Program
    {
        public static void Main()
        {
            Game_OnGameLoad();
        }

        private static void Game_OnGameLoad()
        {
            switch (ObjectManager.Player.ChampionName)
            {
                case "Viktor":
                    new Viktor.Champions.Viktor();
                    Chat.Print("<font color='#FFFF33'>[2HAM Viktor] Successfully Loaded</font>");
                    break;
                default:
                    Chat.Print("[2HAM Viktor] Your Champion is not Viktor");
                    break;
            }
        }
    }
}
