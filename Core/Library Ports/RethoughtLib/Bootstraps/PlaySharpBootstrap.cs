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
 namespace RethoughtLib.Bootstraps
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using RethoughtLib.Bootstraps.Abstract_Classes;

    #endregion

    public class PlaySharpBootstrap : PlaySharpBootstrapBase
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Adds the module.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <exception cref="ArgumentException">There can't be multiple similiar modules in the PlaySharpBootstrap.</exception>
        public override void AddModule(LoadableBase module)
        {
            base.AddModule(module);
        }

        /// <summary>
        ///     Adds the module.
        /// </summary>
        /// <param name="modules">the modules</param>
        /// <exception cref="ArgumentException">There can't be multiple similiar modules in the PlaySharpBootstrap.</exception>
        public override void AddModules(IEnumerable<LoadableBase> modules)
        {
            base.AddModules(modules);
        }

        /// <summary>
        ///     Adds a string with witch the bootstrap is checking for modules.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void AddString(string value)
        {
            base.AddString(value);
        }

        /// <summary>
        ///     Adds strings with witch the bootstrap is checking for modules.
        /// </summary>
        /// <param name="values">the values</param>
        public override void AddStrings(IEnumerable<string> values)
        {
            base.AddStrings(values);
        }

        /// <summary>
        ///     Removes the module.
        /// </summary>
        /// <param name="module">The module.</param>
        public override void RemoveModule(LoadableBase module)
        {
            base.RemoveModule(module);
        }

        /// <summary>
        ///     Removes a string with witch the bootstrap was checking for modules.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void RemoveString(string value)
        {
            base.RemoveString(value);
        }

        /// <summary>
        ///     Removes strings with witch the bootstrap was checking for modules.
        /// </summary>
        /// <param name="values">the values</param>
        public override void RemoveStrings(IEnumerable<string> values)
        {
            base.RemoveStrings(values);
        }

        /// <summary>
        ///     Compares module names with entries in the strings list. If they match it will load the module.
        /// </summary>
        public override void Run()
        {
            base.Run();
        }

        #endregion
    }
}