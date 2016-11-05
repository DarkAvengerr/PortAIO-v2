using System;
using System.Linq;
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
    class SemiRModule : IModule
    {
        public void OnLoad()
        {
            Console.WriteLine("Manual R Module Loaded.");
        }

        public bool ShouldGetExecuted()
        {
            return Variables.Menu.Item("ezreal.misc.semimanualr").GetValue<KeyBind>().Active;
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            if (Variables.Spells[SpellSlot.R].IsReady())
            {
                var target = TargetSelector.GetTarget(2300f, TargetSelector.DamageType.Physical);

                if (target.IsValidTarget(Variables.Spells[SpellSlot.R].Range)
                    && Ezreal.CanExecuteTarget(target)
                    && ObjectManager.Player.Distance(target) >= Orbwalking.GetRealAutoAttackRange(null) * 0.80f
                    &&
                    !(target.Health + 5 <
                      ObjectManager.Player.GetAutoAttackDamage(target) * 2 +
                      Variables.Spells[SpellSlot.Q].GetDamage(target))
                    && HeroManager.Enemies.Count(m => m.Distance(target.ServerPosition) < 200f) >= Variables.Menu.Item("ezreal.combo.r.min").GetValue<Slider>().Value)
                {
                    Variables.Spells[SpellSlot.R].SPredictionCast(
                        target, target.IsMoving ? HitChance.VeryHigh : HitChance.High);
                }
            }
        }

        public string GetName()
        {
            return "SemiManualR";
        }
    }
}
