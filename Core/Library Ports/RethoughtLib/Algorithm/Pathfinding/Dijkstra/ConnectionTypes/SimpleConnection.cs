using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Algorithm.Pathfinding.Dijkstra.ConnectionTypes
{
    public class SimpleEdge<T> : EdgeBase<T>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Constructor for a connection
        /// </summary>
        /// <param name="start">start point</param>
        /// <param name="end">end point</param>
        /// <param name="cost">the cost</param>
        public SimpleEdge(T start, T end, float cost)
        {
            this.Start = start;
            this.End = end;
            this.Cost = cost;
        }

        #endregion
    }
}