using Geass_Tristana.Misc;
using LeagueSharp;
using LeagueSharp.Common;
using System;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Geass_Tristana
{
    internal class Program : Core
    {
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Tristana")
                return;
            if (ObjectManager.Player == null)
            {
                Console.WriteLine($"Null player");
            }
            Libaries.Loader.LoadAssembly();
        }
    }
}