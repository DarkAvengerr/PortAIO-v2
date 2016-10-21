using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Transitions.Implementations
{
    using System;

    using global::RethoughtLib.Transitions.Abstract_Base;

    /// <summary>
    ///     The expo ease in out.
    /// </summary>
    public class ExpoEaseInOut : TransitionBase
    {
        #region Constructors and Destructors

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
            if (time == 0)
            {
                return b;
            }

            if (time == startTime)
            {
                return b + c;
            }

            if ((time /= startTime / 2) < 1)
            {
                return c / 2 * Math.Pow(2, 10 * (time - 1)) + b;
            }

            return c / 2 * (-Math.Pow(2, -10 * --time) + 2) + b;
        }

        #endregion
    }
}