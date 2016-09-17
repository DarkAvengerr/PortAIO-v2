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
        private static void Main()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
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