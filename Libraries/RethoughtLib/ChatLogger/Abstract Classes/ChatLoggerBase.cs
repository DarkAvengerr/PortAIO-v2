using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.ChatLogger.Abstract_Classes
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using global::RethoughtLib.ChatLogger.Implementations;
    using global::RethoughtLib.ChatLogger.Interfaces;

    using LeagueSharp;

    using EventHandler = global::RethoughtLib.Events.EventHandler;

    #endregion

    public abstract class ChatLoggerBase
    {
        #region Fields

        /// <summary>
        ///     The logged messages
        /// </summary>
        public List<Message> LoggedMessages = new List<Message>();

        /// <summary>
        ///     The IChatLoggers
        /// </summary>
        protected readonly List<IChatLogger> ChatLoggers = new List<IChatLogger>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChatLogger" /> class.
        /// </summary>
        protected ChatLoggerBase()
        {
            Chat.OnMessage += this.GameOnOnChat;
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when [message logged].
        /// </summary>
        public event EventHandler MessageLogged;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the specified IChatLoggers.
        /// </summary>
        /// <param name="logger">The IChatLoggers.</param>
        /// <exception cref="System.InvalidOperationException">Can't add multiple similar instances of IChatLogger</exception>
        public abstract void Add(IChatLogger logger);

        /// <summary>
        ///     Removes the specified IChatLoggers.
        /// </summary>
        /// <param name="logger">The IChatLoggers.</param>
        /// <exception cref="System.InvalidOperationException">Can't remove an IChatLogger that is not added.</exception>
        public abstract void Remove(IChatLogger logger);

        #endregion

        #region Methods

        /// <summary>
        ///     Triggers on Game Chat.
        /// </summary>
        /// <param name="args">The <see cref="GameChatEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected abstract void GameOnOnChat(AIHeroClient sender, ChatMessageEventArgs args);

        /// <summary>
        ///     Called when [message logged].
        /// </summary>
        protected virtual void OnMessageLogged()
        {
            this.MessageLogged?.Invoke(EventArgs.Empty);
        }

        /// <summary>
        ///     Updates the logged strings.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected abstract void UpdateLogged(IChatLogger logger);

        #endregion
    }
}