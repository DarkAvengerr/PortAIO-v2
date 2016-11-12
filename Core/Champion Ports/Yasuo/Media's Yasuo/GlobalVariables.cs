using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.ConnectionTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Media;
    using global::YasuoMedia.CommonEx.CastManager;
    using global::YasuoMedia.Yasuo.LogicProvider;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class GlobalVariables
    {
        #region Static Fields

        /// <summary>
        ///     When the assembly got loaded
        /// </summary>
        public static readonly DateTime AssemblyLoadTime = DateTime.Now;

        /// <summary>
        ///     The assembly
        /// </summary>
        public static Assembly Assembly = null;

        /// <summary>
        ///     The cast manager
        /// </summary>
        public static CastManager CastManager = null;

        /// <summary>
        ///     Debugstate
        /// </summary>
        public static bool Debug = true;

        /// <summary>
        ///     The profile name of the GitHub account
        /// </summary>
        public static string GitHubProfile = "MediaGithub";

        /// <summary>
        ///     The grid generator
        /// </summary>
        public static GridGeneratorContainer<Point, ConnectionBase<Point>> GridGenerator = null;

        /// <summary>
        ///     The root menu
        /// </summary>
        public static Menu RootMenu;

        /// <summary>
        ///     The champion(s) the assembly is for
        /// </summary>
        public static List<string> SupportedChampions = new List<string>() { "yasuo" };

        internal static readonly SteelTempestLogicProvider providerQ = new SteelTempestLogicProvider();

        /// <summary>
        ///     If the assembly is limited to a specific champion
        /// </summary>
        internal static bool ChampionDependent = true;

        /// <summary>
        ///     Directory of where plugins should get loaded from
        /// </summary>
        internal static string PluginDirectory = "";

        /// <summary>
        ///     If the assembly supports Plugins (Plugin-Folder)
        /// </summary>
        internal static bool PluginSupport = false;

        /// <summary>
        ///     If Stop is active the assembly will stop loading
        /// </summary>
        internal static bool Stop = false;

        /// <summary>
        ///     The github PathBase of the assembly
        /// </summary>
        public static string GitHubPath = $"{GitHubProfile}/LeagueSharp/tree/master/{Name}";

        /// <summary>
        ///     Dictionary containing all spells
        /// </summary>
        public static Dictionary<SpellSlot, Spell> Spells = new Dictionary<SpellSlot, Spell>()
                                                                {
                                                                    {
                                                                        SpellSlot.Q,
                                                                        new Spell(
                                                                        SpellSlot.Q,
                                                                        475)
                                                                    },
                                                                    {
                                                                        SpellSlot.W,
                                                                        new Spell(
                                                                        SpellSlot.W,
                                                                        400)
                                                                    },
                                                                    {
                                                                        SpellSlot.E,
                                                                        new Spell(
                                                                        SpellSlot.E,
                                                                        475,
                                                                        TargetSelector
                                                                        .DamageType
                                                                        .Magical)
                                                                    },
                                                                    {
                                                                        SpellSlot.R,
                                                                        new Spell(
                                                                        SpellSlot.R,
                                                                        1200)
                                                                    },
                                                                };

        #endregion

        #region Public Properties

        /// <summary>
        ///     The Author of the assembly
        /// </summary>
        public static string Author => "Media";

        public static string DisplayName => string.Format("[{2}] {0}: {1}", Author, Name, Prefix);

        /// <summary>
        ///     The name of the assembly
        /// </summary>
        public static string Name => $"MediaSuo";

        /// <summary>
        ///     The Orbwalker
        /// </summary>
        public static Orbwalking.Orbwalker Orbwalker { get; internal set; }

        /// <summary>
        ///     The Player
        /// </summary>
        public static AIHeroClient Player => ObjectManager.Player;

        /// <summary>
        ///     Different prefixes of the assembly. aka: WIP, BETA, ALPHA, TOBEUPDATED, BEST
        /// </summary>
        public static string Prefix => "WIP";

        #endregion
    }
}