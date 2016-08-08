using iSeriesReborn.Utility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.KogMaw.Skills
{
    class KogQ
    {
        public static void ExecuteLogic()
        {
            var target = TargetSelector.GetTarget(Variables.spells[SpellSlot.Q].Range * 0.70f,
                TargetSelector.DamageType.Magical);

            if (target.LSIsValidTarget())
            {
                var prediction = Variables.spells[SpellSlot.Q].GetPrediction(target);

                var targetHealth = target.Health;
                var AADamage = ObjectManager.Player.LSGetAutoAttackDamage(target);

                if (targetHealth <= AADamage * 4 && prediction.Hitchance > HitChance.Medium)
                {
                    Variables.spells[SpellSlot.Q].Cast(prediction.CastPosition);
                }
                else if (prediction.Hitchance >= HitChance.VeryHigh)
                {
                    Variables.spells[SpellSlot.Q].Cast(prediction.CastPosition);
                }
            }
        }
    }
}
