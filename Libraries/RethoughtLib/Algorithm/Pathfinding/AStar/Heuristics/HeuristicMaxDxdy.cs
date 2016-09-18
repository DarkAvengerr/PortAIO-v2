using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Algorithm.Pathfinding.AStar.Heuristics
{
    #region Using Directives

    using System;

    #endregion

    public class HeuristicMaxDxdy : IHeuristic
    {
        #region Public Methods and Operators

        public float Result(NodeBase node1, NodeBase node2)
        {
            return Math.Max(
                Math.Abs(node1.Position.X - node2.Position.X),
                Math.Abs(node1.Position.Y - node2.Position.Y));
        }

        #endregion
    }
}