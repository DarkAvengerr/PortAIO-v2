using Dark_Star_Thresh.Core;
using LeagueSharp;
using LeagueSharp.Common;
using System;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Dark_Star_Thresh.Update
{
    class Misc : Core.Core
    {
        public static void Skinchanger(EventArgs args)
        {
            if (!MenuConfig.UseSkin)
            {
                return;
            }
        }

        public static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!MenuConfig.Interrupt || sender.IsInvulnerable) return;

            if (sender.IsValidTarget(Spells.E.Range))
            {
                if (Spells.E.IsReady())
                {
                    Spells.E.Cast(sender);
                }
            }
        }

        public static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!MenuConfig.Gapcloser) return;

            var sender = gapcloser.Sender;
            if (sender.IsEnemy && Spells.E.IsReady() && sender.IsValidTarget())
            {
                if (sender.IsValidTarget(Spells.E.Range))
                {
                    Spells.E.Cast(sender);
                }
            }
        }
    }
}
