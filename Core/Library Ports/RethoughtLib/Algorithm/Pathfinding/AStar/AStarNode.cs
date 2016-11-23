//     Copyright (C) 2016 Rethought
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
//     Created: 04.10.2016 1:05 PM
//     Last Edited: 04.10.2016 1:43 PM

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

        #region Constructors

        public AStarNode(Vector3 position)
            : base(position)
        {
        }

        #endregion

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

        #region IComparer<AStarNode> Members

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
            if (x.F > y.F) return 1;

            // x less than y
            else if (x.F < y.F) return -1;

            // x equal to y
            return 0;
        }

        #endregion

        #endregion

        #endregion
    }
}