using System;
using iSeriesDZLib.Logging;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Jinx
{
    class JinxHumanizer
    {
        private static float LastQAttempt = 0f;

        internal static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe 
                && MenuExtensions.GetItemValue<bool>("iseriesr.jinx.q.humanize")
                && args.Slot == SpellSlot.Q)
            {
                if (Environment.TickCount - LastQAttempt < 250f)
                {
                    args.Process = false;
                    return;
                }
                LastQAttempt = Environment.TickCount;
            }
        }
    }
}
