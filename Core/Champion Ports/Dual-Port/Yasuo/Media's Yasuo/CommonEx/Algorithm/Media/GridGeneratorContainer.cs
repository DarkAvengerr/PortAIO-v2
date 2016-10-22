using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Algorithm.Media
{
    #region Using Directives

    using Djikstra;

    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.ConnectionTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes;

    #endregion

    internal class GridGeneratorContainer<T, TV> where TV : ConnectionBase<T> where T : PointBase
    {
        #region Fields

        /// <summary>
        ///     The grid
        /// </summary>
        internal Grid<T, ConnectionBase<T>> Grid;

        /// <summary>
        ///     The implementation
        /// </summary>
        internal IGridGenerator<T, TV> Implementation;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        public GridGeneratorContainer(IGridGenerator<T, TV> implementation)
        {
            this.Implementation = implementation;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Generates this instance.
        /// </summary>
        public void Generate()
        {
            this.Grid = this.Implementation.Generate();
        }

        #endregion
    }
}