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

        public GraphGeneratorModule(IreliaQ ireliaQ)
        {
            this.IreliaQ = ireliaQ;
        }

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
                        !x.IsAlly && !x.IsDead && x.ServerPosition.Distance(start.Position) <= 2500
                        && this.IreliaQ.WillReset(x))
                    .ToList();

            var nodes = this.units.Select(unit => new UnitNode(unit.ServerPosition, unit)).Cast<AStarNode>().ToList();

            nodes.Add(start);
            nodes.Add(end);

            foreach (var centerNode in nodes)
            {
                foreach (var neighbor in
                    nodes.Where(
                        x => centerNode != x && x.Position.Distance(centerNode.Position) <= this.IreliaQ.Spell.Range))
                {
                    this.graph.Edges.Add(
                        new AStarEdge<AStarNode>()
                            {
                                Start = centerNode,
                                End = neighbor,
                                Cost =
                                    centerNode.Position.Distance(neighbor.Position)
                                    / this.IreliaQ.Spell.Speed
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

        #region Methods

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            var draw = this.Menu.AddItem(new MenuItem("drawgraph", "Draw (Debugging)").SetValue(false));

            draw.ValueChanged += (o, args) =>
                {
                    if (args.GetNewValue<bool>())
                    {
                        Drawing.OnDraw += this.DrawingOnOnDraw;
                        Game.OnUpdate += this.DrawingOnUpdate;
                    }
                    else
                    {
                        Drawing.OnDraw -= this.DrawingOnOnDraw;
                        Game.OnUpdate -= this.DrawingOnUpdate;
                    }
                };

            if (draw.GetValue<bool>())
            {
                Drawing.OnDraw += this.DrawingOnOnDraw;
                Game.OnUpdate -= this.DrawingOnUpdate;
            }
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
            foreach (var edge in this.graph.Edges)
            {
                Drawing.DrawLine(
                    Drawing.WorldToScreen(edge.Start.Position),
                    Drawing.WorldToScreen(edge.End.Position),
                    1,
                    Color.White);
            }
        }

        private void DrawingOnUpdate(EventArgs args)
        {
            this.graph = null;
        }

        #endregion
    }

    public class UnitNode : AStarNode
    {
        #region Constructors and Destructors

        public UnitNode(Vector3 position, Obj_AI_Base unit)
            : base(position)
        {
            this.Unit = unit;
        }

        #endregion

        #region Public Properties

        public Obj_AI_Base Unit { get; set; }

        #endregion
    }
}