using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Algorithm.Pathfinding.AStar.Heuristics
{
    public interface IHeuristic
    {
        #region Public Methods and Operators

        float Result(NodeBase node1, NodeBase node2);

        #endregion
    }
}