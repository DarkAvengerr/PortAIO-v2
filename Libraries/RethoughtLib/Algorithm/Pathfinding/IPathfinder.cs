using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Algorithm.Pathfinding
{
    #region Using Directives

    using System.Collections.Generic;

    #endregion

    public interface IPathfinder<TNode>
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets the Path from start to end
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        List<TNode> GetPath(TNode start, TNode end);

        #endregion
    }
}