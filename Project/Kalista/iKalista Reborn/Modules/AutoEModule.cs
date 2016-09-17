using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iKalistaReborn.Modules
{
    using System;
    using System.Linq;

    using DZLib.Modules;

    using iKalistaReborn.Utils;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class AutoEModule : IModule
    {
        #region Public Methods and Operators

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public string GetName()
        {
            return "AutoEHarass";
        }

        public void OnExecute()
        {
            if (Kalista.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed
                || Kalista.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var enemy =
                    HeroManager.Enemies.Where(hero => hero.HasRendBuff())
                        .MinOrDefault(hero => hero.Distance(ObjectManager.Player, true));
                if (
                    !(enemy?.Distance(ObjectManager.Player, true)
                      < Math.Pow(SpellManager.Spell[SpellSlot.E].Range + 200, 2)))
                    return;
                if (
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Any(
                            x =>
                            SpellManager.Spell[SpellSlot.E].IsInRange(x) && x.HasRendBuff() && x.IsRendKillable()))
                {
                    SpellManager.Spell[SpellSlot.E].Cast();
                }
            }
        }

        public void OnLoad()
        {
            Console.WriteLine("Auto E Module Loaded");
        }

        public bool ShouldGetExecuted()
        {
            return SpellManager.Spell[SpellSlot.E].IsReady()
                   && Kalista.Menu.Item("com.ikalista.combo.autoE").GetValue<bool>();
        }

        #endregion
    }
}