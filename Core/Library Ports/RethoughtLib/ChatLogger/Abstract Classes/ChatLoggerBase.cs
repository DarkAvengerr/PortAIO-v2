//     Copyright (C) 2016 Rethought
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
//     Created: 04.10.2016 1:05 PM
//     Last Edited: 04.10.2016 1:43 PM

using EloBuddy; 
using LeagueSharp.Common; 
 namespace RethoughtLib.ChatLogger.Abstract_Classes
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using LeagueSharp;

    using RethoughtLib.ChatLogger.Implementations;
    using RethoughtLib.ChatLogger.Interfaces;

    using EventHandler = RethoughtLib.Events.EventHandler;

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

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChatLogger" /> class.
        /// </summary>
        protected ChatLoggerBase()
        {
            Chat.OnMessage += this.GameOnOnChat;
        }

        #endregion

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