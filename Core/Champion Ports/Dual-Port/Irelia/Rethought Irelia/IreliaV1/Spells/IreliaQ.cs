using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Rethought_Irelia.IreliaV1.Spells
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.Algorithm.Graphs;
    using RethoughtLib.Algorithm.Pathfinding.AStar;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using Rethought_Irelia.IreliaV1.DamageCalculator;
    using Rethought_Irelia.IreliaV1.GraphGenerator;
    using Rethought_Irelia.IreliaV1.Pathfinder;

    using SharpDX;

    #endregion

    internal class IreliaQ : SpellChild, IDamageCalculatorModule
    {
        #region Constructors and Destructors

        public IreliaQ(
            IGraphGenerator<AStarNode, AStarEdge<AStarNode>> graphGenerator,
            PathfinderModule pathfinderModule)
        {
            this.GraphGenerator = graphGenerator;
            this.PathfinderModule = pathfinderModule;
        }

        #endregion

        #region Public Properties

        public int EstimatedAmountInOneCombo { get; } = 1;

        /// <summary>
        ///     Gets or sets the graph generator.
        /// </summary>
        /// <value>
        ///     The graph generator.
        /// </value>
        public IGraphGenerator<AStarNode, AStarEdge<AStarNode>> GraphGenerator { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "Bladesurge";

        /// <summary>
        ///     Gets or sets the pathfinder module.
        /// </summary>
        /// <value>
        ///     The pathfinder module.
        /// </value>
        public PathfinderModule PathfinderModule { get; set; }

        /// <summary>
        ///     Gets or sets the spell.
        /// </summary>
        /// <value>
        ///     The spell.
        /// </value>
        public override Spell Spell { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the damage.
        /// </summary>
        /// <param name="target">The get damage.</param>
        /// <returns></returns>
        public float GetDamage(Obj_AI_Base target)
        {
            return !this.Spell.IsReady() ? 0 : this.Spell.GetDamage(target);
        }

        /// <summary>
        ///     Gets the path.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public List<Obj_AI_Base> GetPath(Vector3 from, Vector3 to)
        {
            var graph = this.GraphGenerator.Generate(new AStarNode(from), new AStarNode(to));

            var path = this.PathfinderModule.GetPath(graph, graph.Start, graph.End);

            return path?.OfType<UnitNode>().Select(x => x.Unit).ToList();
        }

        /// <summary>
        ///     Whether this instance will reset the spell on the specified target
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public bool WillReset(Obj_AI_Base target)
        {
            var predictedTargetHealth = HealthPrediction.GetHealthPrediction(
                target,
                (int)
                (this.Spell.Delay
                 + target.ServerPosition.Distance(ObjectManager.Player.ServerPosition) / this.Spell.Speed));

            return predictedTargetHealth <= this.GetDamage(target);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            this.Spell = new Spell(SpellSlot.Q, 650);
            this.Spell.SetTargetted(0f, 500);
        }

        /// <summary>
        ///     Sets the switch.
        /// </summary>
        protected override void SetSwitch()
        {
            this.Switch = new UnreversibleSwitch(this.Menu);
        }

        #endregion
    }
}