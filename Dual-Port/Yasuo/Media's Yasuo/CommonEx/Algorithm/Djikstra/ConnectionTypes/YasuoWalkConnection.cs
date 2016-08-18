using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Algorithm.Djikstra.ConnectionTypes
{
    #region Using Directives

    using global::YasuoMedia.CommonEx.Algorithm.Djikstra.PointTypes;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Dash = Objects.Dash;

    #endregion

    public class WalkConnection : ConnectionBase<Point>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Constructor for a connection
        /// </summary>
        /// <param name="start">start point</param>
        /// <param name="end">end point</param>
        public WalkConnection(Point start, Point end)
        {
            this.Start = start;
            this.End = end;

            this.Cost = start.Position.Distance(end.Position) / GlobalVariables.Player.MoveSpeed;
        }

        #endregion
    }
}