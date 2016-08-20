using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy;
using LeagueSharp.Common;
namespace kSkarner
{
    internal class Program
    {

        public static void Main()
        {
            Game_OnGameLoad();
        }

        private static void Game_OnGameLoad()
        {
            kSkarner.LoadkSkarner();
        }
    }
}