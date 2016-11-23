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
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.Menu.Interfaces;

    #endregion

    public class ChampionSliderMenu : IGenerator
    {
        #region Fields

        /// <summary>
        ///     The function to set the slider value
        /// </summary>
        public Func<AIHeroClient, int> FuncSliderValue = x => 0;

        /// <summary>
        ///     The function to validate a hero
        /// </summary>
        public Func<AIHeroClient, bool> FuncValidateHero = x => true;

        /// <summary>
        ///     The display name
        /// </summary>
        private readonly string displayName;

        /// <summary>
        ///     The new menu
        /// </summary>
        private Menu attachedMenu;

        #endregion

        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChampionSliderMenu" /> class.
        /// </summary>
        /// <param name="displayName">
        ///     The menu.
        ///     The display name.
        /// </param>
        public ChampionSliderMenu(string displayName)
        {
            this.displayName = displayName;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        public int Modifier { get; set; }

        #endregion

        #region Public Methods and Operators

        #region IGenerator Members

        #region Public Methods and Operators

        /// <summary>
        ///     Generates this instance.
        /// </summary>
        /// <param name="menu">the menu</param>
        public void Generate(Menu menu)
        {
            this.Menu = menu;

            this.SetupMenu();
            this.AddEnemies();
        }

        #endregion

        #endregion

        #endregion

        #region Methods

        /// <summary>
        ///     Adds the enemies.
        /// </summary>
        private void AddEnemies()
        {
            if (HeroManager.Enemies.Count == 0)
            {
                this.attachedMenu.AddItem(new MenuItem(this.attachedMenu.Name + "null", "No enemies found"));
            }
            else
            {
                var maxValue =
                    HeroManager.Enemies.Where(hero => this.FuncValidateHero(hero))
                        .Select(hero => this.FuncSliderValue(hero))
                        .Concat(new[] { 0 })
                        .Max();

                foreach (var hero in HeroManager.Enemies)
                {
                    var value = this.FuncSliderValue(hero);

                    this.attachedMenu.AddItem(
                        new MenuItem(this.attachedMenu.Name + hero.ChampionName, hero.ChampionName).SetValue(
                            new Slider(value + this.Modifier, 0, maxValue)));
                }
            }
        }

        /// <summary>
        ///     Setups the menu.
        /// </summary>
        private void SetupMenu()
        {
            if (this.Menu == null) return;

            this.attachedMenu = new Menu(this.displayName, this.Menu.Name + this.displayName);

            this.Menu.AddSubMenu(this.attachedMenu);

            this.attachedMenu.AddItem(new MenuItem(this.attachedMenu.Name + "Enabled", "Enabled").SetValue(true));

            this.attachedMenu.AddItem(
                        new MenuItem($"{this.attachedMenu.Name}Modifier", "Modifier").SetValue(new Slider(0, -2000, 2000)))
                    .ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        this.Modifier = eventArgs.GetNewValue<Slider>().Value;
                    };
        }

        #endregion
    }
}