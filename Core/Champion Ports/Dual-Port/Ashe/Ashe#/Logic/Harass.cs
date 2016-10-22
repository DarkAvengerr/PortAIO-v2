// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Harass.cs" company="LeagueSharp">
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
//   The harass.
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
    /// The harass.
    /// </summary>
    internal class Harass
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="Harass"/> class.
        /// </summary>
        static Harass()
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
            if (DoHarass || DoHarassT)
            {
                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (target.IsValidTarget())
                {
                    if (UseW && Player.ManaPercent >= ManaSlider)
                    {
                        W.Cast(target);
                    }
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether do harass.
        /// </summary>
        private static bool DoHarass
        {
            get
            {
                return Ashe.Menu.Item("HarassActive").GetValue<KeyBind>().Active;
            }
        }

        /// <summary>
        /// Gets a value indicating whether do harass t.
        /// </summary>
        private static bool DoHarassT
        {
            get
            {
                return Ashe.Menu.Item("HarassActiveT").GetValue<KeyBind>().Active;
            }
        }

        /// <summary>
        /// Gets the mana slider.
        /// </summary>
        private static int ManaSlider
        {
            get
            {
                return Ashe.Menu.Item("ManaSlider").GetValue<Slider>().Value;
            }
        }

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

        /// <summary>
        /// Gets a value indicating whether use w.
        /// </summary>
        private static bool UseW
        {
            get
            {
                return Ashe.Menu.Item("HarassWCombo").GetValue<bool>();
            }
        }

        /// <summary>
        /// Gets the w.
        /// </summary>
        private static Spell W
        {
            get
            {
                return Ashe.W;
            }
        }

        #endregion
    }
}