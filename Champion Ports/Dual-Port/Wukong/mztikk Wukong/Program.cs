using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Wukong
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class Program
    {
        #region Methods

        public static void Main()
        {
            OnGameLoad();
        }

        private static void OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName.ToLower() != "monkeyking")
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