using System;
using DZLib.Modules;
using iDZEzreal.MenuHelper;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SPrediction;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iDZEzreal.Modules
{
    class AutoHarassModule : IModule
    {
        public void OnLoad()
        {
            Console.WriteLine("Auto Harass Module Loaded.");
        }

        public bool ShouldGetExecuted()
        {
            return Variables.Menu.Item("ezreal.modules." + GetName().ToLowerInvariant()).GetValue<bool>();
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            if (Variables.Spells[SpellSlot.Q].IsReady() && Variables.Menu.Item("ezreal.mixed.q").GetValue<bool>())
            {
                var target = TargetSelector.GetTargetNoCollision(Variables.Spells[SpellSlot.Q]);
                if (target.IsValidTarget(Variables.Spells[SpellSlot.Q].Range))
                {
                    var prediction = Variables.Spells[SpellSlot.Q].GetSPrediction(target);
                    var castPosition = prediction.CastPosition.Extend((Vector2)ObjectManager.Player.Position, -140);
                    if (prediction.HitChance >= MenuGenerator.GetHitchance())
                    {
                        Variables.Spells[SpellSlot.Q].Cast(castPosition);
                    }
                }
            }

            if (Variables.Spells[SpellSlot.W].IsReady() && Variables.Menu.Item("ezreal.mixed.w").GetValue<bool>() && ObjectManager.Player.ManaPercent > 35)
            {
                var wTarget = TargetSelector.GetTargetNoCollision(Variables.Spells[SpellSlot.W]);
                if (wTarget.IsValidTarget(Variables.Spells[SpellSlot.W].Range)
                    && Variables.Spells[SpellSlot.W].GetSPrediction(wTarget).HitChance >= MenuGenerator.GetHitchance())
                {
                    Variables.Spells[SpellSlot.W].Cast(Variables.Spells[SpellSlot.W].GetSPrediction(wTarget).CastPosition);
                }
            }
        }

        public string GetName()
        {
            return "AutoHarass";
        }
    }
}
