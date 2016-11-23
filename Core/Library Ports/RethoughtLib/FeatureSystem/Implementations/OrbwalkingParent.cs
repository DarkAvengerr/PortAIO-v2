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
 namespace RethoughtLib.FeatureSystem.Implementations
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    public sealed class OrbwalkingParent : ParentBase
    {
        #region Fields

        /// <summary>
        ///     The orbwalker
        /// </summary>
        private readonly Orbwalking.Orbwalker orbwalker;

        /// <summary>
        ///     The orbwalking mode
        /// </summary>
        private readonly Orbwalking.OrbwalkingMode[] orbwalkingMode;

        #endregion

        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="OrbwalkingParent" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="orbwalker">The orbwalker.</param>
        /// <param name="orbwalkingMode">The orbwalking mode.</param>
        public OrbwalkingParent(
            string name,
            Orbwalking.Orbwalker orbwalker,
            params Orbwalking.OrbwalkingMode[] orbwalkingMode)
        {
            this.Name = name;
            this.orbwalker = orbwalker;

            this.orbwalkingMode = orbwalkingMode;
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
        public override string Name { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        /// <param name="sender">The sender of the input</param>
        /// <param name="eventArgs">the contextual information</param>
        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= this.GameOnOnUpdate;
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        /// <param name="sender">the sender of the input</param>
        /// <param name="eventArgs">the contextual information</param>
        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += this.GameOnOnUpdate;
        }

        /// <summary>
        ///     Triggers on GameUpdate
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void GameOnOnUpdate(EventArgs args)
        {
            if (!this.orbwalkingMode.Any(x => x == this.orbwalker.ActiveMode)) foreach (var keyValuePair in this.Children.Where(x => x.Value.Item1)) keyValuePair.Key.Switch.InternalDisable(new FeatureBaseEventArgs(this));
            else foreach (var child in this.Children.Where(x => x.Value.Item1)) child.Key.Switch.InternalEnable(new FeatureBaseEventArgs(this));
        }

        #endregion
    }
}