using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace KurisuMorgana
{
    internal class Program
    {
        public static void Game_OnGameLoad()
        {
            new KurisuMorgana();
        }
    }
}
