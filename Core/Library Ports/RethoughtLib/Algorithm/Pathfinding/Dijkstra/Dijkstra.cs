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
namespace RethoughtLib.Algorithm.Pathfinding.Dijkstra
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.Common;

    #endregion

    /// <summary>
    ///     Generic Implementation of the Dijkstra (A shortest path / sailman problem) algorithm.
    /// </summary>
    /// <typeparam name="T">Node</typeparam>
    /// <typeparam name="TV">Edge</typeparam>
    public class Dijkstra<T, TV>
        where TV : EdgeBase<T>
    {
        #region Fields

        public List<T> Base = new List<T>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the connections.
        /// </summary>
        /// <value>
        ///     The connections.
        /// </value>
        public List<TV> Connections { get; set; } = new List<TV>();

        /// <summary>
        ///     Gets or sets the costs.
        /// </summary>
        /// <value>
        ///     The costs.
        /// </value>
        public Dictionary<T, float> Costs { get; set; } = new Dictionary<T, float>();

        /// <summary>
        ///     Gets or sets the previous node.
        /// </summary>
        /// <value>
        ///     The previous.
        /// </value>
        public Dictionary<T, T> Previous { get; set; } = new Dictionary<T, T>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns the distance between 2 nodes
        /// </summary>
        /// <param name="node1">Start</param>
        /// <param name="node2">End</param>
        /// <returns></returns>
        public float GetCostBetween(T node1, T node2)
        {
            return
                this.Connections.Where(connection => connection.Start.Equals(node1) && connection.End.Equals(node2))
                    .Select(connection => connection.Cost)
                    .FirstOrDefault();
        }

        /// <summary>
        ///     Gives all neighbors that are still in the base
        /// </summary>
        /// <param name="point">Points</param>
        /// <returns></returns>
        public List<T> GetNeighbors(T point)
        {
            var neighbors = new List<T>();

            foreach (var connection in this.Connections) if (connection.Start.Equals(point) && this.Base.Contains(point) && this.Base.Contains(connection.End)) neighbors.Add(connection.End);

            return neighbors;
        }

        /// <summary>
        ///     Gets the node with the least cost
        /// </summary>
        /// <returns></returns>
        public T GetNodeLeastCost()
        {
            return this.Base.MinOrDefault(node => this.Costs[node]);

            //var speed = float.MaxValue;

            //var smallest = default(T);

            //foreach (var n in this.Base)
            //{
            //    if (!(this.Costs[n] < speed)) continue;

            //    speed = this.Costs[n];
            //    smallest = n;
            //}

            //return smallest;
        }

        /// <summary>
        ///     Calculates the Node path to the specified Node
        /// </summary>
        /// <param name="node">Targeted Node</param>
        /// <returns>node Path</returns>
        public List<T> GetNodesTo(T node)
        {
            var path = new List<T>();

            path.Insert(0, node);

            while (this.Previous.ContainsKey(node) && (this.Previous[node] != null))
            {
                node = this.Previous[node];
                path.Insert(0, node);
            }

            return path;
        }

        /// <summary>
        ///     Calculates the shortest distance from the Start Node to all other Points
        /// </summary>
        /// <param name="start">Startnode</param>
        public void SetStart(T start)
        {
            this.Costs[start] = 0;

            // while we have points to process
            while (this.Base.Count > 0)
            {
                var point = this.GetNodeLeastCost();

                if (point == null)
                {
                    Console.WriteLine("LEast Cost null");
                    this.Base.Clear();
                }

                else
                {
                    foreach (var neighbor in this.GetNeighbors(point))
                    {
                        var tempCost = this.Costs[point] + this.GetCostBetween(point, neighbor);

                        if (tempCost > this.Costs[neighbor]) continue;

                        this.Costs[neighbor] = tempCost;
                        this.Previous[neighbor] = point;
                    }
                    this.Base.Remove(point);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Dijkstra{T,TV}" /> class.
        /// </summary>
        /// <param name="tvlist">The list of edges.</param>
        public Dijkstra(List<TV> tvlist)
        {
            try
            {
                this.Connections = tvlist;

                foreach (var connection in this.Connections)
                {
                    if (!this.Base.Contains(connection.Start)) this.Base.Add(connection.Start);

                    if (!this.Base.Contains(connection.End)) this.Base.Add(connection.End);
                }

                foreach (var point in this.Base.Where(x => !this.Costs.ContainsKey(x)))
                {
                    this.Costs.Add(point, float.MaxValue);
                    this.Previous.Add(point, default(T));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Dijkstra{T,TV}" /> class.
        /// </summary>
        /// <param name="tlist">The tlist.</param>
        /// <param name="tvlist">The tvlist.</param>
        public Dijkstra(List<T> tlist, List<TV> tvlist)
            : this(tvlist)
        {
            try
            {
                this.Base = tlist;
                this.Connections = tvlist;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion
    }
}