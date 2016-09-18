using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Transitions.Implementations
{
    using global::RethoughtLib.Transitions.Abstract_Base;

    /// <summary>
    ///     The quad ease in out.
    /// </summary>
    public class QuadEaseOut : TransitionBase
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="QuadEaseOut" /> class.
        /// </summary>
        /// <param name="duration">
        ///     The duration.
        /// </param>
        public QuadEaseOut(double duration)
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
            return -c * (time /= startTime) * (time - 2) + b;
        }

        #endregion
    }
}