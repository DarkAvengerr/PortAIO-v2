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
namespace RethoughtLib.PriorityQuequeV2
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    /// <summary>
    ///     Second implementation of the priority Queue. The first one had some error, gonna rework it later eventually.
    /// </summary>
    /// <typeparam name="TPriority">The type of the priority.</typeparam>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class PriorityQueue<TPriority, TItem>
    {
        #region Fields

        /// <summary>
        ///     The dictionary containing all queques (reference: heap principle)
        /// </summary>
        internal readonly SortedDictionary<TPriority, Queue<TItem>> Dictionary;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the amount of items.
        /// </summary>
        /// <value>
        ///     The count.
        /// </value>
        public int Count
        {
            get
            {
                return this.Dictionary.Sum(q => q.Value.Count);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Clear()
        {
            this.Dictionary.Clear();
        }

        /// <summary>
        ///     Dequeues the item with the highest priority.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">The queue is empty</exception>
        public TItem Dequeue()
        {
            if (this.Dictionary.Any()) return this.DequeueFromHighPriorityQueue();
            else throw new InvalidOperationException("The queue is empty");
        }

        /// <summary>
        ///     Dequeues the item with the highest priority.
        /// </summary>
        /// <param name="count">The amount of items.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">The queue is empty</exception>
        public List<TItem> Dequeue(int count)
        {
            var result = new List<TItem>();

            for (var i = 0; i < count; i++)
                if (this.Dictionary.Any()) result.Add(this.DequeueFromHighPriorityQueue());
                else return null;

            return result;
        }

        /// <summary>
        ///     Enqueues the specified priority.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="item">The item.</param>
        public void Enqueue(TPriority priority, TItem item)
        {
            if (!this.Dictionary.ContainsKey(priority)) this.AddQueueOfPriority(priority);

            this.Dictionary[priority].Enqueue(item);
        }

        /// <summary>
        ///     Returns the value at the begining of the Queue without removing it.
        /// </summary>
        /// <returns>TItem</returns>
        /// <exception cref="InvalidOperationException">The queue is empty</exception>
        public TItem Peek()
        {
            if (this.Dictionary.Any()) return this.Dictionary.First().Value.Peek();
            else throw new InvalidOperationException("The queue is empty");
        }

        #endregion

        #region Methods

        private void AddQueueOfPriority(TPriority priority)
        {
            this.Dictionary.Add(priority, new Queue<TItem>());
        }

        /// <summary>
        ///     Returns the first item of the highest priority queque and removes it.
        /// </summary>
        /// <returns></returns>
        private TItem DequeueFromHighPriorityQueue()
        {
            var first = this.Dictionary.First();
            var nextItem = first.Value.Dequeue();
            if (!first.Value.Any()) this.Dictionary.Remove(first.Key);
            return nextItem;
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PriorityQueue{TPriority, TItem}" /> class.
        /// </summary>
        /// <param name="priorityComparer">The priority comparer.</param>
        public PriorityQueue(IComparer<TPriority> priorityComparer)
        {
            this.Dictionary = new SortedDictionary<TPriority, Queue<TItem>>(priorityComparer);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PriorityQueue{TPriority, TItem}" /> class.
        /// </summary>
        public PriorityQueue()
            : this(Comparer<TPriority>.Default)
        {
            this.Dictionary = new SortedDictionary<TPriority, Queue<TItem>>();
        }

        #endregion
    }
}