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
namespace RethoughtLib.Transitions.Abstract_Base
{
    #region Using Directives

    using System;

    using LeagueSharp.Common;

    using SharpDX;

    #endregion

    /// <summary>
    ///     The transition.
    /// </summary>
    public abstract class TransitionBase
    {
        #region Fields

        /// <summary>
        ///     The end position.
        /// </summary>
        private Vector3 endPosition;

        /// <summary>
        ///     The final value.
        /// </summary>
        private float finalValue;

        /// <summary>
        ///     The last value.
        /// </summary>
        private float lastValue;

        /// <summary>
        ///     Gets or sets the last position.
        /// </summary>
        private Vector3 startPosition;

        /// <summary>
        ///     The start value.
        /// </summary>
        private float startValue;

        #endregion

        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TransitionBase" /> class.
        /// </summary>
        /// <param name="duration">
        ///     The duration.
        /// </param>
        protected TransitionBase(double duration)
        {
            this.Duration = duration;
            this.StartTime = -9999999;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the duration.
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        ///     Gets a value indicating whether moving.
        /// </summary>
        public bool Moving => this.Time <= this.StartTime + this.Duration;

        /// <summary>
        ///     Gets or sets the start time.
        /// </summary>
        public float StartTime { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the time.
        /// </summary>
        private float Time => Utils.TickCount;

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
        public abstract double Equation(double time, double b, double c, double startTime);

        /// <summary>
        ///     The get position.
        /// </summary>
        /// <returns>
        ///     The <see cref="Vector2" />.
        /// </returns>
        public Vector3 GetPosition()
        {
            if (!this.Moving) return this.endPosition;

            return this.startPosition.Extend(
                this.endPosition,
                (float)
                this.Equation(
                    this.Time - this.StartTime,
                    0,
                    this.endPosition.Distance(this.startPosition),
                    this.Duration));
        }

        /// <summary>
        ///     The get value.
        /// </summary>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public float GetValue()
        {
            if ((Math.Abs(this.startValue) < 0.1f) && (Math.Abs(this.finalValue) < 0.1f))
            {
                this.lastValue =
                    (float)
                    this.Equation(
                        this.Time - this.StartTime,
                        0,
                        this.endPosition.Distance(this.startPosition),
                        this.Duration);
                return this.lastValue;
            }

            if (!this.Moving && (this.StartTime > 0)) return this.lastValue;

            this.lastValue =
                (float)this.Equation(this.Time - this.StartTime, this.startValue, this.finalValue, this.Duration);
            return this.lastValue;
        }

        /// <summary>
        ///     Starts transisting.
        /// </summary>
        /// <param name="from">
        ///     The from.
        /// </param>
        /// <param name="to">
        ///     The to.
        /// </param>
        /// <param name="startTime"></param>
        public void Start(Vector3 from, Vector3 to, int startTime = -1)
        {
            if (startTime == -1) this.StartTime = Utils.TickCount;
            else this.StartTime = startTime;

            this.startPosition = from;
            this.endPosition = to;

            Console.WriteLine(
                $"Start: {this.startPosition}, End: {this.endPosition}, StartTime: {this.StartTime}, Duration: {this.Duration}, Moving {this.Moving}");
        }

        /// <summary>
        ///     The start.
        /// </summary>
        /// <param name="from">
        ///     The from.
        /// </param>
        /// <param name="to">
        ///     The to.
        /// </param>
        public void Start(float from, float to)
        {
            this.lastValue = from;
            this.startValue = from;
            this.finalValue = to;
            this.StartTime = this.Time;
        }

        #endregion
    }
}