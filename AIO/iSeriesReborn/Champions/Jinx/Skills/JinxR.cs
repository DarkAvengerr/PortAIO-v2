using System.Linq;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Jinx.Skills
{
    class JinxR
    {
        public static void HandleRLogic()
        {
            if (Variables.spells[SpellSlot.R].IsEnabledAndReady())
            {
                var target = TargetSelector.GetTarget(
                    Variables.spells[SpellSlot.R].Range * 0.75f, TargetSelector.DamageType.Magical);
                if (target.LSIsValidTarget() && target.LSDistance(ObjectManager.Player) > 400f)
                {
                    var enemiesAround = target.LSGetEnemiesInRange(450f);
                    if (ObjectManager.Player.LSDistance(target) > JinxUtility.GetFishboneRange() 
                        && HealthPrediction.GetHealthPrediction(target, 375) > 0 &&
                             HealthPrediction.GetHealthPrediction(target, 375) + 5 <
                             Variables.spells[SpellSlot.R].GetDamage(target))
                    {
                        //Target is over the minimum distance.
                        //Check for overkill logics. 

                        //We can kill target with W. Don't use R.
                        if ((Variables.spells[SpellSlot.W].IsEnabledAndReady() &&
                             Variables.spells[SpellSlot.W].GetPrediction(target).Hitchance >= HitChance.VeryHigh
                            ? Variables.spells[SpellSlot.W].GetDamage(target)
                            : 0) > target.Health)
                        {
                            return;
                        }

                        Variables.spells[SpellSlot.R].CastIfHitchanceEquals(target, HitChance.VeryHigh);

                    }
                    else if (ObjectManager.Player.LSDistance(target) < JinxUtility.GetFishboneRange() &&
                             HealthPrediction.GetHealthPrediction(target, 375) > 0 &&
                             HealthPrediction.GetHealthPrediction(target, 375) + 5 <
                             Variables.spells[SpellSlot.R].GetDamage(target))
                    {
                        //Else if the target is in range and we are low health and we can kill them.
                        //Cast R without prodiction.
                        if (ObjectManager.Player.HealthPercent < 10 && target.HealthPercent > ObjectManager.Player.HealthPercent + 20)
                        {
                            Variables.spells[SpellSlot.R].Cast(target.ServerPosition);
                            return;
                        }

                        //We can kill the target with W (If we can hit it, using prediction) and 5 AA then return.
                        if (ObjectManager.Player.LSGetAutoAttackDamage(target) * 5 +
                            (Variables.spells[SpellSlot.W].IsEnabledAndReady() &&
                             Variables.spells[SpellSlot.W].GetPrediction(target).Hitchance >= HitChance.VeryHigh
                                ? Variables.spells[SpellSlot.W].GetDamage(target)
                                : 0) >= target.Health)
                        {
                            return;
                        }

                        Variables.spells[SpellSlot.R].CastIfHitchanceEquals(target, HitChance.VeryHigh);
                    }
                    else if (
                        enemiesAround.Count(
                            m => Variables.spells[SpellSlot.R].GetDamage(m) >= target.Health * 0.25f + 5) > 1)
                    {
                        //We can do more than 25% health % damage to at least 3 enemies. Go for it lol.
                        Variables.spells[SpellSlot.R].CastIfHitchanceEquals(target, HitChance.High);
                    }
                }
            }
        }
    }
}
