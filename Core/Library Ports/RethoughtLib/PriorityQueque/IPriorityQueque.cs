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
//     Last Edited: 04.10.2016 1:44 PM

using EloBuddy; 
using LeagueSharp.Common; 
namespace RethoughtLib.PriorityQueque
{
    public interface IPriorityQueque<T>
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Changes the priority.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="priority">The priority.</param>
        void ChangePriority(T item, int priority);

        /// <summary>
        ///     Removes all T.
        /// </summary>
        void Clear();

        /// <summary>
        ///     Returns the next T to be dequeded/processed.
        /// </summary>
        /// <returns></returns>
        T Dequeque();

        /// <summary>
        ///     Enqueques the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="priority">The priority.</param>
        void Enqueque(T item, int priority);

        #endregion
    }
}