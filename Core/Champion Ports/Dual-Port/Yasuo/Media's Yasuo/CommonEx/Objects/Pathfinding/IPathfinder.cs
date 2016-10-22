using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Objects.Pathfinding
{
    #region Using Directives

    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.ConnectionTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PathTypes;
    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes;

    #endregion

    public interface IPathfinder<T, TV, out TP>
        where TV : ConnectionBase<T>
        where TP : PathBase<T, TV>
        where T : PointBase
    {
        #region Public Methods and Operators

        void ExecutePath();

        /// <summary>
        ///     Gets the PathBase.
        /// </summary>
        /// <returns></returns>
        TP GeneratePath();

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        void Initialize();

        #endregion
    }
}