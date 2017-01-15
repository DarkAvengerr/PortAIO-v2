using System;
using System.Linq;
using DZLib.Modules;
using iKalistaReborn.Utils;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iKalistaReborn.Modules
{
    class AutoELeavingModule : IModule
    {
        public void OnLoad()
        {
            Console.WriteLine("Leaving Module");
        }

        public string GetName()
        {
            return "AutoELeaving";
        }

        public bool ShouldGetExecuted()
        {
            return SpellManager.Spell[SpellSlot.E].IsReady() && Kalista.Menu.Item("com.ikalista.combo.eLeaving").GetValue<bool>();
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var target =
                HeroManager.Enemies
                    .FirstOrDefault(x => x.HasRendBuff() && SpellManager.Spell[SpellSlot.E].IsInRange(x));
            if (target == null) return;
            var damage = Math.Ceiling(Helper.GetRendDamage(target)*100/target.Health);
            if (damage >= Kalista.Menu.Item("com.ikalista.combo.ePercent").GetValue<Slider>().Value && target.ServerPosition.Distance(ObjectManager.Player.ServerPosition, true) > Math.Pow(SpellManager.Spell[SpellSlot.E].Range * 0.8, 2))
            {
                SpellManager.Spell[SpellSlot.E].Cast();
            }
        }
    }
}
