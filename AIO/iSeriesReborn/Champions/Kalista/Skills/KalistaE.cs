using System;
using System.Linq;
using DZLib.Logging;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Kalista.Skills
{
    class KalistaE
    {
        private static float LastCastTime = 0f;

        public static void ExecuteComboLogic()
        {
            var spells = Variables.CurrentChampion.GetSpells();

            if (spells[SpellSlot.E].IsEnabledAndReady())
            {
                var killableRendTarget= HeroManager.Enemies.FirstOrDefault(CanBeRendKilled);

                if (killableRendTarget.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(killableRendTarget)))
                {
                    //The target is in E range, let's execute the logic.
                    var heroWithRendStack =
                        HeroManager.Enemies.Where(
                            target =>
                                target.LSIsValidTarget(Variables.CurrentChampion.GetSpells()[SpellSlot.E].Range) &&
                                target.HasRend()).OrderByDescending(GetRendDamage).FirstOrDefault();

                    if (killableRendTarget != null && (Environment.TickCount - LastCastTime > 250))
                    {
                        //We found a killable target.
                        spells[SpellSlot.E].Cast();
                        LastCastTime = Environment.TickCount;
                    }
                    else if (heroWithRendStack != null && (Environment.TickCount - LastCastTime > 250))
                    {
                        //We found a target with rend stacks on them.
                        var rendBuffCount = heroWithRendStack.GetRendBuff().Count;
                        if (rendBuffCount >= MenuExtensions.GetItemValue<Slider>($"iseriesr.kalista.{Variables.Orbwalker.ActiveMode.ToString().ToLower()}.e.minstacks").Value)
                        {
                            //The target has the minimum required rend buff count.
                            var distance = ObjectManager.Player.LSDistance(heroWithRendStack);

                            if (distance > spells[SpellSlot.E].Range * 0.85f ||
                                (heroWithRendStack.IsRendAboutToExpire()))
                            {
                                //The Target is about to leave range or the rend buff is about to expire.
                                spells[SpellSlot.E].Cast();
                                LastCastTime = Environment.TickCount;
                            }
                        }
                    }
                }
                else
                {
                    //The Target is not in E range, we'll do some math and get minions to AA to get to the target.
                }
               
            }
        }

        public static bool CanBeRendKilled(Obj_AI_Base target)
        {
            return target.HasRend() 
                    && target.LSIsValidTarget(Variables.CurrentChampion.GetSpells()[SpellSlot.E].Range) &&
                   (HealthPrediction.GetHealthPrediction(target, 250) > 0 &&
                    HealthPrediction.GetHealthPrediction(target, 250) + 20 <= GetRendDamage(target)) &&
                   (CanBeDamaged(target));
        }

        public static float GetRendDamage(Obj_AI_Base target)
        {
            if (!CanBeDamaged(target))
            {
                return 0f;
            }

            var spells = Variables.CurrentChampion.GetSpells();
            var baseDamage = spells[SpellSlot.E].GetDamage(target);

            if (ObjectManager.Player.LSHasBuff("summonerexhaust"))
            {
                baseDamage *= 0.4f;
            }

            if (target.LSHasBuff("FerociousHowl"))
            {
                baseDamage *= 0.35f;
            }

            return baseDamage;
        }

        public static bool CanBeDamaged(Obj_AI_Base target)
        {
            var invulnerable = !(target.IsInvulnerable || TargetSelector.IsInvulnerable(target, TargetSelector.DamageType.Physical, false));

            return invulnerable;
        }
    }
}
