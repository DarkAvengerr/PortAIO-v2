using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkSona.OtherUtils
{
    using LeagueSharp.Common;

    using mztikkSona.Extensions;

    using Config = mztikkSona.Config;

    internal static class Gapclose
    {
        #region Public Methods and Operators

        public static void OnGapclose(ActiveGapcloser gapcloser)
        {
            if (Config.IsChecked("gapclose.bE") && Spells.E.CanCast())
            {
                Spells.E.Cast();
            }
        }

        #endregion
    }
}