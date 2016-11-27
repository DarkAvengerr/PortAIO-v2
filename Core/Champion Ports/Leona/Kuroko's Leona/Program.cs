using System;
using Kuroko_s_Leona.Champions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Kuroko_s_Leona
{
    class Program
    {
        public static void Main()
        {
            OnGameLoad(new EventArgs());
        }

        private static void OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName == "Leona")
            {
                new Leona();
            }
            else
            {
                Chat.Print(ObjectManager.Player.ChampionName + "NOT SUPPORTED");
                return;
            }
        }
    }
}
