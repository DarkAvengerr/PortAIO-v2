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
namespace RethoughtLib.ActionManager.Abstract_Classes
{
    #region Using Directives

    using System;

    using RethoughtLib.PriorityQuequeV2;

    #endregion

    public interface IActionManager
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the queque.
        /// </summary>
        /// <value>
        ///     The queque.
        /// </value>
        PriorityQueue<int, Action> Queue { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Processes all items that are supposed to get casted.
        /// </summary>
        void Process();

        #endregion
    }
}