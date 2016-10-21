using System.Collections.Generic;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Functions
{

    internal static class Names
    {
        #region Public Classes

        /// <summary>
        ///     Contains the menu names
        /// </summary>
        public static class Menu
        {
            #region Public Fields

            public static string BaseItem="MenuSettings.";
            public static string BaseName="Menu Settings";
            public static string DrawingItemBase="Drawing.";
            public static string DrawingNameBase=$"Drawing";
            public static string ItemMenuBase="Items";
            public static string ItemNameBase="Item.";
            public static string LevelItemBase="Level.";
            public static string LevelNameBase="On Level";
            public static string ManaItemBase="Mana.";
            public static string ManaNameBase="Mana";
            public static string MenuDefensiveItemBase=ItemNameBase+"Defensive.";
            public static string MenuDefensiveNameBase="Defensive";
            public static string MenuOffensiveItemBase=ItemNameBase+"Offensive.";
            public static string MenuOffensiveNameBase="Offensive";
            public static string TrinketItemBase="Trinket.";
            public static string TrinketNameBase="Trinket";

            #endregion Public Fields
        }

        #endregion Public Classes

        #region Public Fields

        /// <summary>
        ///     The project name
        /// </summary>
        public static readonly string ProjectName="[Project]Geass";

        /// <summary>
        ///     The settings name
        /// </summary>
        public static readonly string SettingsName="[Project]Geass Modules";

        public static List<string> PredictionMethods=new List<string> {"Common", "SebbyPrediction", "SPrediction"};

        #endregion Public Fields
    }

}