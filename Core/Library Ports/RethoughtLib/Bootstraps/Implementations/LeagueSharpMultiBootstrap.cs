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
 namespace RethoughtLib.Bootstraps.Implementations
{
    #region Using Directives

    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.Bootstraps.Abstract_Classes;

    #endregion

    public class LeagueSharpMultiBootstrap : PlaySharpBootstrapBase
    {
        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LeagueSharpMultiBootstrap" /> class.
        /// </summary>
        public LeagueSharpMultiBootstrap(
            IEnumerable<LoadableBase> modules = null,
            IEnumerable<string> additionalStrings = null)
        {
            if (modules != null) this.Modules = new List<LoadableBase>(modules);

            if (additionalStrings != null) this.Strings = new List<string>(additionalStrings);

            this.AddString(ObjectManager.Player.ChampionName);
        }

        #endregion

        #endregion
    }
}