using System;
using System.Linq;
using DZLib.Logging;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.Evade;
using iSeriesReborn.Utility.ModuleHelper;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Ezreal.Modules
{
    class EzrealAutoQ : IModule
    {
        public string GetName()
        {
            return "Jinx_AutoW";
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldRun()
        {
            return Variables.spells[SpellSlot.Q].IsReady() &&
                    MenuExtensions.GetItemValue<bool>("iseriesr.ezreal.misc.autoq.immobile");
        }

        public void Run()
        {
            var selectedTarget = TargetSelector.GetTarget(Variables.spells[SpellSlot.Q].Range * 0.80f, TargetSelector.DamageType.Physical);
            var QSpell = Variables.spells[SpellSlot.Q];

            if (selectedTarget.IsValidTarget())
            {
                //The selected target is valid.
                var healthPrediction =
                    HealthPrediction.GetHealthPrediction(
                        selectedTarget,
                        250 + Game.Ping / 2 +
                        (int) (ObjectManager.Player.Distance(selectedTarget) / QSpell.Speed) * 1000);
                var prediction = QSpell.GetPrediction(selectedTarget);

                if (healthPrediction + 5 <= QSpell.GetDamage(selectedTarget) && healthPrediction > 0)
                {
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        QSpell.Cast(prediction.CastPosition);
                        return;
                    }
                }

                if (prediction.Hitchance >= HitChance.Immobile)
                {
                    QSpell.Cast(prediction.CastPosition);
                }
            }
        }
    }
}
