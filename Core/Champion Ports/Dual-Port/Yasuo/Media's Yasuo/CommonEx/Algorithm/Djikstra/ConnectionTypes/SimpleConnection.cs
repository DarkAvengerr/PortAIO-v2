using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Algorithm.Djikstra.ConnectionTypes
{
    #region Using Directives

    using PointTypes;

    #endregion

    public class SimpleConnection<T> : ConnectionBase<T>
        where T : PointBase
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Constructor for a connection
        /// </summary>
        /// <param name="start">start point</param>
        /// <param name="end">end point</param>
        /// <param name="cost">the cost</param>
        public SimpleConnection(T start, T end, float cost)
        {
            this.Start = start;
            this.End = end;
            this.Cost = cost;
        }

        #endregion
    }
}