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
 namespace RethoughtLib.Algorithm.Graphs
{
    #region Using Directives

    using System.Collections.Generic;

    using RethoughtLib.Algorithm.Pathfinding;

    #endregion

    public class Graph<TNode, TEdge>
        where TNode : NodeBase where TEdge : EdgeBase<TNode>
    {
        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Graph{TNode, TEdge}" /> class.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="edges">The edges.</param>
        public Graph(List<TNode> nodes, List<TEdge> edges)
        {
            this.Edges = edges;
            this.Nodes = nodes;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the edges.
        /// </summary>
        /// <value>
        ///     The edges.
        /// </value>
        public List<TEdge> Edges { get; set; }

        public TNode End { get; set; }

        /// <summary>
        ///     Gets or sets the nodes.
        /// </summary>
        /// <value>
        ///     The nodes.
        /// </value>
        public List<TNode> Nodes { get; set; }

        public TNode Start { get; set; }

        #endregion
    }
}