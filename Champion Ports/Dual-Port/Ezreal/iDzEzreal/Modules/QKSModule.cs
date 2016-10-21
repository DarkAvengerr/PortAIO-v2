using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZLib.Modules;
using iDZEzreal.MenuHelper;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iDZEzreal.Modules
{
    public class QKSModule : IModule
    {
        public void OnLoad()
        {
            Console.WriteLine("QKS Module Loaded");
        }

        public bool ShouldGetExecuted()
        {
            return Variables.Spells[SpellSlot.Q].IsReady() && Variables.Menu.Item("ezreal.modules." + GetName().ToLowerInvariant()).GetValue<bool>();
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            foreach (
                var enemy in HeroManager.Enemies.Where(m => m.Health + 5 <= Variables.Spells[SpellSlot.Q].GetDamage(m) && m.IsValidTarget(Variables.Spells[SpellSlot.Q].Range)))
            {
                var sPrediction = Variables.Spells[SpellSlot.Q].GetSPrediction(enemy);
                if (sPrediction.HitChance >= HitChance.High)
                {
                    Variables.Spells[SpellSlot.Q].Cast(sPrediction.CastPosition);
                }
            }
        }

        public string GetName()
        {
            return "QKS";
        }
    }
}