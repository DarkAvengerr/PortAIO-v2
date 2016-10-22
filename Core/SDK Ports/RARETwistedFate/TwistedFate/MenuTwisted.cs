#region copyrights

//  Copyright 2016 Marvin Piekarek
//  MenuTwisted.cs is part of RARETwistedFate.
//  RARETwistedFate is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//  RARETwistedFate is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//  You should have received a copy of the GNU General Public License
//  along with RARETwistedFate. If not, see <http://www.gnu.org/licenses/>.

#endregion

#region usages

using System;
using LeagueSharp.SDK.UI;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace RARETwistedFate.TwistedFate
{
    internal static class MenuTwisted
    {
        public static Menu MainMenu;
        private static int _cBlank = -1;

        #region initialize menu 

        public static void Init(TwistedFate tf)
        {
            MainMenu = new Menu("raretftdz", "RARETwistedFate", true, tf.Player.ChampionName).Attach();
            MainMenu.Separator("We love LeagueSharp.");
            MainMenu.Separator("Developer: @Kyon");

            var qMenu = MainMenu.Add(new Menu("Q", "Q spell"));
            {
                qMenu.Separator("Combo");
                qMenu.Bool("ComboQ", "Use Q");
                qMenu.Separator("Hybrid");
                qMenu.Bool("HybridQ", "Use Q");
                qMenu.Separator("Farm");
                qMenu.Bool("FarmQ", "Use Q");
                qMenu.Separator("LastHit");
                qMenu.Bool("LastQ", "Use Q", false);
                qMenu.Separator("Utils");
                qMenu.Bool("ImmoQ", "Auto Q on immobile");
                qMenu.Bool("OnlyImmoQ", "Only auto Q on immobile", false);

            }

            var wMenu = MainMenu.Add(new Menu("W", "W spell"));
            {
                wMenu.Separator("Combo");
                wMenu.Bool("ComboW", "Use W");
                wMenu.Separator("Hybrid");
                wMenu.Bool("HybridW", "Use W");
                wMenu.Separator("Farm // LastHit");
                wMenu.Bool("FarmW", "Use W");
                wMenu.Separator("CardPicker");
                wMenu.Bool("ShowButton", "Show Button");
                //wMenu.List("ActiveCard", "Active Card", Enum.GetNames(typeof(Cards)), 1);
            }

            var rMenu = MainMenu.Add(new Menu("R", "R spell"));
            {
                rMenu.Separator("FastCardPicker");
                rMenu.Bool("Pick", "Autopick card");
                rMenu.List("ActiveCard", "Active Card", Enum.GetNames(typeof(Cards)), 2);
            }

            var comboMenu = MainMenu.Add(new Menu("Utilities", "Utilities"));
            {
                comboMenu.Bool("AA", "Using AA in LastHit or LaneClear");
            }

            var drawMenu = MainMenu.Add(new Menu("Draw", "Draw"));
            {
                drawMenu.Bool("Q", "Draws Q");
                drawMenu.Bool("W", "Draws W");
                drawMenu.Bool("R", "Draws R");
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     lets you create a new menupoint inside a <seealso cref="Menu" />.
        /// </summary>
        /// <param name="subMenu">Your SubMenu to add it to</param>
        /// <param name="name">the so called ID</param>
        /// <param name="display">The displayed name inside the game</param>
        /// <param name="state">the default state of the menu</param>
        /// <returns>returns a <seealso cref="MenuBool" /> the can be used.</returns>
        public static MenuBool Bool(this Menu subMenu, string name, string display, bool state = true)
        {
            return subMenu.Add(new MenuBool(name, display, state));
        }

        public static MenuList List(this Menu subMenu, string name, string display, string[] array, int value = 0)
        {
            return subMenu.Add(new MenuList<string>(name, display, array) {Index = value});
        }

        public static MenuSeparator Separator(this Menu subMenu, string display)
        {
            _cBlank += 1;
            return subMenu.Add(new MenuSeparator("blank" + _cBlank, display));
        }

        public static MenuSlider Slider(this Menu subMenu, string name, string display,
            int cur, int min = 0, int max = 100)
        {
            return subMenu.Add(new MenuSlider(name, display, cur, min, max));
        }

        #endregion
    }
}