using EloBuddy; namespace RethoughtLib.ChatLogger.Interfaces
{
    #region Using Directives

    using LeagueSharp;

    #endregion

    /// <summary>
    ///     The format in which the messages are gettings logged in
    /// </summary>
    public interface ILogFormat
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Applies the format to the specified arguments.
        /// </summary>
        /// <param name="message">The <see cref="GameChatEventArgs" /> instance containing the event data.</param>
        /// <returns></returns>
        Message Apply(AIHeroClient sender, ChatMessageEventArgs args);

        #endregion
    }
}