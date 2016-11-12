using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElKatarinaDecentralized.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ElKatarinaDecentralized.Enumerations;

    /// <summary>
    ///     The logging class for the console.
    /// </summary>
    internal static class Logging
    {
        #region Static Fields

        /// <summary>
        ///     Contains placeholders for replacements.
        /// </summary>
        private static readonly Dictionary<string, string> Replacements = new Dictionary<string, string>
                                                                              {
                                                                                  {
                                                                                      "[SEPERATOR]",
                                                                                      "----------------------------------------------------"
                                                                                      + Environment.NewLine
                                                                                  }
                                                                              };

        #endregion

        #region Methods

        /// <summary>
        ///     The add entry.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        internal static void AddEntry(LoggingEntryType type, string message, params object[] args)
        {
            switch (type)
            {
                case LoggingEntryType.Debug:
                    AddEntryColored(GetFormattedEntry(type, message, args), ConsoleColor.Green);
                    break;

                case LoggingEntryType.Error:
                    AddEntryColored(GetFormattedEntry(type, message, args), ConsoleColor.DarkRed);
                    break;

                case LoggingEntryType.Generic:
                    AddEntryColored(GetFormattedEntry(type, message, args), ConsoleColor.White);
                    break;

                case LoggingEntryType.Info:
                    AddEntryColored(GetFormattedEntry(type, message, args), ConsoleColor.Yellow);
                    break;

                case LoggingEntryType.Warning:
                    AddEntryColored(GetFormattedEntry(type, message, args), ConsoleColor.Red);
                    break;
            }
        }

        /// <summary>
        ///     The add entry colored.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="color">
        ///     The color.
        /// </param>
        private static void AddEntryColored(string message, ConsoleColor color)
        {
            try
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            }
            catch (Exception e)
            {
                Console.WriteLine("@Logging.cs: Can not set color - {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     Formats a given <see cref="string" /> and adds specific prefixes
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <returns>
        ///     <see cref="string" />
        /// </returns>
        private static string GetFormattedEntry(LoggingEntryType type, string message, params object[] args)
        {
            try
            {
                // ReSharper disable once UseStringInterpolation
                return string.Format(
                    "[ElKatarinaDecentralized {0}] ({1}): {2}",
                    DateTime.Now.ToString("HH:mm:ss"),
                    type,
                    string.Format(ReplaceStrings(message), args));
            }
            catch (Exception e)
            {
                Console.WriteLine("@Logging.cs: Can not format string - {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     Replaces placeholders in a given <see cref="string" />
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     <see cref="string" />
        /// </returns>
        private static string ReplaceStrings(string message)
        {
            try
            {
                return Replacements.Aggregate(
                    message,
                    (current, replacement) => current.Replace(replacement.Key, replacement.Value));
            }
            catch (Exception e)
            {
                Console.WriteLine("@Logging.cs: Can not replace strings - {0}", e);
                throw;
            }
        }

        #endregion
    }
}