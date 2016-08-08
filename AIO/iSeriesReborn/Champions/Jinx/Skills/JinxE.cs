using System.Linq;
using DZLib.Logging;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.Entities;
using iSeriesReborn.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Jinx.Skills
{
    class JinxE
    {
        public static void HandleELogic()
        {
            var ESpell = Variables.spells[SpellSlot.E];
            if (ESpell.IsEnabledAndReady())
            {
                var meleeEnemiesOnMe =
                    GameObjects.EnemyHeroes.Where(
                        enemy =>
                            enemy.IsMelee &&
                            enemy.LSDistance(ObjectManager.Player.ServerPosition) < enemy.AttackRange + 65f).ToList();
                var lowHealth = ObjectManager.Player.HealthPercent < 15;

                if (meleeEnemiesOnMe.Any(m => !m.IsRunningAway()) && lowHealth)
                {
                    //There are enemies on me, I am low(ish) health, I cast E on myself to peel them.
                    ESpell.Cast(ObjectManager.Player.ServerPosition);
                }

                var selectedTarget = TargetSelector.GetTarget(Variables.spells[SpellSlot.E].Range * 0.75f, TargetSelector.DamageType.Physical);

                if (selectedTarget.LSIsValidTarget())
                {
                    //The selected target is valid. Is moving and is not coming towards us while we are facing them.
                    if (selectedTarget.HasBuffOfType(BuffType.Slow) 
                        && selectedTarget.Path.Count() > 1)
                    {
                        //We are facing the target, we have a high"ish" health and the target is coming towards us. No point in using E:
                        if (ObjectManager.Player.LSIsFacing(selectedTarget) &&
                            ObjectManager.Player.LSDistance(selectedTarget) >
                            ObjectManager.Player.LSDistance(selectedTarget.GetPositionInFront(300f))
                            && ObjectManager.Player.HealthPercent > 35)
                        {
                            return;
                        }

                        //Target is slowed.
                        var slowEndTime = JinxUtility.GetSlowEndTime(selectedTarget);
                        if (slowEndTime >= ESpell.Delay + 0.5f + Game.Ping / 2f)
                        {
                            ESpell.CastIfHitchanceEquals(selectedTarget, HitChance.VeryHigh);
                        }
                    } 
                    else if (JinxUtility.IsHeavilyImpaired(selectedTarget))
                    {
                        //The target is actually impaired heavily. Let's cast E on them.
                        var immobileEndTime = JinxUtility.GetImpairedEndTime(selectedTarget);
                        if (immobileEndTime >= ESpell.Delay + 0.5f + Game.Ping / 2f)
                        {
                            ESpell.CastIfHitchanceEquals(selectedTarget, HitChance.VeryHigh);
                        }
                    } else if (selectedTarget.LSGetEnemiesInRange(350f).Count() >= 2 
                        && ESpell.GetPrediction(selectedTarget).Hitchance >= HitChance.High)
                    {
                        //We can almost certainly hit our targets and also at least 2 other targets.
                        var enemiesInRange = selectedTarget.LSGetEnemiesInRange(350f);
                        if (enemiesInRange.Count(enemy => ESpell.GetPrediction(enemy).Hitchance >= HitChance.High) >= 2)
                        {
                            //Cast E.
                            ESpell.Cast(ESpell.GetPrediction(selectedTarget).CastPosition);
                        }
                    }
                }
            }
        }

        internal static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender.LSIsValidTarget() 
                && sender is AIHeroClient 
                && MenuExtensions.GetItemValue<bool>("iseriesr.jinx.e.ops")
                && Variables.spells[SpellSlot.E].LSIsReady())
            {
                if (JinxUtility.GetESpellDict().ContainsKey((sender as AIHeroClient).ChampionName))
                {
                    if (args.Slot == JinxUtility.GetESpellDict()[(sender as AIHeroClient).ChampionName])
                    {
                        const int ESpeed = 2000;
                        var distance = ObjectManager.Player.LSDistance(sender);
                        //Do the calculations for E speed. If it will reach in time then cast E.
                        if (distance / ESpeed < 0.4f)
                        {
                            Variables.spells[SpellSlot.E].Cast(sender.ServerPosition);
                        }
                    }
                }
            }
        }

        internal static void OnGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.LSIsValidTarget()
                && Variables.spells[SpellSlot.E].LSIsReady()
                && MenuExtensions.GetItemValue<bool>("iseriesr.jinx.e.agp")
                && ObjectManager.Player.ManaPercent > 30)
            {
                Variables.spells[SpellSlot.E].Cast(gapcloser.End);
            }
        }
    }
}
