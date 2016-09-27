using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KarthusSharp
{
    class Program
    {
        public static Helper Helper;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            Helper = new Helper();
            new Karthus();
        }
    }
}
