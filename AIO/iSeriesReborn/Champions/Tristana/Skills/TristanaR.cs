using System.Collections.Generic;
using System.Linq;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Tristana.Skills
{
    class TristanaR
    {
        internal static void HandleLogic()
        {
            if (Variables.spells[SpellSlot.R].IsEnabledAndReady())
            {
                var selectedTarget = TargetSelector.GetTarget(
                    TristanaUtility.GetERRange(), TargetSelector.DamageType.Physical);
                if (selectedTarget.LSIsValidTarget())
                {
                    var selectedTargetHealth = HealthPrediction.GetHealthPrediction(
                        selectedTarget,
                        (int)
                            (250 + Game.Ping / 2f + ObjectManager.Player.LSDistance(selectedTarget.ServerPosition) / 2000f));
                    if (selectedTargetHealth > 0 && selectedTargetHealth < TristanaUtility.GetRDamage(selectedTarget))
                    {
                        Variables.spells[SpellSlot.R].Cast(selectedTarget);
                        return;
                    }

                    var enemiesClose =
                        ObjectManager.Player.LSGetEnemiesInRange(250f).Where(m => m.LSIsValidTarget()).OrderBy(m => m.LSDistance(ObjectManager.Player)).ThenByDescending(m => m.GetComboDamage(ObjectManager.Player, new List<SpellSlot>{SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R}));

                    var firstEnemy = enemiesClose.FirstOrDefault();
                    
                    //There are enemies
                    //Sort them by combo damage to us
                    //If we are low health and they have a lot of health
                    //And they are not killable with R and 4 AA
                    //Repel them away with R

                    if (firstEnemy != null 
                        && firstEnemy.Health > ObjectManager.Player.Health * 2.0f 
                        && ObjectManager.Player.HealthPercent < 8
                        && !(firstEnemy.Health + 5 < TristanaUtility.GetRDamage(selectedTarget) + ObjectManager.Player.LSGetAutoAttackDamage(firstEnemy) * 4))
                    {
                        Variables.spells[SpellSlot.R].Cast(firstEnemy);
                    }
                }
            }
        }
    }
}
