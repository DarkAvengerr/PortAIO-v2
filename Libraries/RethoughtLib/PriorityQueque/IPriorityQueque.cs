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