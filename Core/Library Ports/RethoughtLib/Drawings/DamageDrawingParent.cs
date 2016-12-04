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
//     Last Edited: 04.10.2016 1:46 PM

using EloBuddy; 
using LeagueSharp.Common; 
namespace RethoughtLib.Drawings
{
    #region Using Directives

    using LeagueSharp;
    using LeagueSharp.Common;
    using RethoughtLib.Design;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    /// <summary>
    ///     The damage drawing parent.
    /// </summary>
    public sealed class DamageDrawingParent : ParentBase
    {
        #region Constants

        /// <summary>
        ///     The hero bar height
        /// </summary>
        private const int HeroBarHeight = 9;

        /// <summary>
        ///     The hero bar width
        /// </summary>
        private const int HeroBarWidth = 103;

        /// <summary>
        ///     The hero x offset
        /// </summary>
        private const int HeroXOffset = 10;

        /// <summary>
        ///     The hero y offset
        /// </summary>
        private const int HeroYOffset = 20;

        #endregion

        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DamageDrawingParent" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public DamageDrawingParent(string name)
        {
            this.Name = name;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the damage calculators.
        /// </summary>
        /// <value>
        ///     The damage calculators.
        /// </value>
        public List<IDamageDrawing> DamageCalculators { get; set; } = new List<IDamageDrawing>();

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the specified damage calculator.
        /// </summary>
        /// <param name="damageCalculator">The damage calculator.</param>
        public void Add(IDamageDrawing damageCalculator)
        {
            this.DamageCalculators.Add(damageCalculator);

            var @base = damageCalculator as Base;

            if (@base == null) return;

            this.Add(@base);
        }

        /// <summary>
        ///     Adds the specified damage calculators.
        /// </summary>
        /// <param name="damageCalculators">The damage calculators.</param>
        public void Add(IEnumerable<IDamageDrawing> damageCalculators)
        {
            foreach (var damageCalculator in damageCalculators) this.Add(damageCalculator);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="Base.FeatureBaseEventArgs" /> instance containing the event data.</param>
        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Drawing.OnEndScene -= this.DrawingOnEndScene;
        }

        /// <summary>
        ///     Called when [enable].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="Base.FeatureBaseEventArgs" /> instance containing the event data.</param>
        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Drawing.OnEndScene += this.DrawingOnEndScene;
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="Base.FeatureBaseEventArgs" /> instance containing the event data.</param>
        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            this.Menu.AddItem(
                new MenuItem(this.Path + ".ordermode", "Order Mode: ").SetValue(
                    new StringList(new[] { "Static Order", "Dynamic Ordering (Most Damage first)" }, 1)));
        }

        /// <summary>
        /// Drawings the on end scene.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void DrawingOnEndScene(EventArgs args)
        {
            if ((Drawing.Direct3DDevice == null) || Drawing.Direct3DDevice.IsDisposed) return;

            var offset = new Offset<int>();

            // IDrawTarget - Must have offset and some value (health?)
            foreach (var hero in HeroManager.Enemies.Where(x => !x.IsDead && x.IsHPBarRendered))
            {
                offset.Left = 11;
                offset.Bottom = 24;

                var enemyHealth = hero.Health;

                var enemyHealthMultiplicative = enemyHealth / hero.MaxHealth;

                var end = new Vector2(
                              (int)hero.HPBarPosition.X + offset.Left + enemyHealthMultiplicative * HeroBarWidth,
                              (int)hero.HPBarPosition.Y + offset.Bottom);

                foreach (var drawer in this.DamageCalculators)
                {
                    var healthPercDamageApplied = drawer.GetDamage(hero) / hero.MaxHealth;

                    var newSectionLength = (int)(HeroBarWidth * healthPercDamageApplied);

                    var start = new Vector2(
                                    (int)hero.HPBarPosition.X + offset.Left + newSectionLength,
                                    (int)hero.HPBarPosition.Y + offset.Bottom);

                    if ((int)start.Distance(end) != newSectionLength) start = end.Extend(start, newSectionLength);

                    if ((int)start.Distance(end) == 0) continue;

                    drawer.Draw(start, end, 9, Drawing.Direct3DDevice);

                    end = start;
                }
            }
        }

        #endregion
    }
}