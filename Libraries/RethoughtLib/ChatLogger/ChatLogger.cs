using EloBuddy; namespace RethoughtLib.ChatLogger
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using LeagueSharp;

    using Interfaces;

    #endregion

    /// <summary>
    ///     Class to log chats
    /// </summary>
    public class ChatLogger
    {
        #region Fields

        public List<Message> LoggedMessages = new List<Message>();

        /// <summary>
        ///     The IChatLoggers
        /// </summary>
        private readonly List<IChatLogger> chatLoggers = new List<IChatLogger>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChatLogger" /> class.
        /// </summary>
        public ChatLogger()
        {
            Chat.OnMessage += this.GameOnOnChat;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the specified IChatLoggers.
        /// </summary>
        /// <param name="logger">The IChatLoggers.</param>
        /// <exception cref="System.InvalidOperationException">Can't add multiple similiar instances of IChatLogger</exception>
        public void Add(IChatLogger logger)
        {
            if (this.chatLoggers.Contains(logger))
            {
                throw new InvalidOperationException("Can't add multiple similiar instances of IChatLogger");
            }

            this.chatLoggers.Add(logger);
        }

        /// <summary>
        ///     Removes the specified IChatLoggers.
        /// </summary>
        /// <param name="logger">The IChatLoggers.</param>
        /// <exception cref="System.InvalidOperationException">Can't remove an IChatLogger that is not added.</exception>
        public void Remove(IChatLogger logger)
        {
            if (!this.chatLoggers.Contains(logger))
            {
                throw new InvalidOperationException("Can't remove an IChatLogger that is not added.");
            }

            this.chatLoggers.Remove(logger);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Triggers on Game Chat.
        /// </summary>
        /// <param name="args">The <see cref="GameChatEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void GameOnOnChat(AIHeroClient sender, ChatMessageEventArgs args)
        {
            foreach (var logger in this.chatLoggers)
            {
                logger.Log(sender, args);

                this.UpdateLogged(logger);
            }
        }

        /// <summary>
        ///     Updates the logged strings.
        /// </summary>
        /// <param name="logger">The logger.</param>
        private void UpdateLogged(IChatLogger logger)
        {
            foreach (var messageToAdd in logger.Logged)
            {
                foreach (var existingMessage in this.LoggedMessages)
                {
                    if (messageToAdd.Equals(existingMessage))
                    {
                        continue;
                    }

                    this.LoggedMessages.Add(messageToAdd);
                }
            }
        }

        #endregion
    }
}