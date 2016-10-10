using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Rethought_Irelia.IreliaV1.DamageCalculator
{
    internal interface IDamageCalculatorModule : IDamageCalculator
    {
        #region Public Properties

        int EstimatedAmountInOneCombo { get; }

        string Name { get; set; }

        #endregion
    }
}