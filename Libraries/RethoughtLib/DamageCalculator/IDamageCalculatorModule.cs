using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.DamageCalculator
{
    internal interface IDamageCalculatorModule : IDamageCalculator
    {
        #region Public Properties

        int EstimatedAmountInOneCombo { get; }

        string Name { get; set; }

        #endregion
    }
}