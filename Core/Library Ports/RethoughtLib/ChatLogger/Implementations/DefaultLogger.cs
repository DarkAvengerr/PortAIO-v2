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

    using System.Collections.Generic;

    using LeagueSharp;

    using RethoughtLib.ChatLogger.Interfaces;

    #endregion

    /// <summary>
    ///     Default Loggers that implements IChatLogger and logs everything in the chat.
    /// </summary>
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

        #region IChatLogger Members

        #region Public Methods and Operators

        /// <summary>
        ///     Logs this instance.
        /// </summary>
        /// <param name="args">Contextual information</param>
        public void Log(AIHeroClient sender, ChatMessageEventArgs args)
        {
            var message = this.Format.Apply(sender, args);

            if (!string.IsNullOrWhiteSpace(message.FormatedMessage) && !string.IsNullOrEmpty(message.Content)) this.Logged.Add(message);
        }

        #endregion

        #endregion

        #endregion
    }
}