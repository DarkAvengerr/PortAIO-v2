using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Algorithm.Pathfinding.Dijkstra.ConnectionTypes
{
    #region Using Directives

    using System;

    #endregion

    public class FuncEdge<T> : EdgeBase<T>
    {
        #region Constructors and Destructors

        public FuncEdge(T start, T end, Func<T, T, float> funcCost)
        {
            this.Start = start;
            this.End = end;

            this.Cost = funcCost.Invoke(this.Start, this.End);
        }

        #endregion
    }
}