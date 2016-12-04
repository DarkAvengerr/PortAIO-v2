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
namespace RethoughtLib.Core
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using global::RethoughtLib.Classes.General_Intefaces;
    using global::RethoughtLib.Events;

    using LeagueSharp.Common;

    #endregion

    // TODO REMOVE
    public class RethoughtLib : ILoadable
    {
        #region Fields

        /// <summary>
        ///     The loadables
        /// </summary>
        private readonly List<ILoadable> loadables = new List<ILoadable> { new Events() };

        /// <summary>
        ///     The initialized
        /// </summary>
        private bool initialized;

        #endregion

        #region Public Properties

        public static RethoughtLib Instance { get; } = new RethoughtLib();

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; } = nameof(RethoughtLib);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            if (this.initialized) return;

            this.initialized = true;

            Console.WriteLine("Test");

            this.Game_OnGameLoad(new EventArgs());
        }

        #endregion

        #region Explicit Interface Methods

        #region ILoadable Members

        #region Explicit Interface Methods

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        void ILoadable.Load()
        {
            this.Load();
        }

        #endregion

        #endregion

        #endregion

        #region Methods

        private void Game_OnGameLoad(EventArgs args)
        {
            foreach (var loadable in this.loadables) loadable.Load();
        }

        #endregion

        #region Constructors

        static RethoughtLib()
        {
        }

        private RethoughtLib()
        {
        }

        #endregion
    }
}