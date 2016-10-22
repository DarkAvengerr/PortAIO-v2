using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Objects.Pathfinding
{
    #region Using Directives

    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.ConnectionTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PathTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes;

    #endregion

    /// <summary>
    ///     Container that initializes and manages pathfinder implementations
    /// </summary>
    internal class PathfindingContainer<T, TV, TP>
        where TV : ConnectionBase<T> where TP : PathBase<T, TV> where T : PointBase
    {
        #region Fields

        /// <summary>
        ///     The pathfinder implementation
        /// </summary>
        private readonly IPathfinder<T, TV, TP> pathfinderImplementation;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PathfindingContainer{T, TV, TP}" /> class.
        /// </summary>
        /// <param name="pathfinderImplementation">The pathfinder implementation.</param>
        public PathfindingContainer(IPathfinder<T, TV, TP> pathfinderImplementation)
        {
            this.pathfinderImplementation = pathfinderImplementation;
            this.pathfinderImplementation.Initialize();
        }

        #endregion

        #region Public Methods and Operators

        public void ExecutePath()
        {
            this.pathfinderImplementation.ExecutePath();
        }

        /// <summary>
        ///     Gets the PathBase.
        /// </summary>
        /// <returns></returns>
        public TP GetPath()
        {
            var path = this.pathfinderImplementation.GeneratePath();

            return path;
        }

        #endregion
    }
}