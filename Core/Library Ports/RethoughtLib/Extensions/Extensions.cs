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
namespace RethoughtLib.Extensions
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    #endregion

    public static class Extensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Returns all units in Range
        /// </summary>
        /// <param name="position"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static int CountMinionsInRange(this Vector3 position, float range)
        {
            var minionList = MinionManager.GetMinions(position, range);

            return minionList?.Count ?? 0;
        }

        /// <summary>
        ///     returns true if unit is in AttackRange
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns></returns>
        public static bool InAutoAttackRange(this Obj_AI_Base unit)
        {
            return unit.Distance(ObjectManager.Player) <= ObjectManager.Player.AttackRange;
        }

        /// <summary>
        ///     Returns true if unit is airbone
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static bool IsAirbone(this Obj_AI_Base unit)
            => unit.HasBuffOfType(BuffType.Knockup) || unit.HasBuffOfType(BuffType.Knockback);

        /// <summary>
        ///     Raises the event.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static void RaiseEvent(this EventHandler @event, object sender, EventArgs e)
        {
            @event?.Invoke(sender, e);
        }

        /// <summary>
        ///     Raises the event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event">The event.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static void RaiseEvent<T>(this EventHandler<T> @event, object sender, T e) where T : EventArgs
        {
            @event?.Invoke(sender, e);
        }

        /// <summary>
        ///     Returns the remaining airbone time from unit
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static float RemainingAirboneTime(this Obj_AI_Base unit)
        {
            float result = 0;

            foreach (var buff in
                unit.Buffs.Where(buff => (buff.Type == BuffType.Knockback) || (buff.Type == BuffType.Knockup))) result = buff.EndTime - Game.Time;
            return result * 1000;
        }

        /// <summary>
        ///     To the color of the sharp dx.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static Color ToSharpDxColor(this System.Drawing.Color color)
            => new Color(color.R, color.G, color.B, color.A);

        /// <summary>
        ///     Converts a list of Obj_Ai_Base's to Vector3's.
        /// </summary>
        /// <param name="units">The units.</param>
        /// <returns></returns>
        public static List<Vector3> ToVector3S(this List<Obj_AI_Base> units)
        {
            return (from unit in units where unit.IsValid select unit.ServerPosition).ToList();
        }

        #endregion
    }
}