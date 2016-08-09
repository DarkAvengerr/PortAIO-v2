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
 namespace iSeriesReborn.Champions.Jinx.Modules
{
    class JinxAutoE : IModule
    {
        public string GetName()
        {
            return "Jinx_AutoE";
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldRun()
        {
            return Variables.spells[SpellSlot.E].IsReady() &&
                    MenuExtensions.GetItemValue<bool>("iseriesr.jinx.e.auto") && ObjectManager.Player.ManaPercent > 35;
        }

        public void Run()
        {
            var selectedTarget = TargetSelector.GetTarget(Variables.spells[SpellSlot.E].Range * 0.75f, TargetSelector.DamageType.Physical);
            var ESpell = Variables.spells[SpellSlot.E];

            if (selectedTarget.IsValidTarget())
            {
                //The selected target is valid.
                if (selectedTarget.HasBuffOfType(BuffType.Slow) && selectedTarget.Path.Count() > 1)
                {
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
                }
            }
        }
    }
}
