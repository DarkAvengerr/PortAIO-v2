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
namespace RethoughtLib.FeatureSystem.Implementations.SpellParent
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Switches;

    #endregion

    public sealed class SpellParent : ParentBase, ISpellIndex
    {
        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpellParent" /> class.
        /// </summary>
        public SpellParent()
        {
            this.Spells = new Dictionary<SpellSlot, SpellChild>();
        }

        #endregion

        #endregion

        #region Public Indexers

        /// <summary>
        ///     Gets or sets the <see cref="RethoughtLib.FeatureSystem.Implementations.SpellChild" /> with the specified spell
        ///     slot.
        /// </summary>
        /// <value>
        ///     The <see cref="RethoughtLib.FeatureSystem.Implementations.SpellChild" />.
        /// </value>
        /// <param name="spellSlot">The spell slot.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Can't return a SpellChild for a SpellSlot that is non-existing</exception>
        public SpellChild this[SpellSlot spellSlot]
        {
            get
            {
                if (spellSlot == SpellSlot.Unknown) throw new InvalidOperationException("SpellSlot.Unknown is invalid");
                var spellChild = this.Spells[spellSlot];

                if (spellChild == null) throw new InvalidOperationException("Can't return a SpellChild for a SpellSlot that is null");

                return spellChild;
            }
        }

        #endregion

        #region Explicit Interface Indexers

        #region ISpellIndex Members

        #region Explicit Interface Indexers

        SpellChild ISpellIndex.this[SpellSlot spellSlot] => this.Spells[spellSlot];

        #endregion

        #endregion

        #endregion

        #region Public Properties

#pragma warning disable CC0021 // Use nameof
        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "Spells";

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        public Dictionary<SpellSlot, SpellChild> Spells { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [load].
        /// </summary>
        /// <param name="sender">the sender of the input</param>
        /// <param name="eventArgs">the contextual information</param>
        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            foreach (var spell in this.Children.OfType<SpellChild>())
            {
                if (spell.Spell == null) return;

                this.Spells[spell.Spell.Slot] = spell;
            }
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