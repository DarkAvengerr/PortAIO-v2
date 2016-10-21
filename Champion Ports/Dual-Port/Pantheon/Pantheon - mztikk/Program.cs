using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkPantheon
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class Program
    {
        #region Methods

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName.ToLower() != "pantheon")
            {
                return;
            }

            Spells.LoadSpells();
            Config.BuildMenu();
            Mainframe.Init();
        }

        #endregion
    }
}