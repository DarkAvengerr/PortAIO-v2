using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.ChatLogger.Implementations
{
    #region Using Directives

    using System;

    using global::RethoughtLib.ChatLogger.Abstract_Classes;
    using global::RethoughtLib.ChatLogger.Interfaces;

    using LeagueSharp;

    #endregion

    /// <summary>
    ///     Class to log chats
    /// </summary>
    public class ChatLogger : ChatLoggerBase
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Adds the specified IChatLoggers.
        /// </summary>
        /// <param name="logger">The IChatLoggers.</param>
        /// <exception cref="System.InvalidOperationException">Can't add multiple similar instances of IChatLogger</exception>
        public override void Add(IChatLogger logger)
        {
            if (this.ChatLoggers.Contains(logger))
            {
                throw new InvalidOperationException("Can't add multiple similar instances of IChatLogger");
            }

            this.ChatLoggers.Add(logger);
        }

        /// <summary>
        ///     Removes the specified IChatLoggers.
        /// </summary>
        /// <param name="logger">The IChatLoggers.</param>
        /// <exception cref="System.InvalidOperationException">Can't remove an IChatLogger that is not added.</exception>
        public override void Remove(IChatLogger logger)
        {
            if (!this.ChatLoggers.Contains(logger))
            {
                throw new InvalidOperationException("Can't remove an IChatLogger that is not added.");
            }

            this.ChatLoggers.Remove(logger);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Triggers on Game Chat.
        /// </summary>
        /// <param name="args">The <see cref="GameChatEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override void GameOnOnChat(AIHeroClient sender, ChatMessageEventArgs args)
        {
            foreach (var logger in this.ChatLoggers)
            {
                logger.Log(sender, args);

                this.UpdateLogged(logger);
            }
        }

        /// <summary>
        ///     Updates the logged strings.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected override void UpdateLogged(IChatLogger logger)
        {
            foreach (var messageToAdd in logger.Logged)
            {
                foreach (var existingMessage in this.LoggedMessages)
                {
                    if (messageToAdd.Equals(existingMessage))
                    {
                        continue;
                    }

                    this.OnMessageLogged();
                    this.LoggedMessages.Add(messageToAdd);
                }
            }
        }

        #endregion
    }
}