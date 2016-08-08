using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Soraka_HealBot
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    class OtherUtils
    {
        #region Static Fields

        internal static readonly Random RDelay = new Random();

        #endregion

        #region Public Methods and Operators

        public static void OnGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly || gapcloser.Sender.IsMe || gapcloser.Sender.IsDead)
            {
                return;
            }

            if (Config.IsChecked("qGapclose") && Spells.Q.CanCast(gapcloser.Sender))
            {
                var delay = RDelay.Next(50, 120);
                LeagueSharp.Common.Utility.DelayAction.Add(delay, () => Spells.Q.Cast(gapcloser.End));
            }

            if (Config.IsChecked("eGapclose") && Spells.E.CanCast(gapcloser.Sender))
            {
                var delay = RDelay.Next(50, 120);
                LeagueSharp.Common.Utility.DelayAction.Add(delay, () => Spells.E.Cast(gapcloser.End));
            }
        }

        public static void OnInterruptableSpell(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsMe || sender.IsAlly || !Config.IsChecked("bInterrupt"))
            {
                return;
            }

            var wantedLevel = Interrupter2.DangerLevel.High;

            switch (Config.GetStringListValue("dangerL"))
            {
                case 0:
                    wantedLevel = Interrupter2.DangerLevel.Low;
                    break;
                case 1:
                    wantedLevel = Interrupter2.DangerLevel.Medium;
                    break;
                case 2:
                    wantedLevel = Interrupter2.DangerLevel.High;
                    break;
            }

            if (Spells.E.CanCast(sender) && Spells.E.IsInRange(sender) && args.DangerLevel == wantedLevel)
            {
                var delay = RDelay.Next(100, 120);
                LeagueSharp.Common.Utility.DelayAction.Add(delay, () => Spells.E.Cast(sender));
            }
        }

        #endregion
    }
}