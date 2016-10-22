using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace AmumuSharp
{
    class Program
    {
        public static Helper Helper;

        public static void Game_OnGameLoad()
        {
            Helper = new Helper();
            new Amumu();
        }
    }
}
