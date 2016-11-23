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
 namespace RethoughtLib.Transitions.Implementations
{
    #region Using Directives

    using System;

    using RethoughtLib.Transitions.Abstract_Base;

    #endregion

    /// <summary>
    ///     The expo ease in out.
    /// </summary>
    public class ExpoEaseInOut : TransitionBase
    {
        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpoEaseInOut" /> class.
        /// </summary>
        /// <param name="duration">
        ///     The duration.
        /// </param>
        public ExpoEaseInOut(double duration)
            : base(duration)
        {
        }

        #endregion

        #endregion

        #region Public Methods and Operators

        // TODO RENAME FUCKING PARAMS AND GIVE EXPLANATION
        /// <summary>
        ///     The equation.
        /// </summary>
        /// <param name="time">
        ///     The t.
        /// </param>
        /// <param name="b">
        ///     The b.
        /// </param>
        /// <param name="c">
        ///     The c.
        /// </param>
        /// <param name="startTime">
        ///     The d.
        /// </param>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        public override double Equation(double time, double b, double c, double startTime)
        {
            if (time == 0) return b;

            if (time == startTime) return b + c;

            if ((time /= startTime / 2) < 1) return c / 2 * Math.Pow(2, 10 * (time - 1)) + b;

            return c / 2 * (-Math.Pow(2, -10 * --time) + 2) + b;
        }

        #endregion
    }
}