using System;
using System.Linq;
using DZLib.Logging;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.ModuleHelper;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.KogMaw.Modules
{
    class KogRKS : IModule
    {
        public string GetName()
        {
            return "KogR_Ks";
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldRun()
        {
            return Variables.spells[SpellSlot.R].IsReady() &&
                   MenuExtensions.GetItemValue<bool>("iseriesr.kogmaw.misc.r.ks");
        }

        public void Run()
        {
            if (Variables.spells[SpellSlot.R].IsReady())
            {
                    var rTarget = TargetSelector.GetTarget(Variables.spells[SpellSlot.R].Range, TargetSelector.DamageType.Magical);
                    if (rTarget.IsValidTarget() 
                        && HealthPrediction.GetHealthPrediction(rTarget, 300) > 0 
                        && HealthPrediction.GetHealthPrediction(rTarget, 300) + 5 < Variables.spells[SpellSlot.R].GetDamage(rTarget))
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
