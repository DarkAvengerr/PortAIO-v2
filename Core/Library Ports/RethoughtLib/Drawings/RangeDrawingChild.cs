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
namespace RethoughtLib.Drawings
{
    #region Using Directives

    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    public sealed class RangeDrawingChild : ChildBase
    {
        #region Fields

        /// <summary>
        ///     The spell
        /// </summary>
        private readonly Spell spell;

        /// <summary>
        ///     Whether the spell must be ready
        /// </summary>
        private readonly bool spellMustBeReady;

        /// <summary>
        ///     The color
        /// </summary>
        private Color color = Color.White;

        /// <summary>
        ///     Whether the z-axis is considered
        /// </summary>
        private bool zAxis;

        #endregion

        #region Constructors and Destructors

        #region Constructors

        public RangeDrawingChild(Spell spell, string name, bool spellMustBeReady = false)
        {
            this.spell = spell;

            this.Name = name;

            this.spellMustBeReady = spellMustBeReady;
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
        /// <param name="sender">the sender of the input</param>
        /// <param name="eventArgs">the contextual information</param>
        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Drawing.OnDraw -= this.DrawingOnOnDraw;
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        /// <param name="sender">the sender of the input</param>
        /// <param name="eventArgs">the contextual information</param>
        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Drawing.OnDraw += this.DrawingOnOnDraw;
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        /// <param name="sender">the sender of the input</param>
        /// <param name="eventArgs">the contextual information</param>
        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            var colorPicker =
                this.Menu.AddItem(new MenuItem($"{this.Path}.color", "Color").SetValue(new Circle(true, Color.White)));

            colorPicker.ValueChanged += (o, args) => { this.color = args.GetNewValue<Circle>().Color; };

            this.color = colorPicker.GetValue<Circle>().Color;

            this.Menu.AddItem(
                new MenuItem($"{this.Path}.spellready", "Spell must be ready").SetValue(this.spellMustBeReady));

            var zaxisItem = this.Menu.AddItem(new MenuItem(this.Path + "." + "zaxis", "z-axis").SetValue(true));

            zaxisItem.ValueChanged += (o, args) => { this.zAxis = args.GetNewValue<bool>(); };

            this.zAxis = zaxisItem.GetValue<bool>();

            this.Menu.AddItem(
                new MenuItem($"{this.Path}.displaymethod", "Display Method").SetValue(
                    new StringList(new[] { "Render Drawings", "League Drawings" })));
        }

        /// <summary>
        ///     Triggers OnDraw
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void DrawingOnOnDraw(EventArgs args)
        {
            if (this.Menu.Item($"{this.Path}.spellready").GetValue<bool>() && !this.spell.IsReady()) return;

            if (this.Menu.Item($"{this.Path}.displaymethod").GetValue<StringList>().SelectedIndex == 0) Render.Circle.DrawCircle(ObjectManager.Player.Position, this.spell.Range, this.color, 3, this.zAxis);
            else Drawing.DrawCircle(ObjectManager.Player.Position, this.spell.Range, this.color);
        }

        #endregion
    }
}