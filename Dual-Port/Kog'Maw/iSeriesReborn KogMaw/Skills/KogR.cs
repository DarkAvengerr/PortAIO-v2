using DZLib.Logging;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.KogMaw.Skills
{
    class KogR
    {
        public static void ExecuteLogic()
        {
            if (Variables.spells[SpellSlot.R].IsEnabledAndReady())
            {
                if (KogUtils.GetRCount() <
                    MenuExtensions.GetItemValue<Slider>(
                        $"iseriesr.kogmaw.{Variables.Orbwalker.ActiveMode.ToString().ToLower()}.r.limit").Value)
                {
                    var rTarget = TargetSelector.GetTarget(Variables.spells[SpellSlot.R].Range, TargetSelector.DamageType.Magical);
                    if (rTarget.IsValidTarget())
                    {
                        var prediction = Variables.spells[SpellSlot.R].GetPrediction(rTarget);

                        if (prediction.Hitchance >= HitChance.Medium)
                        {
                               Variables.spells[SpellSlot.R].Cast(prediction.CastPosition);
                        }
                    }
                }
                
            }
        }
    }
}
