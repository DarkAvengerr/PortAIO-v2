using EloBuddy; namespace RethoughtLib.Algorithm.ShortestPathAlgorithm.ConnectionTypes
{
    #region Using Directives

    using System;

    #endregion

    public class FuncConnection<T> : Connection<T>
    {
        #region Constructors and Destructors

        // TODO
        public FuncConnection(T start, T end, Func<T, T, float> funcCost)
        {
            this.Start = start;
            this.End = end;

            this.Cost = funcCost.Invoke(this.Start, this.End);
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
        public sealed override T End { get; set; }

        /// <summary>
        ///     Gets or sets the start.
        /// </summary>
        /// <value>
        ///     The start.
        /// </value>
        public sealed override T Start { get; set; }

        #endregion
    }
}