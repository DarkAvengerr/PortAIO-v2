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
    internal class DefaultPriorityQueque<T> : IPriorityQueque<T>
    {
        #region Fields

        /// <summary>
        ///     The capacity
        /// </summary>
        protected int capacity = 0;

        /// <summary>
        ///     The items
        /// </summary>
        internal readonly SortedList<T, int> Items;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultPriorityQueque{T}" /> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public DefaultPriorityQueque(int capacity)
        {
            this.Capacity = capacity;

            this.Items = new SortedList<T, int>(capacity);
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