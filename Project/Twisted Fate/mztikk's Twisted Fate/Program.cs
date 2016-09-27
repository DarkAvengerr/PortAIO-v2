using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksTwistedFate
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    class Program
    {
        #region Methods

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
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