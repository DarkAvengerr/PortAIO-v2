using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksCassiopeia
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class Program
    {
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName.ToLower() != "cassiopeia")
            {
                return;
            }

            Spells.LoadSpells();
            Config.BuildMenu();
            Mainframe.Init();
        }
    }
}
