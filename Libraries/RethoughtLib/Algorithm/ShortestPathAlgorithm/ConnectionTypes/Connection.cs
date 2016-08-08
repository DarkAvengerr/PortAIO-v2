using EloBuddy; namespace RethoughtLib.Algorithm.ShortestPathAlgorithm.ConnectionTypes
{
    /// <summary>
    ///     Base class for Connections
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Connection<T>
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the cost.
        /// </summary>
        /// <value>
        ///     The cost.
        /// </value>
        public abstract float Cost { get; set; }

        /// <summary>
        ///     Gets or sets the end.
        /// </summary>
        /// <value>
        ///     The end.
        /// </value>
        public abstract T End { get; set; }

        /// <summary>
        ///     Gets or sets the start.
        /// </summary>
        /// <value>
        ///     The start.
        /// </value>
        public abstract T Start { get; set; }

        #endregion
    }
}