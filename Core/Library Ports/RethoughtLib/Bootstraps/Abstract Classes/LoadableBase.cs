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
//     Last Edited: 04.10.2016 1:43 PM

using EloBuddy; 
using LeagueSharp.Common; 
 namespace RethoughtLib.Bootstraps.Abstract_Classes
{
    #region Using Directives

    using System.Collections.Generic;

    using LeagueSharp.Common;

    using RethoughtLib.Classes.General_Intefaces;

    #endregion

    /// <summary>
    ///     Class that represents something loadable
    /// </summary>
    /// <seealso cref="RethoughtLib.Classes.General_Intefaces.ILoadable" />
    /// <seealso cref="RethoughtLib.Classes.General_Intefaces.ITagable" />
    public abstract class LoadableBase : ILoadable, ITagable
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the name that will get displayed.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public abstract string DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the internal name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public abstract string InternalName { get; set; }

        /// <summary>
        ///     Gets or sets the root menu.
        /// </summary>
        /// <value>
        ///     The root menu.
        /// </value>
        public Menu RootMenu { get; set; }

        /// <summary>
        ///     Gets or sets the tags.
        /// </summary>
        /// <value>
        ///     The tags.
        /// </value>
        public abstract IEnumerable<string> Tags { get; set; }

        #endregion

        #region Public Methods and Operators

        #region ILoadable Members

        #region Public Methods and Operators

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public abstract void Load();

        #endregion

        #endregion

        #endregion
    }
}