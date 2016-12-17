using System;
using Darwinn_s_Hecarim.Champions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Enurped_s
{
    class Program
    {
        public static void Main()
        {
            OnGameLoad(new EventArgs());
        }

        private static void OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName == "Hecarim")
            {
                new Hecarim();
            }
            else
            {
                Chat.Print(ObjectManager.Player.ChampionName + "NOT SUPPORTED");
                return;
            }
        }
    }
}