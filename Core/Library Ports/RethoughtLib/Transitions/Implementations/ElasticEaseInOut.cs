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
    ///     The elastic ease in out.
    /// </summary>
    public class ElasticEaseInOut : TransitionBase
    {
        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ElasticEaseInOut" /> class.
        /// </summary>
        /// <param name="duration">
        ///     The duration.
        /// </param>
        public ElasticEaseInOut(double duration)
            : base(duration)
        {
        }

        #endregion

        #endregion

        #region Public Methods and Operators

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
            if ((time /= startTime / 2) == 2) return b + c;

            var p = startTime * (.3 * 1.5);
            var s = p / 4;

            if (time < 1)
                return -.5 * (c * Math.Pow(2, 10 * (time -= 1)) * Math.Sin((time * startTime - s) * (2 * Math.PI) / p))
                       + b;

            return c * Math.Pow(2, -10 * (time -= 1)) * Math.Sin((time * startTime - s) * (2 * Math.PI) / p) * .5 + c
                   + b;
        }

        #endregion
    }
}