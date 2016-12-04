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

    using RethoughtLib.ChatLogger.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    public class ChatLoggerModule : ChildBase
    {
        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChatLoggerModule" /> class.
        /// </summary>
        /// <param name="chatLogger">The chat logger.</param>
        public ChatLoggerModule(ChatLoggerBase chatLogger)
        {
            this.ChatLogger = chatLogger;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the chat logger.
        /// </summary>
        /// <value>
        ///     The chat logger.
        /// </value>
        public ChatLoggerBase ChatLogger { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; }

        #endregion
    }
}