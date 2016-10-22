// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Damages.cs" company="LeagueSharp">
//   Copyright (C) 2015 L33T
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
//   The damages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Ekko_the_Boy_Who_Shattered_Time
{
    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The damages.
    /// </summary>
    public class Damages
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Calculates the E Damage.
        /// </summary>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <returns>
        ///     The damage.
        /// </returns>
        public static double GetDamageE(Obj_AI_Base target)
        {
            return Ekko.Spells[SpellSlot.E].IsReady()
                       ? Ekko.Player.CalcDamage(
                           target, 
                           Damage.DamageType.Magical, 
                           new double[] { 50, 80, 110, 140, 170 }[Ekko.Spells[SpellSlot.E].Level - 1]
                           + Ekko.Player.TotalMagicalDamage * .2f)
                       : 0d;
        }

        /// <summary>
        ///     Calculates the Q Damage.
        /// </summary>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <returns>
        ///     The damage.
        /// </returns>
        public static double GetDamageQ(Obj_AI_Base target)
        {
            return Ekko.Spells[SpellSlot.Q].IsReady()
                       ? Ekko.Player.CalcDamage(
                           target, 
                           Damage.DamageType.Magical, 
                           new double[] { 60, 75, 90, 105, 120 }[Ekko.Spells[SpellSlot.Q].Level - 1]
                           + Ekko.Player.TotalMagicalDamage * .2f)
                       : 0d;
        }

        /// <summary>
        ///     Calculates the R Damage.
        /// </summary>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <returns>
        ///     The damage.
        /// </returns>
        public static double GetDamageR(Obj_AI_Base target)
        {
            return Ekko.Spells[SpellSlot.R].IsReady()
                       ? Ekko.Player.CalcDamage(
                           target, 
                           Damage.DamageType.Magical, 
                           new double[] { 200, 350, 500 }[Ekko.Spells[SpellSlot.R].Level - 1]
                           + Ekko.Player.TotalMagicalDamage * 1.3f)
                       : 0d;
        }

        #endregion
    }
}