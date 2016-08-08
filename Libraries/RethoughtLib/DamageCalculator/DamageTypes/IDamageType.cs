using EloBuddy; namespace RethoughtLib.DamageCalculator.DamageTypes
{
    #region Using Directives

    using LeagueSharp;

    #endregion

    /// <summary>
    ///     Interface to define a DamageType
    /// </summary>
    public interface IDamageType
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Calculates the expected damage output.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        double Calculate(double value, Obj_AI_Base target, Obj_AI_Base source = null);

        #endregion
    }
}