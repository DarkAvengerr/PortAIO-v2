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
 namespace Rethought_Irelia
{
    #region Using Directives

    using RethoughtLib.Bootstraps.Implementations;

    using Rethought_Irelia.IreliaV1;

    #endregion

    /// <summary>
    ///     The program.
    /// </summary>
    internal class Program
    {
        #region Methods

        /// <summary>
        ///     The main.
        /// </summary>
        public static void Main()
        {
            var bootstrap = new LeagueSharpMultiBootstrap();

            bootstrap.AddModule(new Loader());

            bootstrap.Run();
        }

        #endregion
    }
}