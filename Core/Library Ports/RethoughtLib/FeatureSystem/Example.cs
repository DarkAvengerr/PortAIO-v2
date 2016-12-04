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
namespace RethoughtLib.FeatureSystem
{
    #region Using Directives

    using System;

    using LeagueSharp;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    #endregion

    internal class Example
    {
        #region Methods

        private static void Setup()
        {
            // The root "menu"
            var root = new SuperParent("Root");

            // A normal Menu
            var comboParent = new Parent("Combo");
            comboParent.Switch = new BoolSwitch(comboParent.Menu, "Disabled", false, comboParent);

            // 2 children containing the same logic
            var child = new ExampleChild("Child");
            var child2 = new ExampleChild("Child2");

            /* "Connects" each composition element
             *
             * compositionElement.AddChild(compositionElement)
             *
            */

            root.Add(comboParent);

            comboParent.Add(child);
            comboParent.Add(child2);

            /* Example Output:
             * Root > Combo            > Child            > Enabled [On/Off]
             *        Enabled [On/Off]   Child2           > Enabled [On/Off]
             *                           Enabled [On/Off]
            */
        }

        #endregion

        #region Nested type: ExampleChild

        private sealed class ExampleChild : ChildBase
        {
            #region Constructors and Destructors

            #region Constructors

            public ExampleChild(string name)
            {
                this.Name = name;
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
            /// <param name="sender">The sender.</param>
            /// <param name="eventArgs">the contextual information</param>
            protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
            {
                Game.OnUpdate -= GameOnOnUpdate;
            }

            /// <summary>
            ///     Called when [enable]
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="eventArgs">the contextual information</param>
            protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
            {
                Game.OnUpdate += GameOnOnUpdate;
            }

            /// <summary>
            ///     Called when [load].
            /// </summary>
            /// <param name="sender">the sender of the input</param>
            /// <param name="eventArgs">the contextual information</param>
            protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
            {
                // Add things to the menu for example, it will auto-generate you don't need to create one nor to add it to another menu
            }

            /// <summary>
            ///     OnUpdate
            /// </summary>
            /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
            private static void GameOnOnUpdate(EventArgs args)
            {
                // Some Logic
            }

            #endregion
        }

        #endregion
    }
}