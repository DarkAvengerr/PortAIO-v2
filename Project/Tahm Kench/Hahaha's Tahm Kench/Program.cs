using LeagueSharp;
using LeagueSharp.Common;
using System;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Hahaha_s_Tahm_Kench
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.CharData.BaseSkinName != "Tahm Kench")
            {
                return;
            }

            Tahm_Kench.OnLoad();
            Chat.Print("Welcome to Hahaha's Tahm Kench!");
        }
    }
}
