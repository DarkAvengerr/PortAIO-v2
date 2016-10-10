using System.Linq;
using iSeriesDZLib.Logging;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.Entities;
using iSeriesReborn.Utility.MenuUtility;
using iSeriesReborn.Utility.Positioning;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Tristana.Skills
{
    class TristanaE
    {
        internal static void HandleLogic()
        {
            if (Variables.spells[SpellSlot.E].IsEnabledAndReady())
            {
                var eTarget = TargetSelector.GetTarget(TristanaUtility.GetERRange(), TargetSelector.DamageType.Physical);
                if ((eTarget.IsValidTarget() && MenuExtensions.GetItemValue<bool>($"iseriesr.tristana.combo.eon.{eTarget.ChampionName.ToLower()}")) 
                    || PositioningVariables.EnemiesClose.Count() == 1)
                {
                    Variables.spells[SpellSlot.E].Cast(eTarget);
                }
            }
        }
    
        internal static void HandleLaneclear()
        {
            if (Variables.spells[SpellSlot.E].IsEnabledAndReady())
            {
                var minionsInRange =
                    GameObjects.EnemyMinions.Where(
                        m =>
                            m.IsValidTarget(Variables.spells[SpellSlot.E].Range) &&
                            GameObjects.EnemyMinions.Count(m_ex => m_ex.Distance(m) < 150f) > 1)
                .OrderBy(m => m.Health);

                if (minionsInRange.Any())
                {
                    var minion = minionsInRange.FirstOrDefault();
                    Variables.spells[SpellSlot.E].Cast(minion);
                }
            }
        }
    }
}
