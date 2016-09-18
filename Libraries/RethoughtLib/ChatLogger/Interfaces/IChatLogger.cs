using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.ChatLogger.Interfaces
{
    #region Using Directives

    using System.Collections.Generic;

    using LeagueSharp;

    #endregion

    /// <summary>
    ///     Inteface for Chat logging.
    /// </summary>
    public interface IChatLogger
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the format.
        /// </summary>
        /// <value>
        ///     The format.
        /// </value>
        ILogFormat Format { get; set; }

        /// <summary>
        ///     Gets or sets the logged strings.
        /// </summary>
        /// <value>
        ///     The logged.
        /// </value>
        List<Message> Logged { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Logs this instance.
        /// </summary>
        void Log(AIHeroClient sender, ChatMessageEventArgs args);

        #endregion
    }
}