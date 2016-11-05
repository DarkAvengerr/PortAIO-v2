using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloDZLib.Logging
{
    class LogHelper
    {
        /// <summary>
        /// The log items
        /// </summary>
        private static List<LogItem> logItems = new List<LogItem>();

        /// <summary>
        /// Gets the log path.
        /// </summary>
        /// <value>
        /// The log path.
        /// </value>
        private static String LogPath => Path.Combine(LogVariables.WorkingDir,
            $"[{LogVariables.AssemblyName}] Log - {Game.GameId} - {DateTime.Now.ToString("dd_MM_yyyy")}.txt");

        /// <summary>
        /// Called when the LogHelper is loaded
        /// </summary>
        public static void OnLoad()
        {
        }

        /// <summary>
        /// Adds an item to the log.
        /// </summary>
        /// <param name="logItem">The log item.</param>
        public static void AddToLog(LogItem logItem)
        {
        }

        /// <summary>
        /// Gets the logs.
        /// </summary>
        /// <returns></returns>
        public static List<LogItem> GetLogs()
        {
            return logItems;
        }

        /// <summary>
        /// Clears the logs.
        /// </summary>
        public static void ClearLogs()
        {
            logItems.Clear();
        }

        /// <summary>
        /// Creates the directory.
        /// </summary>
        private static void CreateDirectory()
        {
        }

        /// <summary>
        /// Initializes the log file.
        /// </summary>
        private static void InitLog()
        {
        }

        /// <summary>
        /// Saves to the log file.
        /// </summary>
        /// <param name="logItem">The log item.</param>
        private static void SaveToFile(LogItem logItem)
        {
        }
    }
}
