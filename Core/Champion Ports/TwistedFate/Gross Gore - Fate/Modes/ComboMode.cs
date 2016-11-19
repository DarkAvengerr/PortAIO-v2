using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate.Modes
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = GrossGoreTwistedFate.Config;

    internal static class ComboMode
    {
        #region Methods

        internal static void Execute()
        {
            if (Spells.W.IsReady())
            {
                CardSelector.StartSelecting(Cards.Yellow);
            }
        }

        #endregion
    }
}