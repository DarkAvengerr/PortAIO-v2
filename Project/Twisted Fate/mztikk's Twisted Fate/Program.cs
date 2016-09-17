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

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
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