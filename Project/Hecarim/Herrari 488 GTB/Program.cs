using System;

using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Herrari_488_GTB
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Initializer.Initialize();
        }
    }
}
