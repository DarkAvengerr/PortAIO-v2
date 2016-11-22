using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace BaseUlt3
{
    class Program
    {
        public static BaseUlt BaseUlt;

        public static void Main()
        {
            Game_OnGameLoad(new EventArgs());
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            BaseUlt = new BaseUlt();
        }
    }
}
