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

    internal class MenuTranslator
    {
        #region Fields

        /// <summary>
        ///     The menu
        /// </summary>
        internal Menu Menu;

        /// <summary>
        ///     The menu translation
        /// </summary>
        private readonly ITranslation menuTranslation;

        /// <summary>
        ///     Whether the menu got translated
        /// </summary>
        private bool translated;

        #endregion

        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuTranslator" /> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="translation">The translation.</param>
        public MenuTranslator(Menu menu, ITranslation translation)
        {
            this.Menu = menu;
            this.menuTranslation = translation;
        }

        #endregion

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Translates this instance.
        /// </summary>
        public void Translate()
        {
            if (this.translated || (this.menuTranslation == null)) return;

            this.translated = true;

            foreach (var entry in this.menuTranslation.Strings()) this.SearchAndTranslate(entry.Key, entry.Value);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Searches the menu-item and translates it.
        /// </summary>
        /// <param name="internalName">Name of the internal.</param>
        /// <param name="newDisplayName">New name of the display.</param>
        private void SearchAndTranslate(string internalName, string newDisplayName)
        {
            try
            {
                var item = this.Menu.Item(internalName);

                if (item != null) item.DisplayName = newDisplayName;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                          $"Failed translating > {internalName} into {newDisplayName}. Exception: {ex}");
            }
        }

        #endregion
    }
}