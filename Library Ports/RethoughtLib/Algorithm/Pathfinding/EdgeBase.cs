using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Algorithm.Pathfinding
{
    public class EdgeBase<TNode>
    {
        #region Public Properties

        public float Cost { get; set; }

        public TNode End { get; set; }

        public TNode Start { get; set; }

        #endregion
    }
}