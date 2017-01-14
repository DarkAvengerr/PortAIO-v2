using EloBuddy; 
using LeagueSharp.Common; 
namespace MoonRiven
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Program
    {
        public static void Main()
        {
            OnGameLoad(new EventArgs());
        }

        private static void OnGameLoad(EventArgs Args)
        {
            if (ObjectManager.Player.ChampionName != "Riven")
            {
                return;
            }

            Riven.LoadAssembly();
        }
    }
}
