using System;
using Darwinn_s_velkoz.Plugins;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Darwinn_s_velkoz
{
    class Program
    {
        public static void Main()
        {
            OnGameLoad(new EventArgs());
        }

        private static void OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName == "Velkoz")
            {
                new Darwinn_s_velkoz.Plugins.Velkoz();
            }
            else
            {
                Chat.Print(ObjectManager.Player.ChampionName + "NOT SUPPORTED");
                return;
            }
        }
    }
}