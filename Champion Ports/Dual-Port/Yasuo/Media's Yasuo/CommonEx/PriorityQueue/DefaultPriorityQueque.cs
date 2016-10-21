using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.PriorityQueue
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    #endregion

    /// <summary>
    ///     Default implementation of a PriorityQueque using a SortedList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class DefaultPriorityQueque<T> : IPriorityQueque<T>
    {
        #region Fields

        /// <summary>
        ///     The capacity
        /// </summary>
        protected int capacity = 0;

        /// <summary>
        ///     The items
        /// </summary>
        internal readonly SortedList<T, int> items;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultPriorityQueque{T}" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public DefaultPriorityQueque(int capacity)
        {
            this.Capacity = capacity;

            this.items = new SortedList<T, int>(capacity);
        }

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
                if (value > 0)
                {
                    this.capacity = value;
                }
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
            this.items.Remove(item);
            this.items.Add(item, priority);
        }

        /// <summary>
        ///     Removes all T.
        /// </summary>
        public void Clear()
        {
            this.items.Clear();
        }

        /// <summary>
        ///     Determines whether the list contains specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool ContainsItem(T item) => this.items.Any(x => x.Key.Equals(item));

        /// <summary>
        ///     Determines whether the list contains specified priority.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <returns></returns>
        public bool ContainsPriority(int priority) => this.items.Any(x => x.Value == priority);

        /// <summary>
        ///     Returns the next T to be dequeded/processed.
        /// </summary>
        /// <returns></returns>
        public T Dequeque()
        {
            var keyvalue = this.items.FirstOrDefault();

            return keyvalue.Key;
        }

        /// <summary>
        ///     Enqueques the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="priority">The priority.</param>
        public void Enqueque(T item, int priority)
        {
            this.items.Add(item, priority);
        }

        #endregion
    }
}