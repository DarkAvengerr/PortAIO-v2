//     File:  Rethought Series/RethoughtLib/Base.cs
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
//     Last Edited: 04.10.2016 3:38 PM

using EloBuddy; 
using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Abstract_Classes
{
    #region Using Directives

    using LeagueSharp.Common;
    using RethoughtLib.FeatureSystem.Switches;
    using RethoughtLib.Menu;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    /// <summary>
    ///     Base class of Parent-Child System
    /// </summary>
    public abstract class Base
    {
        #region Public Events

        /// <summary>
        ///     Occurs when [on load event].
        /// </summary>
        public event EventHandler<FeatureBaseEventArgs> OnLoadEvent;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="Base" /> is initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if initialized; otherwise, <c>false</c>.
        /// </value>
        public bool Initialized { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Base" /> is loaded.
        /// </summary>
        /// <value>
        ///     <c>true</c> if loaded; otherwise, <c>false</c>.
        /// </value>
        public bool Loaded { get; private set; }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public abstract string Name { get; set; }

        /// <summary>
        ///     Gets or sets the path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     Gets or sets the switch.
        /// </summary>
        /// <value>
        ///     The switch.
        /// </value>
        public SwitchBase Switch { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Disables the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void Disable(Base sender = null)
        {
            if (sender == null) sender = this;

            this.Switch.Disable(new FeatureBaseEventArgs(sender));
        }

        /// <summary>
        ///     Enables the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void Enable(Base sender = null)
        {
            if (sender == null) sender = this;

            this.Switch.Enable(new FeatureBaseEventArgs(sender));
        }

        /// <summary>
        ///     "Hides" this instance
        /// </summary>
        /// <param name="hideSubMenus">if set to <c>true</c> [hide sub menus].</param>
        public void Hide(bool hideSubMenus = true)
        {
            this.Menu.DisplayName += " (Hidden)";

            var currentMenu = this.Menu;

            if (!hideSubMenus) return;

            var todo = new List<Menu>();

            var index = 0;

            do
            {
                {
                    if (!currentMenu.Children.Any()) break;

                    todo.AddRange(currentMenu.Children);

                    index++;

                    currentMenu = todo[index];
                }
            }
            while (true);

            foreach (var menu in todo) foreach (var menuItem in menu.Items) menuItem.Hide();
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            if (this.Initialized) return;

            this.Initialized = true;

            this.Path = this.Name;

            this.OnLoadEvent += this.OnLoad;

            this.SetMenu();
            this.SetSwitch();
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public virtual void Load()
        {
            if (this.Loaded) return;

            if (!this.Initialized) this.Initialize();

            this.Switch.OnDisableEvent += this.OnDisable;
            this.Switch.OnEnableEvent += this.OnEnable;

            this.Switch.Setup();

            this.Loaded = true;

            this.OnLoadEvent?.Invoke(null, new FeatureBaseEventArgs(this));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        /// <param name="sender">the sender of the input</param>
        /// <param name="eventArgs">the contextual information</param>
        protected virtual void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
#if DEBUG
            Console.WriteLine($"{this}: OnDisable");
#endif
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        /// ///
        /// <param name="sender">the sender of the input</param>
        /// <param name="eventArgs">the contextual information</param>
        protected virtual void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
#if DEBUG
            Console.WriteLine($"{this}: OnEnable");
#endif
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        /// <param name="sender">the sender of the input</param>
        /// <param name="eventArgs">the contextual information</param>
        protected virtual void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
#if DEBUG
            Console.WriteLine($"{this}: OnLoad");
#endif
        }

        /// <summary>
        ///     Called when [unload].
        /// </summary>
        /// <param name="sender">the sender of the input</param>
        /// <param name="eventArgs">the contextual information</param>
        protected virtual void OnUnload(object sender, FeatureBaseEventArgs eventArgs)
        {
            this.Switch.Disable(eventArgs);
        }

        /// <summary>
        ///     Sets the menu
        /// </summary>
        protected virtual void SetMenu()
        {
#if DEBUG
            Console.WriteLine($"{this}: SetMenu");
#endif
            this.Menu = new Menu(this.Path, this.Name);
        }

        /// <summary>
        ///     Sets the switch.
        /// </summary>
        protected virtual void SetSwitch()
        {
#if DEBUG
            Console.WriteLine($"{this}: SetSwitch");
#endif
            this.Switch = new BoolSwitch(this.Menu, "Enabled", true, this);
        }

        #endregion

        #region Nested type: FeatureBaseEventArgs

        /// <summary>
        ///     FeatureBaseEventArgs
        /// </summary>
        /// <seealso cref="System.EventArgs" />
        public class FeatureBaseEventArgs : EventArgs
        {
            #region Constructors and Destructors

            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="FeatureBaseEventArgs" /> class.
            /// </summary>
            /// <param name="sender">The sender.</param>
            public FeatureBaseEventArgs(Base sender)
            {
                this.Sender = sender;
            }

            #endregion

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the sender.
            /// </summary>
            /// <value>
            ///     The sender.
            /// </value>
            public Base Sender { get; set; }

            #endregion
        }

        #endregion
    }
}