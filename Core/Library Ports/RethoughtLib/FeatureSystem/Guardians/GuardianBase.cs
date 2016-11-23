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
 namespace RethoughtLib.FeatureSystem.Guardians
{
    #region Using Directives

    using System;

    #endregion

    /// <summary>
    ///     Guardian
    /// </summary>
    public abstract class GuardianBase
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="GuardianBase" /> is negated.
        /// </summary>
        /// <value>
        ///     <c>true</c> if negated; otherwise, <c>false</c>.
        /// </value>
        public bool Negated { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the function.
        /// </summary>
        /// <value>
        ///     The function.
        /// </value>
        protected Func<bool> Func { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Checks this instance.
        /// </summary>
        /// <returns></returns>
        public bool Check()
        {
            return this.Negated ? !this.Func.Invoke() : this.Func.Invoke();
        }

        #endregion
    }
}