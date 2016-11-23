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
 namespace RethoughtLib.VersionChecker.Implementations
{
    #region Using Directives

    using System;
    using System.Threading.Tasks;

    #endregion

    public interface IVersionChecker
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the name of the assembly.
        /// </summary>
        /// <value>
        ///     The name of the assembly.
        /// </value>
        string AssemblyName { get; set; }

        /// <summary>
        ///     The GitHub path
        /// </summary>
        string GitHubPath { get; set; }

        /// <summary>
        ///     Gets the relative result of comparison.
        /// </summary>
        /// <value>
        ///     The relative result.
        /// </value>
        int RelativeResult { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether an update is available.
        /// </summary>
        /// <value>
        ///     <c>true</c> if update available; otherwise, <c>false</c>.
        /// </value>
        bool UpdateAvailable { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Checks this instance for an update.
        /// </summary>
        void Check();

        /// <summary>
        ///     Compares the specified versions.
        /// </summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        void Compare(Version version1, Version version2);

        /// <summary>
        ///     Gets the GitHub version.
        /// </summary>
        /// <returns></returns>
        Task<Version> GetGitHubVersion();

        #endregion
    }
}