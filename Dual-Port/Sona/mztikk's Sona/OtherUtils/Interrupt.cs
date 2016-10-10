using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkSona.OtherUtils
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using mztikkSona.Extensions;

    using Config = mztikkSona.Config;

    internal static class Interrupt
    {
        #region Static Fields

        private static Interrupter2.DangerLevel wanteDangerLevel;

        #endregion

        #region Public Methods and Operators

        public static void OnInterruptableSpell(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsMe || sender.IsAlly || !Config.IsChecked("intr.bR"))
            {
                return;
            }

            switch (Config.GetStringListValue("dangerL"))
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

            if (Spells.R.CanCast() && sender.IsValidTarget(Spells.R.Range) && args.DangerLevel == wanteDangerLevel)
            {
                var delay = Computed.RDelay.Next(100, 120);
                LeagueSharp.Common.Utility.DelayAction.Add(delay, () => Spells.R.Cast(sender));
            }
        }

        #endregion
    }
}