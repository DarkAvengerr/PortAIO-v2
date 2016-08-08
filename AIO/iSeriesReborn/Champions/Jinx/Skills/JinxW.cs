using System.Collections.Generic;
using System.Linq;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;
using iSeriesReborn.Utility.Entities;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Jinx.Skills
{
    class JinxW
    {
        public static void HandleWLogic()
        {
            var defaultHitchance = HitChance.VeryHigh;

            if (Variables.spells[SpellSlot.W].IsEnabledAndReady())
            {
                //If there are 2 or more enemies in our AA range and our health percentage is lower than
                //the average of theirs then don't fire W. 
                //We only get heroes with a priority of 2 or higher, as they are more likely to do more damage to us.

                var enemiesAround = ObjectManager.Player.LSGetEnemiesInRange(JinxUtility.GetMinigunRange(null));


                if (enemiesAround.Any())
                {
                    //If the enemies around have more average health percent then we do * 1.5f (Since we are an ADC and there might be tanks)
                    //among the enemy team members that surround us.
                    //Or their average combo will do more than 60% of our health in damage.
                    var spellsList = new List<SpellSlot>()
                    {
                        SpellSlot.Q,
                        SpellSlot.W,
                        SpellSlot.E,
                        SpellSlot.R
                    };

                    if (enemiesAround.Count() == 1)
                    {
                        var killableEnemy = enemiesAround.FirstOrDefault(k => k.LSIsValidTarget());
                        if (killableEnemy != null)
                        {

                            //We are lower health than a tankier enemy and enemy is in melee range. Better put some distance before casting W.
                            if (ObjectManager.Player.Health * 1.4f < killableEnemy.Health && killableEnemy.ServerPosition.LSDistance(ObjectManager.Player.ServerPosition) <= 350f)
                            {
                                return;
                            }

                            var WPrediction = Variables.spells[SpellSlot.W].GetPrediction(killableEnemy);
                            //if there is only 1 target and it is killable by W and we are not about to die then shoot W and pew pew mode.
                            if (WPrediction.Hitchance >= defaultHitchance
                                && ObjectManager.Player.HealthPercent > 8
                                && HealthPrediction.GetHealthPrediction(killableEnemy, 300) > 0
                                && HealthPrediction.GetHealthPrediction(killableEnemy, 300) + 5 < Variables.spells[SpellSlot.W].GetDamage(killableEnemy))
                            {
                                Variables.spells[SpellSlot.W].Cast(WPrediction.CastPosition);
                            }

                            
                        }
                        
                    }

                    if (enemiesAround.Count(m => ProrityHelper.GetPriorityFromDb(m.ChampionName) >= 2) > 1
                        &&
                        (ObjectManager.Player.HealthPercent * 1.5f <=
                        enemiesAround.Where(m => ProrityHelper.GetPriorityFromDb(m.ChampionName) >= 2)
                            .Average(m => m.HealthPercent)
                        || enemiesAround.Average(enemy => enemy.GetComboDamage(ObjectManager.Player, spellsList) + enemy.LSGetAutoAttackDamage(ObjectManager.Player) * 2f) >= ObjectManager.Player.Health * 0.60f))
                    {
                        return;
                    }

                    //If there are 3 enemies which are not low health, always return.
                    if (enemiesAround.Count(en => en.HealthPercent > 20) > 3)
                    {
                        return;
                    }
                }

                var selectedTarget = TargetSelector.GetTarget(Variables.spells[SpellSlot.W].Range * 0.65f,
                    TargetSelector.DamageType.Physical);

                if (selectedTarget.LSIsValidTarget())
                {
                    //Since we already determined  the conditions for not firing W when enemies are in our AA range
                    //We can now not do any check here and just fire W.
                    var WPrediction = Variables.spells[SpellSlot.W].GetPrediction(selectedTarget);
                    //Cast the spell using prediction.
                    if (WPrediction.Hitchance >= defaultHitchance)
                    {
                        //Don't use W to poke under enemy turret unless the target is killable.
                        if (ObjectManager.Player.LSUnderTurret(true) 
                            && HealthPrediction.GetHealthPrediction(selectedTarget, 300) + 5 > Variables.spells[SpellSlot.W].GetDamage(selectedTarget))
                        {
                            return;
                        }

                        Variables.spells[SpellSlot.W].Cast(WPrediction.CastPosition);
                    }
                }
            }
        }
    }
}
