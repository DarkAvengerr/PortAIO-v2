using System;
using System.IO;
using System.Linq;
using System.Reflection;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloDZLib.Logging
{
    class LogVariables
    {
        public static string AssemblyName { get; set; } = "DZLib";

        /// <summary>
        /// Gets the leaguesharp application data folder.
        /// </summary>
        /// <value>
        /// The leaguesharp application data folder.
        /// </value>
        public static string LeagueSharpAppData => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LS" + Environment.UserName.GetHashCode().ToString("X"));

        /// <summary>
        /// Gets the working dir.
        /// </summary>
        /// <value>
        /// The working dir.
        /// </value>
        public static String WorkingDir => Path.Combine(LeagueSharpAppData, AssemblyName);

        /// <summary>
        /// Gets the enemy team.
        /// </summary>
        /// <value>
        /// The enemy team.
        /// </value>
        public static string[] EnemyTeam
        {
            get { return HeroManager.Enemies.Select(en => en.ChampionName).ToArray(); }
        }

        /// <summary>
        /// Gets the own team.
        /// </summary>
        /// <value>
        /// The own team.
        /// </value>
        public static string[] OwnTeam
        {
            get { return HeroManager.Allies.Select(en => en.ChampionName).ToArray(); }
        }

        /// <summary>
        /// Gets the assembly version.
        /// </summary>
        /// <value>
        /// The assembly version.
        /// </value>
        public static string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// Gets the game version.
        /// </summary>
        /// <value>
        /// The game version.
        /// </value>
        public static string GameVersion => Game.Version;

        /// <summary>
        /// Gets the game region.
        /// </summary>
        /// <value>
        /// The game region.
        /// </value>
        public static string GameRegion => Game.Region;
    }
}
