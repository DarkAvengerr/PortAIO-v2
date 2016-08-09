using EloBuddy; namespace RethoughtLib.Algorithm.ShortestPathAlgorithm.ConnectionTypes
{
    #region Using Directives

    using LeagueSharp.Common;

    using SharpDX;

    #endregion

    public class AutoVector2Connection : Connection<Vector2>
    {
        #region Constructors and Destructors

        public AutoVector2Connection(Vector2 start, Vector2 end)
        {
            this.Start = start;
            this.End = end;
            this.Cost = this.Start.Distance(end);
        }

        #endregion

        #region Public Properties

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
        public sealed override Vector2 End { get; set; }

        /// <summary>
        ///     Gets or sets the start.
        /// </summary>
        /// <value>
        ///     The start.
        /// </value>
        public sealed override Vector2 Start { get; set; }

        #endregion
    }
}