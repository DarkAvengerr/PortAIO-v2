using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Soraka_HealBot
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Program
    {
        #region Methods
        public static void OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName.ToLower() != "soraka")
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