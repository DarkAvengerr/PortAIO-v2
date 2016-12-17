using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Fiora
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Program
    {
        public static void Main()
        {
            OnLoad(new EventArgs());
        }

        private static void OnLoad(EventArgs Args)
        {
            if (ObjectManager.Player.ChampionName != "Fiora")
            {
                return;
            }

            Logic.Load();
        }
    }
}
