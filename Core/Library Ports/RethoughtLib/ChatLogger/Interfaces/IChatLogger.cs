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
namespace RethoughtLib.ChatLogger.Interfaces
{
    #region Using Directives

    using System.Collections.Generic;

    using LeagueSharp;

    #endregion

    /// <summary>
    ///     Inteface for Chat logging.
    /// </summary>
    public interface IChatLogger
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the format.
        /// </summary>
        /// <value>
        ///     The format.
        /// </value>
        ILogFormat Format { get; set; }

        /// <summary>
        ///     Gets or sets the logged strings.
        /// </summary>
        /// <value>
        ///     The logged.
        /// </value>
        List<Message> Logged { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Logs this instance.
        /// </summary>
        void Log(AIHeroClient sender, ChatMessageEventArgs args);

        #endregion
    }
}