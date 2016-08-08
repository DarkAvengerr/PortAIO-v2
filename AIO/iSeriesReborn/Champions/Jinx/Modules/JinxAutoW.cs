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
    class JinxAutoW : IModule
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
            return Variables.spells[SpellSlot.W].LSIsReady() &&
                    MenuExtensions.GetItemValue<bool>("iseriesr.jinx.w.auto") && ObjectManager.Player.ManaPercent > 35;
        }

        public void Run()
        {
            var selectedTarget = TargetSelector.GetTarget(Variables.spells[SpellSlot.W].Range * 0.70f, TargetSelector.DamageType.Physical);
            var WSpell = Variables.spells[SpellSlot.W];

            if (selectedTarget.LSIsValidTarget())
            {
                //The selected target is valid.
                if (selectedTarget.HasBuffOfType(BuffType.Slow) && selectedTarget.Path.Count() > 1)
                {
                    //Target is slowed.
                    var slowEndTime = JinxUtility.GetSlowEndTime(selectedTarget);
                    if (slowEndTime >= WSpell.Delay + 0.5f + Game.Ping / 2f)
                    {
                        WSpell.CastIfHitchanceEquals(selectedTarget, HitChance.VeryHigh);
                    }
                }
                else if (JinxUtility.IsHeavilyImpaired(selectedTarget))
                {
                    //The target is actually impaired heavily. Let's cast E on them.
                    var immobileEndTime = JinxUtility.GetImpairedEndTime(selectedTarget);
                    if (immobileEndTime >= WSpell.Delay + 0.5f + Game.Ping / 2f)
                    {
                        WSpell.CastIfHitchanceEquals(selectedTarget, HitChance.VeryHigh);
                    }
                }
            }
        }
    }
}
