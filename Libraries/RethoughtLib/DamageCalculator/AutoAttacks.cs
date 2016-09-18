using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.DamageCalculator
{
    #region Using Directives

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    /// <summary>
    ///     Auto-Attacks DamageCalculatorModule
    /// </summary>
    /// <seealso cref="IDamageCalculatorModule" />
    internal class AutoAttacks : IDamageCalculatorModule
    {
        #region Public Properties

        /// <summary>
        ///     Gets the estimated amount in one combo.
        /// </summary>
        /// <value>
        ///     The estimated amount in one combo.
        /// </value>
        public int EstimatedAmountInOneCombo { get; } = 2;

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; } = "Auto-Attacks";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the damage.
        /// </summary>
        /// <param name="target">The get damage.</param>
        /// <returns></returns>
        public float GetDamage(Obj_AI_Base target)
        {
            return (float)ObjectManager.Player.GetAutoAttackDamage(target);
        }

        #endregion
    }
}