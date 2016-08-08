using System;
using DZLib.Logging;
using iSeriesReborn.Utility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Kalista.Skills
{
    class KalistaHooks
    {
        public static AIHeroClient SoulBound { get; set; }

        internal static void OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe && args.Order == GameObjectOrder.MoveTo)
            {
                //Console.WriteLine(@"Movement Order issued!");
            }
        }

        internal static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (sender.IsMe && args.SData.Name.Equals("KalistaExpungeWrapper"))
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(0x7D, Orbwalking.ResetAutoAttackTimer);
                }
            }
            catch
            {
                LogHelper.AddToLog(new LogItem("Kalista_ProcessSpellCast", "Error: Failed to reset AA", LogSeverity.Error));
            }
        }

        internal static void OnNonKillableMinion(AttackableUnit minion)
        {
            try
            {
                if (Variables.spells[SpellSlot.E].LSIsReady() &&
                    MenuExtensions.GetItemValue<bool>("iseriesr.kalista.misc.lhassit"))
                {
                    if (Variables.spells[SpellSlot.E].CanCast((Obj_AI_Base)minion)
                        && KalistaE.CanBeRendKilled((Obj_AI_Base)minion))
                    {
                        if (Environment.TickCount - Variables.spells[SpellSlot.E].LastCastAttemptT >= 500)
                        {
                            Variables.spells[SpellSlot.E].Cast();
                            Variables.spells[SpellSlot.E].LastCastAttemptT = Environment.TickCount;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                LogHelper.AddToLog(new LogItem("Kalista_OnNonKillableMinion", e, LogSeverity.Error));
            }
        }
    }
}
