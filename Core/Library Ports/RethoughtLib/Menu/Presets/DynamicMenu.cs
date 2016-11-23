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
 namespace RethoughtLib.Menu.Presets
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.Common;

    using RethoughtLib.Menu.Interfaces;

    #endregion

    public class DynamicMenu : IGenerator
    {
        #region Fields

        /// <summary>
        ///     The display name
        /// </summary>
        private readonly string displayName;

        /// <summary>
        ///     The menu sets
        /// </summary>
        private readonly List<List<MenuItem>> menuSets;

        /// <summary>
        ///     The selecter
        /// </summary>
        private readonly MenuItem selecter;

        /// <summary>
        ///     The new menu
        /// </summary>
        private Menu attachedMenu;

        #endregion

        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicMenu" /> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="selecter">The selecter.</param>
        /// <param name="menuItems">The menu items.</param>
        public DynamicMenu(string displayName, MenuItem selecter, IEnumerable<List<MenuItem>> menuItems)
        {
            this.displayName = displayName;
            this.selecter = selecter;
            this.menuSets = new List<List<MenuItem>>();

            foreach (var itemSet in menuItems) this.menuSets.Add(itemSet);
        }

        #endregion

        #endregion

        #region IGenerator Members

        #region Public Properties

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Setups the menu.
        /// </summary>
        /// <param name="menu">The menu</param>
        public void Generate(Menu menu)
        {
            this.Menu = menu;

            if ((this.Menu == null) || (this.menuSets == null) || !this.menuSets.Any()) return;

            this.attachedMenu = new Menu(this.displayName, this.Menu.Name + this.displayName);

            this.Menu.AddSubMenu(this.attachedMenu);

#if DEBUG
            Console.WriteLine(string.Empty);
            Console.WriteLine(@"==== Setting up new DynamicMenu ====");
            Console.WriteLine(@"DisplayName: " + this.displayName);
            Console.WriteLine(@"Internal Menu Name: " + this.Menu.Name + this.displayName);
            Console.WriteLine(@"MenuSets Amount: " + this.menuSets.Count);
            Console.WriteLine(@"Internal Selecter Name: " + this.selecter.Name);
            Console.WriteLine(@"Displaying Selecter Name: " + this.selecter.DisplayName);
            Console.WriteLine(@"Example Naming: " + this.displayName + @"SomeItemName");
#endif

            var value = this.selecter.GetValue<StringList>();

            this.attachedMenu.AddItem(
                    new MenuItem(this.attachedMenu.Name + this.selecter.Name, this.selecter.DisplayName).SetValue(value))
                .ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    foreach (var item in this.attachedMenu.Items)
                    {
                        if (item.Tag != 0) item.Hide();

                        if (item.Tag == eventArgs.GetNewValue<StringList>().SelectedIndex + 1) item.Show();
                    }
                };

            var tag = 1;

            foreach (var itemSet in this.menuSets)
            {
                foreach (var item in itemSet)
                {
                    item.Name = this.attachedMenu.Name + item.Name;
                    this.attachedMenu.AddItem(item).SetTag(tag);
                }
                tag++;
            }

#if DEBUG
            {
                foreach (var item in this.attachedMenu.Items) Console.WriteLine(@"DisplayName: {0}, Internal Name: {1}", item.DisplayName, item.Name);
            }
#endif

            MenuExtensions.RefreshTagBased(
                this.attachedMenu,
                this.attachedMenu.Item(this.attachedMenu.Name + this.selecter.Name).GetValue<StringList>().SelectedIndex
                + 1);
        }

        #endregion

        #endregion
    }
}