using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.ChatLogger
{
    #region Using Directives

    using LeagueSharp;

    #endregion

    /// <summary>
    ///     A Chat Message
    /// </summary>
    public class Message
    {
        #region Fields

        /// <summary>
        ///     The content
        /// </summary>
        public readonly string Content;

        /// <summary>
        ///     Whether processing or not
        /// </summary>
        public readonly bool Process;

        /// <summary>
        ///     The sender
        /// </summary>
        public readonly AIHeroClient Sender;

        /// <summary>
        ///     The time the message got send
        /// </summary>
        public readonly float Time;

        /// <summary>
        ///     The formated message
        /// </summary>
        public string FormatedMessage;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Message" /> class.
        /// </summary>
        /// <param name="args">The <see cref="GameChatEventArgs" /> instance containing the event data.</param>
        public Message(ChatMessageEventArgs args)
        {
            this.Content = args.Message;
            this.Sender = args.Sender;
            this.Time = Game.Time;
            this.Process = args.Process;
        }

        #endregion
    }
}