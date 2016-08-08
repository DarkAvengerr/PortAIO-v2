using System.Linq;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Tristana.Skills
{
    class TristanaQ
    {
        internal static void HandleLogic()
        {
            if (Variables.spells[SpellSlot.Q].IsEnabledAndReady())
            {
                var target = Variables.Orbwalker.GetTarget();
                if (target is AIHeroClient)
                {
                    var targetHero = target as AIHeroClient;

                    if (targetHero.LSIsValidTarget())
                    {
                        //Determine whether we should use Q or not.
                        var enemiesInRange =
                            ObjectManager.Player.LSGetEnemiesInRange(Orbwalking.GetRealAutoAttackRange(null));
                        if (enemiesInRange.Any())
                        {
                            var maxPriorityHero =
                                enemiesInRange.OrderByDescending(TargetSelector.GetPriority).FirstOrDefault();
                            if (maxPriorityHero != null 
                                && maxPriorityHero.LSIsValidTarget() 
                                && target.NetworkId == maxPriorityHero.NetworkId)
                            {
                                Variables.spells[SpellSlot.Q].Cast();
                                return;
                            }

                            if (targetHero.Health + 5 < ObjectManager.Player.LSGetAutoAttackDamage(targetHero, true) * 4)
                            {
                                Variables.spells[SpellSlot.Q].Cast();
                            }
                        }
                       
                    }
                }
            }
        }
    }
}
