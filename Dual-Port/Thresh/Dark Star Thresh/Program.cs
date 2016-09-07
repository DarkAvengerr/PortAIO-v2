using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Dark_Star_Thresh
{
    class Program
    {
        public static void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Thresh")
            {
                Chat.Print("Could not load Dark Star Thresh");
                return;
            }
            Load.LoadAssembly();
        }
    }
}
