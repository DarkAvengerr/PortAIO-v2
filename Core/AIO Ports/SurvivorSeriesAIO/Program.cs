using System;
using LeagueSharp;
using LeagueSharp.Common;
using SurvivorSeriesAIO.Core;
using SurvivorSeriesAIO.SurvivorMain;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO
{
    internal class Program
    {
        /// <summary>
        ///     SurvivorSeries AIO NEWS
        /// </summary>
        public static string SSNews = "Reminders/Improvements/JungleClear for Ryze Added";

        public static IRootMenu RootMenu { get; set; }

        public static IChampion Champion { get; set; }

        public static IActivator Activator { get; set; }

        private static AIHeroClient Player => ObjectManager.Player;

        public static void Main()
        {
            GameOnOnGameLoad();
        }

        /// <summary>
        ///     GameOnOnGameLoad - Load Every Plugin/Addon
        /// </summary>
        /// <param name="args"></param>
        private static void GameOnOnGameLoad()
        {
            #region Subscriptions

            Chat.Print(
                "<font color='#0993F9'>[SurvivorSeries AIO]</font> <font color='#FF8800'>Successfully Loaded.</font>");

            Chat.Print("<font color='#b756c5'>[SurvivorSeries] NEWS: </font>" + SSNews);

            #endregion

            RootMenu = new RootMenu("SurvivorSeries AIO");

            #region Utility Loads
            SpellCast.RootConfig = RootMenu;

            ChampionFactory.Load(ObjectManager.Player.ChampionName, RootMenu);

            ActivatorFactory.Create(ObjectManager.Player.ChampionName, RootMenu);

            AutoLevelerFactory.Create(ObjectManager.Player.ChampionName, RootMenu);

            #endregion
        }
    }
}