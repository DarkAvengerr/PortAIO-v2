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
 namespace Rethought_Irelia.IreliaV1.GraphGenerator
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.Algorithm.Graphs;
    using RethoughtLib.Algorithm.Pathfinding.AStar;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Switches;

    using Rethought_Irelia.IreliaV1.Spells;

    using SharpDX;

    using Color = System.Drawing.Color;

    #endregion

    internal class GraphGeneratorModule : ChildBase, IGraphGenerator<AStarNode, AStarEdge<AStarNode>>
    {
        #region Fields

        private Graph<AStarNode, AStarEdge<AStarNode>> graph =
            new Graph<AStarNode, AStarEdge<AStarNode>>(new List<AStarNode>(), new List<AStarEdge<AStarNode>>());

        private IEnumerable<Obj_AI_Base> units = new List<Obj_AI_Base>();

        #endregion

        #region Constructors and Destructors

        #region Constructors

        public GraphGeneratorModule(IreliaQ ireliaQ)
        {
            this.IreliaQ = ireliaQ;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        ///     The spell
        /// </summary>
        public IreliaQ IreliaQ { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "GraphGenerator";

        #endregion

        #region Public Methods and Operators

        #region IGraphGenerator<AStarNode,AStarEdge<AStarNode>> Members

        #region Public Methods and Operators

        /// <summary>
        ///     Generates the specified start to end graph
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        public Graph<AStarNode, AStarEdge<AStarNode>> Generate(AStarNode start, AStarNode end)
        {
            this.graph = new Graph<AStarNode, AStarEdge<AStarNode>>(
                             new List<AStarNode>(),
                             new List<AStarEdge<AStarNode>>());
            this.units =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(
                        x =>
                            !x.IsAlly && !x.IsDead && (x.ServerPosition.Distance(start.Position) <= 2500)
                            && this.IreliaQ.WillReset(x))
                    .ToList();

            var nodes = this.units.Select(unit => new UnitNode(unit.ServerPosition, unit)).Cast<AStarNode>().ToList();

            nodes.Add(start);
            nodes.Add(end);

            foreach (var centerNode in nodes)
            {
                foreach (var neighbor in
                    nodes.Where(
                        x => (centerNode != x) && (x.Position.Distance(centerNode.Position) <= this.IreliaQ.Spell.Range))
                )
                {
                    this.graph.Edges.Add(
                        new AStarEdge<AStarNode>
                            {
                                Start = centerNode,
                                End = neighbor,
                                Cost = centerNode.Position.Distance(neighbor.Position) / this.IreliaQ.Spell.Speed
                            });

                    this.graph.Nodes.Add(neighbor);
                }

                this.graph.Nodes.Add(centerNode);
            }

            this.graph.Start = start;
            this.graph.End = end;

            this.graph.Nodes = nodes;

            return this.graph;
        }

        #endregion

        #endregion

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);
#if DEBUG
            var draw = this.Menu.AddItem(new MenuItem("drawgraph", "Draw (Debugging)").SetValue(false));

            draw.ValueChanged += (o, args) =>
                {
                    if (args.GetNewValue<bool>()) Drawing.OnDraw += this.DrawingOnOnDraw;
                    else Drawing.OnDraw -= this.DrawingOnOnDraw;
                };

            if (draw.GetValue<bool>()) Drawing.OnDraw += this.DrawingOnOnDraw;
#endif
        }

        /// <summary>
        ///     Sets the switch.
        /// </summary>
        protected override void SetSwitch()
        {
            this.Switch = new UnreversibleSwitch(this.Menu);
        }

        private void DrawingOnOnDraw(EventArgs args)
        {
            if (this.graph == null) return;

            foreach (var edge in this.graph.Edges)
                Drawing.DrawLine(
                    Drawing.WorldToScreen(edge.Start.Position),
                    Drawing.WorldToScreen(edge.End.Position),
                    1,
                    Color.White);

            this.graph = null;
        }

        #endregion
    }

    public class UnitNode : AStarNode
    {
        #region Constructors and Destructors

        #region Constructors

        public UnitNode(Vector3 position, Obj_AI_Base unit)
            : base(position)
        {
            this.Unit = unit;
        }

        #endregion

        #endregion

        #region Public Properties

        public Obj_AI_Base Unit { get; set; }

        #endregion
    }
}