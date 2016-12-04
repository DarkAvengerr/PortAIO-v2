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
namespace RethoughtLib.Menu
{
    #region Using Directives

    using System;

    using LeagueSharp.Common;

    using RethoughtLib.Menu.Interfaces;

    #endregion

    #region Using Directives

    #endregion

    // REWORK TODO
    /// <summary>
    ///     Generates a Preset Menu to the given Menu
    /// </summary>
    public class MenuGenerator
    {
        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuGenerator" /> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="generator">The menu preset.</param>
        public MenuGenerator(Menu menu, IGenerator generator)
        {
            this.Generator = generator;
            this.Menu = menu;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        ///     The menu set
        /// </summary>
        public IGenerator Generator { get; set; }

        /// <summary>
        ///     The menu
        /// </summary>
        public Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Generates the menu.
        /// </summary>
        /// <exception cref="System.NullReferenceException">
        ///     Get sure that you declared a valid menuPreset and a valid menu in the
        ///     constructor before generating.
        /// </exception>
        public void Generate()
        {
            if ((this.Generator == null) || (this.Menu == null))
                throw new NullReferenceException(
                          "Get sure that you declared a valid menuPreset and a valid menu in the constructor before generating.");

            this.Generator.Generate(this.Menu);
        }

        #endregion
    }
}