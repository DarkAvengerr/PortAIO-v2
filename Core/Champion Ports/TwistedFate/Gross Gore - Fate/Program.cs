using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate
{
    #region Use

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    class Program
    {
        #region Methods

        public static void Main()
        {
            OnGameLoad();
        }

        private static void OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName.ToLower() != "twistedfate")
            {
                return;
            }

            Spells.LoadSpells();
            Config.BuildConfig();
            Mainframe.Init();
        }

        #endregion
    }
}