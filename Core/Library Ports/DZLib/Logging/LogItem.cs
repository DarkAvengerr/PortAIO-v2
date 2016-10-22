using System;
using System.Globalization;
using LeagueSharp;
using EloBuddy;

namespace DZLib.Logging
{
    public class LogItem
    {
        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public LogSeverity Severity { get; set; }

        /// <summary>
        /// The log exception
        /// </summary>
        public string LogException;

        /// <summary>
        /// The module the exception is from
        /// </summary>
        public string Module;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogItem"/> class.
        /// </summary>
        /// <param name="Module">The module.</param>
        /// <param name="Exception">The exception.</param>
        /// <param name="Severity">The severity.</param>
        public LogItem(string Module, object Exception, LogSeverity Severity = LogSeverity.Medium)
        {
            this.Module = Module;
            this.LogException = Exception.ToString();
            this.Severity = Severity;
        }

        /// <summary>
        /// Gets the logging string.
        /// </summary>
        /// <returns></returns>
        public string GetLoggingString()
        {
            return string.Format("\r\n[{3} | From: {0} | Severity: {1}] {2}\r\n",
                Module,
                Severity,
                LogException,
                (Game.Time / 60f).ToString().Remove(5, (Game.Time / 60f).ToString().Length - 5).Replace(",", ":"));
        }
    }
}
