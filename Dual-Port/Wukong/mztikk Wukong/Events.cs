using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Wukong
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class Events
    {
        #region Static Fields

        private static Interrupter2.DangerLevel wanteDangerLevel;

        #endregion

        #region Public Methods and Operators

        public static void OnInterruptableSpell(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsMe || sender.IsAlly || !Config.IsChecked("interrupt.r"))
            {
                return;
            }

            switch (Config.GetStringListValue("interrupt.danger"))
            {
                case 0:
                    wanteDangerLevel = Interrupter2.DangerLevel.Low;
                    break;
                case 1:
                    wanteDangerLevel = Interrupter2.DangerLevel.Medium;
                    break;
                case 2:
                    wanteDangerLevel = Interrupter2.DangerLevel.High;
                    break;
                default:
                    wanteDangerLevel = Interrupter2.DangerLevel.High;
                    break;
            }

            if (Spells.R.IsReady() && sender.IsValidTarget(Spells.R.Range) && args.DangerLevel == wanteDangerLevel)
            {
                Spells.R.Cast();
            }
        }

        #endregion
    }
}