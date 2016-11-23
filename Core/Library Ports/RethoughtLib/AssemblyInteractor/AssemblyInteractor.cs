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
 namespace RethoughtLib.AssemblyInteractor
{
    #region Using Directives

    using RethoughtLib.AssemblyInteractor.Abstract_Classes;

    #endregion

    public static class AssemblyInteractor
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Disables the by custom method.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public static void DisableByCustom(Assembly assembly)
        {
            assembly.DisableByCustom();
        }

        /// <summary>
        ///     Disables the by menu.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public static void DisableByMenu(Assembly assembly)
        {
            assembly.DisableByMenu();
        }

        /// <summary>
        ///     Enables the by custom method.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public static void EnableByCustom(Assembly assembly)
        {
            assembly.EnableByCustom();
        }

        /// <summary>
        ///     Enables the by menu.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public static void EnableByMenu(Assembly assembly)
        {
            assembly.EnableByMenu();
        }

        #endregion
    }
}