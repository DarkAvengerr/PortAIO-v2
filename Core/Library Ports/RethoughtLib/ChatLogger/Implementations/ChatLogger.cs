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
 namespace RethoughtLib.ChatLogger.Implementations
{
    #region Using Directives

    using System;

    using LeagueSharp;

    using RethoughtLib.ChatLogger.Abstract_Classes;
    using RethoughtLib.ChatLogger.Interfaces;

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
            if (this.ChatLoggers.Contains(logger)) throw new InvalidOperationException("Can't add multiple similar instances of IChatLogger");

            this.ChatLoggers.Add(logger);
        }

        /// <summary>
        ///     Removes the specified IChatLoggers.
        /// </summary>
        /// <param name="logger">The IChatLoggers.</param>
        /// <exception cref="System.InvalidOperationException">Can't remove an IChatLogger that is not added.</exception>
        public override void Remove(IChatLogger logger)
        {
            if (!this.ChatLoggers.Contains(logger)) throw new InvalidOperationException("Can't remove an IChatLogger that is not added.");

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
                foreach (var existingMessage in this.LoggedMessages)
                {
                    if (messageToAdd.Equals(existingMessage)) continue;

                    this.OnMessageLogged();
                    this.LoggedMessages.Add(messageToAdd);
                }
        }

        #endregion
    }
}