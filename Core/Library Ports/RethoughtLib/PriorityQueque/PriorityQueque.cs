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
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    #endregion

    /// <summary>
    ///     Default implementation of a PriorityQueque using a SortedList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueque<T> : IPriorityQueque<T>
    {
        #region Fields

        /// <summary>
        ///     The items
        /// </summary>
        internal readonly SortedList<T, int> Items;

        /// <summary>
        ///     The capacity
        /// </summary>
        protected int capacity = 0;

        #endregion

        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PriorityQueque{T}" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public PriorityQueque(int capacity)
        {
            this.Capacity = capacity;

            this.Items = new SortedList<T, int>(capacity);
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the capacity.
        /// </summary>
        /// <value>
        ///     The capacity.
        /// </value>
        protected int Capacity
        {
            get
            {
                return this.capacity;
            }
            set
            {
                if (value > 0) this.capacity = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Changes the priority.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="priority">The priority.</param>
        public void ChangePriority(T item, int priority)
        {
            this.Items.Remove(item);
            this.Items.Add(item, priority);
        }

        /// <summary>
        ///     Removes all T.
        /// </summary>
        public void Clear()
        {
            this.Items.Clear();
        }

        /// <summary>
        ///     Determines whether the list contains specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool ContainsItem(T item) => this.Items.Any(x => x.Key.Equals(item));

        /// <summary>
        ///     Determines whether the list contains specified priority.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <returns></returns>
        public bool ContainsPriority(int priority) => this.Items.Any(x => x.Value == priority);

        /// <summary>
        ///     Returns the next T to be dequeded/processed.
        /// </summary>
        /// <returns></returns>
        public T Dequeque()
        {
            var keyvalue = this.Items.FirstOrDefault();

            return keyvalue.Key;
        }

        /// <summary>
        ///     Enqueques the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="priority">The priority.</param>
        public void Enqueque(T item, int priority)
        {
            this.Items.Add(item, priority);
        }

        #endregion
    }
}