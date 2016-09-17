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
            Console.WriteLine(@"[{0}] >>> Logger loaded successfully!", LogVariables.AssemblyName);
        }

        /// <summary>
        /// Adds an item to the log.
        /// </summary>
        /// <param name="logItem">The log item.</param>
        public static void AddToLog(LogItem logItem)
        {
            CreateDirectory();
            logItems.Add(logItem);
            SaveToFile(logItem);
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
            if (!Directory.Exists(LogVariables.WorkingDir))
            {
                Directory.CreateDirectory(LogVariables.WorkingDir);
            }

            if (!File.Exists(LogPath))
            {
                //File.Create(LogPath);
                InitLog();
            }
        }

        /// <summary>
        /// Initializes the log file.
        /// </summary>
        private static void InitLog()
        {
            var logString = string.Format("[{0} | {1} | {2} | Assembly Version. {3}] \r\n[Enemies: {4}] \r\n[Allies: {5}] \r\n \r\n", DateTime.Now.ToString("yyyy_MM_dd"),
                    LogVariables.GameRegion,
                    LogVariables.GameVersion,
                    LogVariables.AssemblyVersion,
                    LogVariables.EnemyTeam.Aggregate("", (current, en) => current + (" " + en)),
                    LogVariables.OwnTeam.Aggregate("", (current, en) => current + (" " + en)));
            try
            {
                File.AppendAllText(LogPath, logString);
            }
            catch
            {
                Console.WriteLine(@"[DZAwareness] >>> Exception: Cannot Write To Logs File.");
            }
        }

        /// <summary>
        /// Saves to the log file.
        /// </summary>
        /// <param name="logItem">The log item.</param>
        private static void SaveToFile(LogItem logItem)
        {
            try
            {
                File.AppendAllText(LogPath, logItem.GetLoggingString());
            }
            catch
            {
                Console.WriteLine(@"[DZAwareness] >>> Exception: Cannot Write To Logs File.");
            }
        }
    }
}
