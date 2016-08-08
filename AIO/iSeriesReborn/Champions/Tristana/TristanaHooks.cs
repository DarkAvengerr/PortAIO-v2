using System.Linq;
using DZLib.Logging;
using iSeriesReborn.Utility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Tristana
{
    class TristanaHooks
    {
        internal static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Variables.spells[SpellSlot.R].LSIsReady() 
                && MenuExtensions.GetItemValue<bool>("iseriesr.tristana.misc.antigp")
                &&
                !ObjectManager.Player.LSGetEnemiesInRange(TristanaUtility.GetERRange())
                    .Any(m => m.Health + 5 < TristanaUtility.GetRDamage(m)))
            {
                if (gapcloser.Sender.LSIsValidTarget(TristanaUtility.GetERRange()))
                {
                    Variables.spells[SpellSlot.R].Cast(gapcloser.Sender);
                }
            }
        }

        internal static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Variables.spells[SpellSlot.R].LSIsReady()
                && MenuExtensions.GetItemValue<bool>("iseriesr.tristana.misc.interrupter")
                && !ObjectManager.Player.LSGetEnemiesInRange(TristanaUtility.GetERRange())
                    .Any(m => m.Health + 5 < TristanaUtility.GetRDamage(m)))
            {
                if (sender.LSIsValidTarget(TristanaUtility.GetERRange()) && args.DangerLevel >= Interrupter2.DangerLevel.High)
                {
                    Variables.spells[SpellSlot.R].Cast(sender);
                }
            }
        }
    }
}
