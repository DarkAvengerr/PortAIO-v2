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
namespace RethoughtLib.Orbwalker.Implementations
{
    #region Using Directives

    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    /// <summary>
    ///     A simple FeatureSystem implementation containing an instance of an Orbwalker.
    /// </summary>
    /// <seealso cref="RethoughtLib.FeatureSystem.Abstract_Classes.Base" />
    public class OrbwalkerModule : Base
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = nameof(Orbwalker);

        /// <summary>
        ///     Gets or sets the orbwalker instance.
        /// </summary>
        /// <value>
        ///     The orbwalker instance.
        /// </value>
        public Orbwalking.Orbwalker OrbwalkerInstance { get; set; }

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

            this.OrbwalkerInstance.SetAttack(false);
            this.OrbwalkerInstance.SetMovement(false);
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        /// <param name="sender">the sender of the input</param>
        /// <param name="eventArgs">the contextual information</param>
        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            this.OrbwalkerInstance.SetAttack(true);
            this.OrbwalkerInstance.SetMovement(true);
        }

        /// <summary>
        ///     Sets the menu
        /// </summary>
        protected override void SetMenu()
        {
            this.Menu = new Menu(this.Name, this.Name);

            this.OrbwalkerInstance = new Orbwalking.Orbwalker(this.Menu);

            this.Menu.DisplayName = this.Name;
        }

        #endregion
    }
}