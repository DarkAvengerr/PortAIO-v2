// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpellManager.cs" company="LeagueSharp">
//   Copyright (C) 2016 LeagueSharp
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
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicGalio.Managers
{
    #region Using Directives

    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class SpellManager
    {
        #region Static Fields

        /// <summary>
        ///     Spells
        /// </summary>
        private static readonly Dictionary<SpellEnum, Spell> Spells = new Dictionary<SpellEnum, Spell>
                                                                          {
                                                                              {
                                                                                  SpellEnum.Q,
                                                                                  new Spell(SpellSlot.Q, 940f)
                                                                              },
                                                                              {
                                                                                  SpellEnum.W,
                                                                                  new Spell(SpellSlot.W, 800f)
                                                                              },
                                                                              {
                                                                                  SpellEnum.E,
                                                                                  new Spell(SpellSlot.E, 1180f)
                                                                              },
                                                                              {
                                                                                  SpellEnum.R,
                                                                                  new Spell(SpellSlot.R, 600f)
                                                                              }
                                                                          };

        #endregion

        #region Enums

        internal enum SpellEnum
        {
            Q,

            W,

            E,

            R
        }

        #endregion

        #region Public Properties

        public static Spell E => Spells[SpellEnum.E];

        public static Spell Q => Spells[SpellEnum.Q];

        public static Spell R => Spells[SpellEnum.R];

        public static Spell W => Spells[SpellEnum.W];

        #endregion
    }
}