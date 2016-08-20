using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.ChatLogger.Implementations
{
    #region Using Directives

    using System.Collections.Generic;

    using LeagueSharp;

    using global::RethoughtLib.ChatLogger.Interfaces;

    #endregion

    /// <summary>
    ///     Default Loggers that implements IChatLogger and logs everything in the chat.
    /// </summary>
    /// <seealso cref="RethoughtLib.ChatLogger.Interfaces.IChatLogger" />
    public class DefaultLogger : IChatLogger
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the format.
        /// </summary>
        /// <value>
        ///     The format.
        /// </value>
        public ILogFormat Format { get; set; } = new DefaultFormat();

        /// <summary>
        ///     Gets or sets the logged strings.
        /// </summary>
        /// <value>
        ///     The logged.
        /// </value>
        public List<Message> Logged { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Logs this instance.
        /// </summary>
        public void Log(ChatMessageEventArgs args)
        {
            var message = this.Format.Apply(args);

            if (!string.IsNullOrWhiteSpace(message.FormatedMessage) && !string.IsNullOrEmpty(message.Content))
            {
                this.Logged.Add(message);
            }
        }

        #endregion
    }
}