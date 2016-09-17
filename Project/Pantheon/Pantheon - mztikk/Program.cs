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

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
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