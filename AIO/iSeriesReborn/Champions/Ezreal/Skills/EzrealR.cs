using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Ezreal.Skills
{
    class EzrealR
    {
        internal static void ExecuteLogic()
        {
            if (Variables.spells[SpellSlot.R].IsEnabledAndReady())
            {
                var target = TargetSelector.GetTarget(2500f, TargetSelector.DamageType.Physical);

                if (target.LSIsValidTarget(Variables.spells[SpellSlot.R].Range) 
                    && CanExecuteTarget(target) 
                    && ObjectManager.Player.LSDistance(target) >= Orbwalking.GetRealAutoAttackRange(null) * 0.80f
                    && !(target.Health + 5 < ObjectManager.Player.LSGetAutoAttackDamage(target) * 2 + Variables.spells[SpellSlot.Q].GetDamage(target)))
                {
                    Variables.spells[SpellSlot.R].CastIfHitchanceEquals(
                      target, target.IsMoving ? HitChance.VeryHigh : HitChance.High);
                }

                var rPred = Variables.spells[SpellSlot.R].GetPrediction(target);
                if (rPred.AoeTargetsHitCount >= 3)
                {
                    Variables.spells[SpellSlot.R].Cast(rPred.CastPosition);
                }
            }
        }

        private static bool CanExecuteTarget(AIHeroClient target)
        {
            double damage = 1f;

            var prediction = Variables.spells[SpellSlot.R].GetPrediction(target);
            var count = prediction.CollisionObjects.Count;

            damage += ObjectManager.Player.LSGetSpellDamage(target, SpellSlot.R);

            if (count >= 7)
            {
                damage = damage * .3;
            }
            else if (count != 0)
            {
                damage = damage * (10 - count / 10);
            }

            return damage > target.Health + 10;
        }
    }
}
