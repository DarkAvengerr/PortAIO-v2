using System.Linq;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Ezreal.Skills
{
    class EzrealQ
    {
        internal static void ExecuteLogic()
        {
            if (Variables.spells[SpellSlot.Q].IsEnabledAndReady())
            {
                var target = TargetSelector.GetTarget(Variables.spells[SpellSlot.Q].Range * 0.75f, TargetSelector.DamageType.Physical);

                if (target.LSIsValidTarget(Variables.spells[SpellSlot.Q].Range))
                {
                    Variables.spells[SpellSlot.Q].CastIfHitchanceEquals(
                      target, target.IsMoving ? HitChance.VeryHigh : HitChance.High);
                }
            }
        }

        internal static void ExecuteFarmLogic()
        {
            if (Variables.spells[SpellSlot.Q].IsEnabledAndReady())
            {
                var minion = MinionManager.GetMinions(Variables.spells[SpellSlot.Q].Range).Where(m => 
                    HealthPrediction.GetHealthPrediction(m, (int)(m.LSDistance(ObjectManager.Player) / Variables.spells[SpellSlot.Q].Speed * 1000f)) > Variables.spells[SpellSlot.Q].GetDamage(m) - 5);
                var qFarmLocation = Variables.spells[SpellSlot.Q].GetLineFarmLocation(minion.ToList());

                if (qFarmLocation.MinionsHit >= 1)
                {
                    Variables.spells[SpellSlot.Q].Cast(qFarmLocation.Position);
                }
            }
        }
    }
}
