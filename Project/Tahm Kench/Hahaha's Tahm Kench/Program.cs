using LeagueSharp;
using LeagueSharp.Common;
using System;

using EloBuddy;
using LeagueSharp.Common;
namespace Hahaha_s_Tahm_Kench
{
    class Program
    {
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
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
