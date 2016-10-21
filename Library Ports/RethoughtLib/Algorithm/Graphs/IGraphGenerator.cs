using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Algorithm.Graphs
{
    using RethoughtLib.Algorithm.Pathfinding;

    public interface IGraphGenerator<TNode, TEdge>
        where TEdge : EdgeBase<TNode> where TNode : NodeBase
    {
        #region Public Methods and Operators

        /// <summary>
        /// Generates the specified start to end graph
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        Graph<TNode, TEdge> Generate(TNode start, TNode end);

        #endregion
    }
}