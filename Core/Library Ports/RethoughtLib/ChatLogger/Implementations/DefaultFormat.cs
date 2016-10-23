using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.ChatLogger.Implementations
{
    #region Using Directives

    using LeagueSharp;

    using global::RethoughtLib.ChatLogger.Interfaces;

    #endregion

    public class DefaultFormat : ILogFormat
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Applies the format to the specified arguments.
        /// </summary>
        /// <param name="args">The <see cref="GameChatEventArgs" /> instance containing the event data.</param>
        /// <returns></returns>
        public Message Apply(ChatMessageEventArgs args)
        {
            var message = new Message(args);

            message.FormatedMessage = $"[{message.Time} {message.Sender.Name} ({message.Sender.ChampionName}): {message.Content}";

            return message;
        }

        #endregion
    }
}