using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace AhriSharp
{
    class Program
    {
        public static Helper Helper;

        public static void Game_OnGameLoad()
        {
            Helper = new Helper();
            new Ahri();
        }
    }
}
