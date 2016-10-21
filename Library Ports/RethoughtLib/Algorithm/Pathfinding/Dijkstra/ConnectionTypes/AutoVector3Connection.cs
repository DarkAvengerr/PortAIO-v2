using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Algorithm.Pathfinding.Dijkstra.ConnectionTypes
{
    #region Using Directives

    using LeagueSharp.Common;

    using SharpDX;

    #endregion

    public class AutoVector3Edge : EdgeBase<Vector2>
    {
        #region Constructors and Destructors

        public AutoVector3Edge(Vector2 start, Vector2 end)
        {
            this.Start = start;
            this.End = end;
            this.Cost = this.Start.Distance(end);
        }

        #endregion
    }
}