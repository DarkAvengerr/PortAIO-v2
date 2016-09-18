using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Transitions.Implementations
{
    using System;

    using global::RethoughtLib.Transitions.Abstract_Base;

    /// <summary>
    ///     The elastic ease in out.
    /// </summary>
    public class ElasticEaseInOut : TransitionBase
    {
        #region Constructors and Destructors

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
            if ((time /= startTime / 2) == 2)
            {
                return b + c;
            }

            var p = startTime * (.3 * 1.5);
            var s = p / 4;

            if (time < 1)
            {
                return -.5 * (c * Math.Pow(2, 10 * (time -= 1)) * Math.Sin((time * startTime - s) * (2 * Math.PI) / p)) + b;
            }

            return c * Math.Pow(2, -10 * (time -= 1)) * Math.Sin((time * startTime - s) * (2 * Math.PI) / p) * .5 + c + b;
        }

        #endregion
    }
}