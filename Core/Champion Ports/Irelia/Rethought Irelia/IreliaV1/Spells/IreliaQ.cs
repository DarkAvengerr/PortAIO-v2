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
 namespace Rethought_Irelia.IreliaV1.Spells
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.Algorithm.Graphs;
    using RethoughtLib.Algorithm.Pathfinding.AStar;
    using RethoughtLib.DamageCalculator;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using Rethought_Irelia.IreliaV1.GraphGenerator;
    using Rethought_Irelia.IreliaV1.Pathfinder;

    using SharpDX;

    #endregion

    /// <summary>
    ///     The irelia q.
    /// </summary>
    internal class IreliaQ : SpellChild, IDamageCalculatorModule
    {
        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="IreliaQ" /> class.
        /// </summary>
        /// <param name="graphGenerator">
        ///     The graph generator.
        /// </param>
        /// <param name="pathfinderModule">
        ///     The pathfinder module.
        /// </param>
        public IreliaQ(
            IGraphGenerator<AStarNode, AStarEdge<AStarNode>> graphGenerator,
            PathfinderModule pathfinderModule)
        {
            this.GraphGenerator = graphGenerator;
            this.PathfinderModule = pathfinderModule;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the estimated amount in one combo.
        /// </summary>
        public int EstimatedAmountInOneCombo { get; set; } = 1;

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
        /// <param name="target">
        ///     The get damage.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public float GetDamage(Obj_AI_Base target)
        {
            if (!this.Spell.IsReady()) return 0;

            var damage = this.Spell.GetDamage(target);

            if (this.Menu.Item(this.Path + ".sheendamage").GetValue<bool>()) damage += this.GetSheenDamage(target);

            return damage;
        }

        /// <summary>
        ///     Gets the path.
        /// </summary>
        /// <param name="from">
        ///     From.
        /// </param>
        /// <param name="to">
        ///     To.
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        public List<Obj_AI_Base> GetPath(Vector3 from, Vector3 to)
        {
            var graph = this.GraphGenerator.Generate(new AStarNode(from), new AStarNode(to));

            var path = this.PathfinderModule.GetPath(graph, graph.Start, graph.End);

            return path?.OfType<UnitNode>().Select(x => x.Unit).ToList();
        }

        /// <summary>
        ///     Custom Made GetSheenDamage function -&gt; Including all Sheen buildable items.
        /// </summary>
        /// <param name="target">
        /// </param>
        /// <remarks>
        ///     SupportExtraGoz
        /// </remarks>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public float GetSheenDamage(Obj_AI_Base target)
        {
            float totalDamage = 0;

            if (Items.HasItem(3057, ObjectManager.Player))
            {
                if (Items.CanUseItem(3057))
                    totalDamage +=
                        (float)
                        ObjectManager.Player.CalcDamage(
                            target,
                            Damage.DamageType.Physical,
                            ObjectManager.Player.TotalAttackDamage);
            }
            else if (Items.HasItem(3025, ObjectManager.Player))
            {
                if (Items.CanUseItem(3025))
                    totalDamage +=
                        (float)
                        ObjectManager.Player.CalcDamage(
                            target,
                            Damage.DamageType.Physical,
                            ObjectManager.Player.TotalAttackDamage);
            }
            else if (Items.HasItem(3078, ObjectManager.Player))
            {
                if (Items.CanUseItem(3078))
                    totalDamage +=
                        (float)
                        ObjectManager.Player.CalcDamage(
                            target,
                            Damage.DamageType.Physical,
                            ObjectManager.Player.TotalAttackDamage * 2);
            }

            return totalDamage;
        }

        /// <summary>
        ///     Whether this instance will reset the spell on the specified target
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
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
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="eventArgs">
        ///     The event Args.
        /// </param>
        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            this.Spell = new Spell(SpellSlot.Q, 650);
            this.Spell.SetTargetted(0f, 500);

            this.Menu.AddItem(
                new MenuItem(this.Path + ".sheendamage", "Calculate Sheen-Items in Damage Calculations").SetValue(true));
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