using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Algorithm.Djikstra.ConnectionTypes
{
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes;
    /// <summary>
    ///     Base class for Connections
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConnectionBase<T> where T : PointBase
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the cost.
        /// </summary>
        /// <value>
        ///     The cost.
        /// </value>
        public float Cost { get; set; }

        /// <summary>
        ///     Gets or sets the end.
        /// </summary>
        /// <value>
        ///     The end.
        /// </value>
        public T End { get; set; }

        /// <summary>
        ///     Gets or sets the start.
        /// </summary>
        /// <value>
        ///     The start.
        /// </value>
        public T Start { get; set; }

        #endregion
    }
}