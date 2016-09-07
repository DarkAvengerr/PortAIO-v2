using LeagueSharp.SDK;
using System;

using EloBuddy;
using LeagueSharp.SDK;
namespace Preserved_Kassadin
{
    class Program
    {
        public static void Main()
        {
            Bootstrap.Init();
            Load();
        }
        private static void Load()
        {
            if (GameObjects.Player.ChampionName != "Kassadin") return;
            LoadAssembly.Load();
        }
    }
}
