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
    internal class JungleStealModule : IModule
    {
        public void OnLoad()
        {
            Console.WriteLine("Jungle Steal Module Loaded");
        }

        public string GetName()
        {
            return "JungleSteal";
        }

        public bool ShouldGetExecuted()
        {
            return SpellManager.Spell[SpellSlot.E].IsReady()
                   && Kalista.Menu.Item("com.ikalista.jungleSteal.enabled").GetValue<bool>();
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var small =
                GameObjects.JungleSmall.Any(
                    x => SpellManager.Spell[SpellSlot.E].CanCast(x) && x.IsMobKillable() && x.IsValid);
            var large =
                GameObjects.JungleLarge.Any(
                    x => SpellManager.Spell[SpellSlot.E].CanCast(x) && x.IsMobKillable() && x.IsValid);
            var legendary =
                GameObjects.JungleLegendary.Any(
                    x => SpellManager.Spell[SpellSlot.E].CanCast(x) && x.IsMobKillable() && x.IsValid);

            if ((small && Kalista.Menu.Item("com.ikalista.jungleSteal.small").GetValue<bool>())
                || (large && Kalista.Menu.Item("com.ikalista.jungleSteal.large").GetValue<bool>())
                || (legendary && Kalista.Menu.Item("com.ikalista.jungleSteal.legendary").GetValue<bool>()))
            {
                SpellManager.Spell[SpellSlot.E].Cast();
            }
        }
    }
}