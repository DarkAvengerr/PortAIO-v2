// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Misc.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The misc.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace AsheSharp.Source.Logic
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    /// The misc.
    /// </summary>
    internal class Misc
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="Misc"/> class.
        /// </summary>
        static Misc()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The game_ on update.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Game_OnUpdate(EventArgs args)
        {
            // Auto buy blue trinket
            if (BuyTrinket && Player.Level >= 6 && Player.InShop() && !(Items.HasItem(3342) || Items.HasItem(3363)))
            {
                Shop.BuyItem(ItemId.Farsight_Alteration);
            }
        }

        #endregion

        #region Properties

        // Booleans
        /// <summary>
        /// Gets a value indicating whether buy trinket.
        /// </summary>
        private static bool BuyTrinket
        {
            get
            {
                return Ashe.Menu.Item("blueTrinket").GetValue<bool>();
            }
        }

        // Player
        /// <summary>
        /// Gets the player.
        /// </summary>
        private static AIHeroClient Player
        {
            get
            {
                return Ashe.Player;
            }
        }

        #endregion
    }
}