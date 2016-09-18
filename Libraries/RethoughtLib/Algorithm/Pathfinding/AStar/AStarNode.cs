using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Algorithm.Pathfinding.AStar
{
    #region Using Directives

    using System.Collections.Generic;

    using SharpDX;

    #endregion

    public class AStarNode : NodeBase
    {
        #region Constructors and Destructors

        public AStarNode(Vector3 position)
            : base(position)
        {
        }

        #endregion

        #region Public Properties

        public float F { get; set; } = 0;

        public float G { get; set; } = 0;

        public float H { get; set; } = 0;

        // TODO: Optimize through minimizing data size
        public NodeBase ParentNode { get; set; }

        #endregion
    }

    public class AStarNodeComparer : IComparer<AStarNode>
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the
        ///     other.
        /// </summary>
        /// <returns>
        ///     A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as
        ///     shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />
        ///     .Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than
        ///     <paramref name="y" />.
        /// </returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public int Compare(AStarNode x, AStarNode y)
        {
            // x more than y
            if (x.F > y.F)
            {
                return 1;
            }

            // x less than y
            else if (x.F < y.F)
            {
                return -1;
            }

            // x equal to y
            return 0;
        }

        #endregion
    }
}