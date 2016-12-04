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

    using LeagueSharp.Common;

    using RethoughtLib.Algorithm.Pathfinding.AStar.Heuristics;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    public class AStarModule<TNode, TEdge> : ChildBase
        where TEdge : AStarEdge<TNode> where TNode : AStarNode
    {
        #region Fields

        private readonly List<IHeuristic> heuristics = new List<IHeuristic>
                                                           {
                                                               new HeuristicEuclidean(),
                                                               new HeuristicManhattan(),
                                                               new HeuristicEuclideanNoSqr(),
                                                               new HeuristicMaxDxdy()
                                                           };

        private AStar<TNode, TEdge> algorithm;

        private float heuristicEstimate;

        private IHeuristic heuristicFormula;

        private bool reopenClosedNodes;

        private bool tieBreaking;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "A-Star (A*) Algorithm";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the path. Returns null if no path found.
        /// </summary>
        /// <param name="edges">The edges.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        public List<TNode> GetPath(List<TEdge> edges, TNode start, TNode end)
        {
            this.algorithm = new AStar<TNode, TEdge>(edges)
                                 {
                                     HeuristicFormula = this.heuristicFormula,
                                     HeuristicEstimate = this.heuristicEstimate,
                                     ReopenCloseNodes = this.reopenClosedNodes,
                                     TieBreaker = this.tieBreaking
                                 };

            return this.algorithm.GetPath(start, end);
        }

        /// <summary>
        ///     Gets the path. Returns null if no path found.
        /// </summary>
        /// <param name="edges">The edges.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="heuristic">The heuristic.</param>
        /// <param name="allowReopenClosedNodes">if set to <c>true</c> [allow reopen closed nodes].</param>
        /// <param name="allowTieBreaking">if set to <c>true</c> [allow tie breaking].</param>
        /// <param name="predefinedHeuristicEstimate">The predefined heuristic estimate.</param>
        /// <returns></returns>
        public List<TNode> GetPath(
            List<TEdge> edges,
            TNode start,
            TNode end,
            IHeuristic heuristic,
            bool allowReopenClosedNodes,
            bool allowTieBreaking,
            float predefinedHeuristicEstimate = 0f)
        {
            this.algorithm = new AStar<TNode, TEdge>(edges)
                                 {
                                     HeuristicFormula = heuristic,
                                     HeuristicEstimate = this.heuristicEstimate,
                                     ReopenCloseNodes = allowReopenClosedNodes,
                                     TieBreaker = allowTieBreaking
                                 };

            if (Math.Abs(predefinedHeuristicEstimate) > 0) this.algorithm.HeuristicEstimate = predefinedHeuristicEstimate;

            return this.algorithm.GetPath(start, end);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        /// <param name="sender">the sender of the input</param>
        /// <param name="eventArgs">the contextual information</param>
        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        /// <param name="sender">the sender of the input</param>
        /// <param name="eventArgs">the contextual information</param>
        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        /// <param name="sender">the sender of the input</param>
        /// <param name="eventArgs">the contextual information</param>
        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            var stringArray = new string[] { };

            var index = 0;

            foreach (var heuristic in this.heuristics)
            {
                index++;
                stringArray[index] = nameof(heuristic);
            }

            this.Menu.AddItem(
                    new MenuItem("HeuristicFormula", "Heuristic Formula: ").SetValue(new StringList(stringArray)))
                .ValueChanged += (o, args) =>
                {
                    var heuristic = args.GetNewValue<StringList>().SelectedValue;
                    this.heuristicFormula = this.heuristics.FirstOrDefault(x => nameof(x) == heuristic);
                };
            var heuristic2 = this.Menu.Item("HeuristicFormula").GetValue<StringList>().SelectedValue;
            this.heuristicFormula = this.heuristics.FirstOrDefault(x => nameof(x) == heuristic2);

            // TODO
            this.Menu.AddItem(new MenuItem("HeuristicEstimate", "Heuristic Estimate: ").SetValue(new Slider(0, 0, 100)))
                .ValueChanged += (o, args) => { this.heuristicEstimate = args.GetNewValue<Slider>().Value; };
            this.heuristicEstimate = this.Menu.Item("HeuristicEstimate").GetValue<Slider>().Value;

            this.Menu.AddItem(new MenuItem("ReopenClosedNodes", "Reopen Closed Nodes").SetValue(false)).ValueChanged +=
                (o, args) => { this.reopenClosedNodes = args.GetNewValue<bool>(); };
            this.reopenClosedNodes = this.Menu.Item("ReopenClosedNodes").GetValue<bool>();

            this.Menu.AddItem(new MenuItem("TieBreaker", "Tie Breaking").SetValue(true)).ValueChanged +=
                (o, args) => { this.tieBreaking = args.GetNewValue<bool>(); };
            this.tieBreaking = this.Menu.Item("TieBreaker").GetValue<bool>();
        }

        #endregion
    }
}