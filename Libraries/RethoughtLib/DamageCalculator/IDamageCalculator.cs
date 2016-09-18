using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.DamageCalculator
{
    #region Using Directives

    using LeagueSharp;

    #endregion

    internal interface IDamageCalculator
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the damage.
        /// </summary>
        /// <param name="target">The get damage.</param>
        /// <returns></returns>
        float GetDamage(Obj_AI_Base target);

        #endregion
    }
}