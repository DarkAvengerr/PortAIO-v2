using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace kSkarner
{
    internal class Program
    {

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName.ToLowerInvariant() == "skarner")
            {
                // Abrir o Skarner.cs
                kSkarner.LoadkSkarner();
            }
        }
    }
}