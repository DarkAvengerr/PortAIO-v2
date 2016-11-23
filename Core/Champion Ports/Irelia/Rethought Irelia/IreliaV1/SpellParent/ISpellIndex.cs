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
 namespace Rethought_Irelia.IreliaV1.SpellParent
{
    #region Using Directives

    using System.Collections.Generic;

    using LeagueSharp;

    using RethoughtLib.FeatureSystem.Implementations;

    #endregion

    internal interface ISpellIndex
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        Dictionary<SpellSlot, SpellChild> Spells { get; set; }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     Gets or sets the <see cref="SpellChild" /> with the specified spell slot.
        /// </summary>
        /// <value>
        ///     The <see cref="SpellChild" />.
        /// </value>
        /// <param name="spellSlot">The spell slot.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Can't return a SpellChild for a SpellSlot that is non-existing</exception>
        SpellChild this[SpellSlot spellSlot] { get; }

        #endregion
    }
}