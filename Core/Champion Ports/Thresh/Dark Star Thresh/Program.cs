using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Dark_Star_Thresh
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Program
    {
        private static void Main()
        {
            OnLoad();
        }

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
