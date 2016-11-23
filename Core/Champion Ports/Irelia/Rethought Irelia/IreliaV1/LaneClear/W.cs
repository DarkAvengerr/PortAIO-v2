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
 namespace Rethought_Irelia.IreliaV1.LaneClear
{
    #region Using Directives

    using System.Linq;

    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;

    using Rethought_Irelia.IreliaV1.Spells;

    #endregion

    internal class W : OrbwalkingChild
    {
        #region Fields

        /// <summary>
        ///     The irelia w
        /// </summary>
        private readonly IreliaW ireliaW;

        #endregion

        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="W" /> class.
        /// </summary>
        /// <param name="ireliaW">The irelia w.</param>
        public W(IreliaW ireliaW)
        {
            this.ireliaW = ireliaW;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "W";

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Orbwalking.BeforeAttack -= this.OrbwalkingOnBeforeAttack;
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Orbwalking.BeforeAttack += this.OrbwalkingOnBeforeAttack;
        }

        /// <summary>
        ///     Triggers on before attack
        /// </summary>
        /// <param name="args">The <see cref="Orbwalking.BeforeAttackEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OrbwalkingOnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!this.CheckGuardians()) return;

            if (MinionManager.GetMinions(1000, MinionTypes.All, MinionTeam.NotAlly).Count > 3) this.ireliaW.Spell.Cast();

            if (MinionManager.GetMinions(400, MinionTypes.All, MinionTeam.Neutral).Any(x => x.HealthPercent > 20)) this.ireliaW.Spell.Cast();
        }

        #endregion
    }
}