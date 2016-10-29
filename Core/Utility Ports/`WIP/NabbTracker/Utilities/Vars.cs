using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace NabbTracker
{
    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.SDK.UI;

    using Font = SharpDX.Direct3D9.Font;

    /// <summary>
    ///     The Vars class.
    /// </summary>
    internal class Vars
    {
        #region Static Fields

        /// <summary>
        ///     A list of the names of the champions who have a different healthbar type.
        /// </summary>
        public static readonly List<string> SpecialChampions = new List<string> { "Annie", "Jhin" };

        /// <summary>
        ///     Gets the SummonerSpell name.
        /// </summary>
        public static string GetSummonerSpellName;

        /// <summary>
        ///     Gets the Color.
        /// </summary>
        public static Color SdColor = Color.Black;

        /// <summary>
        ///     Gets the Color.
        /// </summary>
        public static SharpDX.Color SdxColor = SharpDX.Color.Black;

        /// <summary>
        ///     Gets the spellslots.
        /// </summary>
        public static SpellSlot[] SpellSlots = { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };

        /// <summary>
        ///     Gets the summoner spellslots.
        /// </summary>
        public static SpellSlot[] SummonerSpellSlots = { SpellSlot.Summoner1, SpellSlot.Summoner2 };

        #endregion

        #region Public Properties

        /// <summary>
        ///     The Colorblind Menu.
        /// </summary>
        public static Menu ColorblindMenu { internal get; set; }

        /// <summary>
        ///     The Text fcnt.
        /// </summary>
        public static Font DisplayTextFont { get; set; } = new Font(
            Drawing.Direct3DDevice,
            new System.Drawing.Font("Tahoma", 8));

        /// <summary>
        ///     The ExpTracker Menu.
        /// </summary>
        public static Menu ExpTrackerMenu { internal get; set; }

        /// <summary>
        ///     The Exp Healthbars X coordinate.
        /// </summary>
        public static int ExpX { internal get; set; }

        /// <summary>
        ///     The Exp Healthbars Y coordinate.
        /// </summary>
        public static int ExpY { internal get; set; }

        /// <summary>
        ///     The Main Menu.
        /// </summary>
        public static Menu Menu { internal get; set; }

        /// <summary>
        ///     The Miscellaneous Menu.
        /// </summary>
        public static Menu MiscMenu { internal get; set; }

        /// <summary>
        ///     The SpellLevel X coordinate.
        /// </summary>
        public static int SpellLevelX { internal get; set; }

        /// <summary>
        ///     The Healthbars Y coordinate.
        /// </summary>
        public static int SpellLevelY { internal get; set; }

        /// <summary>
        ///     The SpellTracker Menu.
        /// </summary>
        public static Menu SpellTrackerMenu { internal get; set; }

        /// <summary>
        ///     The Spells Healthbars X coordinate.
        /// </summary>
        public static int SpellX { internal get; set; }

        /// <summary>
        ///     The Spells Healthbars Y coordinate.
        /// </summary>
        public static int SpellY { internal get; set; }

        /// <summary>
        ///     The SummonerSpells Healthbar X coordinate.
        /// </summary>
        public static int SummonerSpellX { internal get; set; }

        /// <summary>
        ///     The SummonerSpells Healthbar Y coordinate.
        /// </summary>
        public static int SummonerSpellY { internal get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The Exp Healthbars X coordinate adjustment.
        /// </summary>
        public static int ExpXAdjustment(AIHeroClient target)
        {
            return SpecialChampions.Contains(target.ChampionName) ? 77 : 85;
        }

        /// <summary>
        ///     The Spells Healthbars Y coordinate adjustment.
        /// </summary>
        public static int ExpYAdjustment(AIHeroClient target)
        {
            if (SpecialChampions.Contains(target.ChampionName))
            {
                return Menu["miscellaneous"]["name"].GetValue<MenuBool>().Value ? -47 : -38;
            }

            return target.IsMe
                       ? Menu["miscellaneous"]["name"].GetValue<MenuBool>().Value ? -40 : -30
                       : Menu["miscellaneous"]["name"].GetValue<MenuBool>().Value ? -33 : -22;
        }

        /// <summary>
        ///     The Spells Healthbars X coordinate adjustment.
        /// </summary>
        public static int SpellXAdjustment(AIHeroClient target)
        {
            if (SpecialChampions.Contains(target.ChampionName))
            {
                return target.IsMe ? 34 : 17;
            }

            return target.IsMe ? 55 : 10;
        }

        /// <summary>
        ///     The Spells Healthbars Y coordinate adjustment.
        /// </summary>
        public static int SpellYAdjustment(AIHeroClient target)
        {
            if (SpecialChampions.Contains(target.ChampionName))
            {
                return 25;
            }

            return target.IsMe ? 25 : 35;
        }

        /// <summary>
        ///     The Healthbars X coordinate adjustment.
        /// </summary>
        public static int SummonerSpellXAdjustment(AIHeroClient target)
        {
            if (SpecialChampions.Contains(target.ChampionName))
            {
                return 2;
            }

            return 10;
        }

        /// <summary>
        ///     SummonerSpells The Healthbars Y coordinate adjustment.
        /// </summary>
        public static int SummonerSpellYAdjustment(AIHeroClient target)
        {
            if (SpecialChampions.Contains(target.ChampionName))
            {
                return -12;
            }

            return target.IsMe ? -4 : 4;
        }

        #endregion
    }
}