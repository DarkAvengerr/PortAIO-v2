using System;

using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Herrari_488_GTB
{
    class Program
    {
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            Initializer.Initialize();
        }
    }
}
