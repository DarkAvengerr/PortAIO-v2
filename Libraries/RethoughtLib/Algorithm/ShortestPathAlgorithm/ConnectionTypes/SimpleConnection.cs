using EloBuddy; namespace RethoughtLib.Algorithm.ShortestPathAlgorithm.ConnectionTypes
{
    public class SimpleConnection<T> : Connection<T>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Constructor for a connection
        /// </summary>
        /// <param name="start">start point</param>
        /// <param name="end">end point</param>
        /// <param name="cost">the cost</param>
        public SimpleConnection(T start, T end, float cost)
        {
            this.Start = start;
            this.End = end;
            this.Cost = cost;
        }

        #endregion

        /// <summary>
        ///     Gets or sets the cost.
        /// </summary>
        /// <value>
        ///     The cost.
        /// </value>
        public sealed override float Cost { get; set; }

        /// <summary>
        ///     Gets or sets the end.
        /// </summary>
        /// <value>
        ///     The end.
        /// </value>
        public sealed override T End { get; set; }

        /// <summary>
        ///     Gets or sets the start.
        /// </summary>
        /// <value>
        ///     The start.
        /// </value>
        public sealed override T Start { get; set; }
    }
}