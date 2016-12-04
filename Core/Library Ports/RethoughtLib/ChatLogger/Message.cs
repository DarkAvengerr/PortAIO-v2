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
namespace RethoughtLib.ChatLogger
{
    #region Using Directives

    using LeagueSharp;

    #endregion

    /// <summary>
    ///     A Chat Message
    /// </summary>
    public class Message
    {
        #region Fields

        /// <summary>
        ///     The content
        /// </summary>
        public readonly string Content;

        /// <summary>
        ///     Whether processing or not
        /// </summary>
        public readonly bool Process;

        /// <summary>
        ///     The sender
        /// </summary>
        public readonly AIHeroClient Sender;

        /// <summary>
        ///     The time the message got send
        /// </summary>
        public readonly float Time;

        /// <summary>
        ///     The formated message
        /// </summary>
        public string FormatedMessage;

        #endregion

        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Message" /> class.
        /// </summary>
        /// <param name="args">The <see cref="GameChatEventArgs" /> instance containing the event data.</param>
        public Message(AIHeroClient sender, ChatMessageEventArgs args)
        {
            this.Content = args.Message;
            this.Sender = args.Sender;
            this.Time = Game.Time;
            this.Process = args.Process;
        }

        #endregion

        #endregion
    }
}