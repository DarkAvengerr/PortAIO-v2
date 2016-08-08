using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Ezreal.Skills
{
    class EzrealW
    {
        internal static void ExecuteLogic()
        {
            if (Variables.spells[SpellSlot.W].IsEnabledAndReady())
            {
                var target = TargetSelector.GetTarget(Variables.spells[SpellSlot.W].Range * 0.75f, TargetSelector.DamageType.Physical);

                if (target.LSIsValidTarget(Variables.spells[SpellSlot.W].Range))
                {
                    Variables.spells[SpellSlot.W].CastIfHitchanceEquals(
                      target, target.IsMoving ? HitChance.VeryHigh : HitChance.High);
                }
            }
        }
    }
}
