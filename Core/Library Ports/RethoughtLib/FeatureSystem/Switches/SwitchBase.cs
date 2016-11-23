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
 namespace RethoughtLib.FeatureSystem.Switches
{
    #region Using Directives

    using System;

    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    public abstract class SwitchBase
    {
        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SwitchBase" /> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        protected SwitchBase(Menu menu)
        {
            this.Menu = menu;
        }

        #endregion

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when [on disable event].
        /// </summary>
        public event EventHandler<Base.FeatureBaseEventArgs> OnDisableEvent;

        /// <summary>
        ///     Occurs when [on enable event].
        /// </summary>
        public event EventHandler<Base.FeatureBaseEventArgs> OnEnableEvent;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Base" /> is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; set; }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Raises the <see cref="E:OnDisableEvent" /> event.
        /// </summary>
        /// <param name="e">The <see cref="Base.FeatureBaseEventArgs" /> instance containing the event data.</param>
        public virtual void Disable(Base.FeatureBaseEventArgs e)
        {
            if (!this.Enabled) return;

            this.Enabled = false;

            this.OnDisableEvent?.Invoke(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="E:OnEnableEvent" /> event.
        /// </summary>
        /// <param name="e">The <see cref="Base.FeatureBaseEventArgs" /> instance containing the event data.</param>
        public virtual void Enable(Base.FeatureBaseEventArgs e)
        {
            if (this.Enabled) return;

            this.Enabled = true;

            this.OnEnableEvent?.Invoke(this, e);
        }

        /// <summary>
        ///     Bypasses the Disable method and directly invokes the OnDisableEvent
        /// </summary>
        /// <param name="e">The <see cref="Base.FeatureBaseEventArgs" /> instance containing the event data.</param>
        public void InternalDisable(Base.FeatureBaseEventArgs e)
        {
            if (!this.Enabled) return;

            this.Enabled = false;

            this.OnDisableEvent?.Invoke(this, e);
        }

        /// <summary>
        ///     Bypasses the Enable method and directly invokes the OnEnableEvent
        /// </summary>
        /// <param name="e">The <see cref="Base.FeatureBaseEventArgs" /> instance containing the event data.</param>
        public void InternalEnable(Base.FeatureBaseEventArgs e)
        {
            if (this.Enabled) return;

            this.Enabled = true;

            this.OnEnableEvent?.Invoke(this, e);
        }

        /// <summary>
        ///     Setups this instance.
        /// </summary>
        public abstract void Setup();

        /// <summary>
        ///     Setups the specified menu.
        /// </summary>
        /// <param name="menu">The menu.</param>
        public void Setup(Menu menu)
        {
            this.Menu = menu;

            this.Setup();
        }

        #endregion
    }
}