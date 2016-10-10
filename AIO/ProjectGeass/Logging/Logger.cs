using System;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Logging
{

    public class Logger
    {
        #region Private Fields

        private readonly string _baseName;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Logger" /> class.
        /// </summary>
        /// <param name="_base">
        ///     The base.
        /// </param>
        public Logger(string _base) {_baseName=_base;}

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        ///     Writes the log.
        /// </summary>
        /// <param name="text">
        ///     The text.
        /// </param>
        public void WriteLog(string text) {Console.WriteLine($"{_baseName}:{text}");}

        /// <summary>
        ///     Writes the log.
        /// </summary>
        /// <param name="text">
        ///     The text.
        /// </param>
        /// <param name="color">
        ///     The color.
        /// </param>
        public void WriteLog(string text, ConsoleColor color)
        {
            var holder=Console.ForegroundColor;
            Console.ForegroundColor=color;
            Console.WriteLine($"{_baseName}:{text}");
            Console.ForegroundColor=holder;
        }

        #endregion Public Methods
    }

}