using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZLib.Modules;
using iKalistaReborn.Utils;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iKalistaReborn.Modules
{
    internal class AutoRendModule : IModule
    {
        public string GetName()
        {
            return "AutoRend";
        }

        public bool ShouldGetExecuted()
        {
            return SpellManager.Spell[SpellSlot.E].IsReady() &&
                   Kalista.Menu.Item("com.ikalista.combo.useE").GetValue<bool>();
        }

        public void OnExecute()
        {
            foreach (
                var source in
                    HeroManager.Enemies.Where(
                        x => x.IsValid && x.HasRendBuff() && SpellManager.Spell[SpellSlot.E].IsInRange(x)))
            {
                if (source.IsRendKillable())
                {
                    SpellManager.Spell[SpellSlot.E].Cast();
                }
            }
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnLoad()
        {
            Console.WriteLine("Auto Rend Module Loaded");
        }
    }
}