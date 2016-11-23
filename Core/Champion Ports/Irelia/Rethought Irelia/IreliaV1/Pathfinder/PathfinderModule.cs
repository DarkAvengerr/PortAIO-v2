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
//     Last Edited: 04.10.2016 1:44 PM

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Rethought_Irelia.IreliaV1.Pathfinder
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.Algorithm.Graphs;
    using RethoughtLib.Algorithm.Pathfinding.AStar;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using Rethought_Irelia.IreliaV1.GraphGenerator;

    #endregion

    internal class PathfinderModule : ChildBase
    {
        #region Constants

        /// <summary>
        ///     The prioritization multiplicand
        /// </summary>
        private const float PrioritizationMultiplicand = 0.5f;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "Pathfinder";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the path.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public List<AStarNode> GetPath(Graph<AStarNode, AStarEdge<AStarNode>> graph, AStarNode from, AStarNode to)
        {
            if (this.Menu.Item("prioritizechampion").GetValue<bool>())
                foreach (var edgeBase in graph.Edges)
                {
                    var node = edgeBase.End as UnitNode;

                    var unitNode = node;

                    if (!(unitNode?.Unit is AIHeroClient)) continue;

                    edgeBase.Cost = edgeBase.Cost *= PrioritizationMultiplicand;
                }

            var edges = graph.Edges.Select(edge => edge).ToList();

            var astar = new AStar<AStarNode, AStarEdge<AStarNode>>(edges);

            return astar.GetPath(new AStarNode(graph.Start.Position), new AStarNode(graph.End.Position));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            this.Menu.AddItem(new MenuItem("prioritizechampion", "Prioritize Champions").SetValue(true));
        }

        #endregion
    }
}