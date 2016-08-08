using EloBuddy; namespace RethoughtLib.DamageCalculator.DamageTypes.Implementations
{
    #region Using Directives

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    public class Magical : IDamageType
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Calculates the expected damage output.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public double Calculate(double value, Obj_AI_Base target, Obj_AI_Base source = null)
        {
            if (source == null) source = ObjectManager.Player;

            return source.CalcDamage(target, Damage.DamageType.Magical, value);
        }

        #endregion
    }
}