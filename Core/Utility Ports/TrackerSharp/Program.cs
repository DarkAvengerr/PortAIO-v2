#region

using System;
using LeagueSharp.Common;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Tracker
{
    internal class Program
    {
        public static Menu Config;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        private static void Game_OnGameLoad()
        {
            Config = new Menu("Tracker", "Tracker", true);
            HbTracker.AttachToMenu(Config);
            WardTracker.AttachToMenu(Config);
            Config.AddToMainMenu();
        }
    }
}