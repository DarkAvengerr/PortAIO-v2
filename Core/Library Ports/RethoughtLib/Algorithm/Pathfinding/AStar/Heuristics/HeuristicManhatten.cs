using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Algorithm.Pathfinding.AStar.Heuristics
{
    #region Using Directives

    using System;

    #endregion

    public class HeuristicManhattan : IHeuristic
    {
        #region Public Methods and Operators

        public float Result(NodeBase node1, NodeBase node2)
        {
            var result = Math.Abs(node1.Position.X - node2.Position.X) + Math.Abs(node1.Position.Y - node2.Position.Y);

            return result;
        }

        #endregion
    }
}