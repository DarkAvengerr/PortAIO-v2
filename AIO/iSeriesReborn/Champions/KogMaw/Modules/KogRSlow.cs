using System;
using System.Linq;
using DZLib.Logging;
using iSeriesReborn.Champions.Jinx;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.ModuleHelper;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.KogMaw.Modules
{
    class KogRSlow : IModule
    {
        public string GetName()
        {
            return "KogR_Slow";
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldRun()
        {
            return Variables.spells[SpellSlot.R].LSIsReady() &&
                   MenuExtensions.GetItemValue<bool>("iseriesr.kogmaw.misc.r.slow");
        }

        public void Run()
        {
            if (Variables.spells[SpellSlot.R].LSIsReady())
            {
                var rTarget = TargetSelector.GetTarget(Variables.spells[SpellSlot.R].Range, TargetSelector.DamageType.Magical);
                if (rTarget.LSIsValidTarget()
                    && (JinxUtility.IsHeavilyImpaired(rTarget) || JinxUtility.IsLightlyImpaired(rTarget)))
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
