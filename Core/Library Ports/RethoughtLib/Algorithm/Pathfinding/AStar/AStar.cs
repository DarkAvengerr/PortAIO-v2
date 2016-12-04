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

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using RethoughtLib.Algorithm.Pathfinding.AStar.Heuristics;
    using RethoughtLib.PriorityQuequeV2;

    #endregion

    public class AStar<TNode, TEdge> : IPathfinder<TNode>
        where TNode : AStarNode where TEdge : AStarEdge<TNode>
    {
        #region Fields

        /// <summary>
        ///     The closed nodes
        /// </summary>
        private readonly List<TNode> closedNodes = new List<TNode>();

        /// <summary>
        ///     The edges
        /// </summary>
        private readonly List<TEdge> edges;

        /// <summary>
        ///     The open nodes
        /// </summary>
        private readonly PriorityQueue<float, TNode> openNodes = new PriorityQueue<float, TNode>();

        #endregion

        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AStar" /> class.
        /// </summary>
        /// <param name="edges">The edges representing a graph.</param>
        public AStar(List<TEdge> edges)
        {
            this.edges = edges;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="AStar" /> is finished.
        /// </summary>
        /// <value>
        ///     <c>true</c> if finished; otherwise, <c>false</c>.
        /// </value>
        public bool Finished { get; set; } = true;

        /// <summary>
        ///     The heuristic estimate
        /// </summary>
        public float HeuristicEstimate { get; set; } = 0f;

        /// <summary>
        ///     Gets or sets the heuristic formula.
        /// </summary>
        /// <value>
        ///     The heuristic formula.
        /// </value>
        public IHeuristic HeuristicFormula { get; set; } = new HeuristicManhattan();

        /// <summary>
        ///     The reopen close nodes bool
        /// </summary>
        public bool ReopenCloseNodes { get; set; } = false;

        public bool TieBreaker { get; set; }

        #endregion

        #region Public Methods and Operators

        #region IPathfinder<TNode> Members

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the Path from start to end
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        public List<TNode> GetPath(TNode start, TNode end)
        {
            this.Finished = false;

            this.openNodes.Clear();
            this.closedNodes.Clear();

            start.H = this.HeuristicEstimate;
            start.G = 0;
            start.F = start.G + start.H;

            this.openNodes.Enqueue(start.F, start);

            while (this.openNodes.Count > 0)
            {
                var parentNode = this.openNodes.Dequeue();

                // Possible Path Found
                if (parentNode.Position == end.Position)
                {
                    this.closedNodes.Add(parentNode);
                    this.Finished = true;
                    break;
                }

                // Foreach Connection
                foreach (var edge in
                    this.edges.Where(
                        x =>
                            (x.Start == parentNode) || x.Start.Equals(parentNode)
                            || x.Start.Position.Equals(parentNode.Position)))
                {
                    var newNode = edge.End;

                    newNode.G = edge.Cost;

                    if (Math.Abs(newNode.G - parentNode.G) < 0.1f) continue;

                    var iOpen = -1;

                    AStarNode resultNodeOpen = null;

                    foreach (var queque in this.openNodes.Dictionary.Values)
                        foreach (var node in queque.Reverse())
                        {
                            if (node.Position != newNode.Position) continue;
                            resultNodeOpen = node;

                            iOpen++;

                            break;
                        }

                    if ((resultNodeOpen != null) && (iOpen != -1) && (resultNodeOpen.G <= newNode.G)) continue;

                    var iClosed = -1;

                    AStarNode resultNodeClosed = null;

                    foreach (var node in this.closedNodes)
                    {
                        iClosed++;

                        if (node.Position != newNode.Position) continue;
                        resultNodeClosed = node;
                        break;
                    }

                    if ((resultNodeClosed != null)
                        && (this.ReopenCloseNodes || ((iClosed != -1) && (resultNodeClosed.G <= newNode.G)))) continue;

                    newNode.ParentNode = parentNode;

                    newNode.H = this.HeuristicEstimate * this.HeuristicFormula.Result(newNode, end);

                    if (this.TieBreaker)
                    {
                        var dx1 = parentNode.Position.X - end.Position.X;
                        var dy1 = parentNode.Position.Y - end.Position.Y;
                        var dx2 = start.Position.X - end.Position.X;
                        var dy2 = start.Position.Y - end.Position.Y;
                        var cross = Math.Abs(dx1 * dy2 - dx2 * dy1);
                        newNode.H = newNode.H + cross * 0.001f;
                    }

                    newNode.F = newNode.G + newNode.H;

                    this.openNodes.Enqueue(newNode.F, newNode);
                }

                this.closedNodes.Add(parentNode);
            }

            if (!this.Finished) return null;

            var fNode = this.closedNodes[this.closedNodes.Count - 1];

            for (var i = this.closedNodes.Count - 1; i >= 0; i--)
            {
                var node = this.closedNodes[i];
                var parent = fNode.ParentNode;

                if ((node == null) || (parent == null)) continue;

                if (((Math.Abs(parent.Position.X - node.Position.X) < 0.1f)
                     && (Math.Abs(parent.Position.Y - node.Position.Y) < 0.1f)) || (i == this.closedNodes.Count - 1)) fNode = this.closedNodes[i];
                else this.closedNodes.RemoveAt(i);
            }

            return this.closedNodes;
        }

        #endregion

        #endregion

        #endregion
    }
}