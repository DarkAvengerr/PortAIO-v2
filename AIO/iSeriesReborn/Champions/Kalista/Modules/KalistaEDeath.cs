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
 namespace iSeriesReborn.Champions.Kalista.Modules
{
    class KalistaEDeath : IModule
    {
        private float LastCastTime = 0f;

        public string GetName()
        {
            return "Kalista_EDeath";
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldRun()
        {
            return Variables.spells[SpellSlot.E].IsReady() &&
                    MenuExtensions.GetItemValue<bool>("iseriesr.kalista.misc.edeath");
        }

        public void Run()
        {
            var skillshots = EvadeHelper.EvadeDetectedSkillshots.Where(
                skillshot =>
                    skillshot.SpellData.IsDangerous
                    && skillshot.SpellData.DangerValue >= 3
                    && skillshot.IsAboutToHit(250, ObjectManager.Player.ServerPosition)).ToList();

            if (Variables.spells[SpellSlot.E].IsReady())
            {
                 if (skillshots.Any(
                     skillshot =>
                         skillshot.Caster.GetSpellDamage(ObjectManager.Player, skillshot.SpellData.SpellName) >=
                         HealthPrediction.GetHealthPrediction(ObjectManager.Player, 250) + 5))
                 {
                     if ((Environment.TickCount - LastCastTime > 250))
                     {
                         Variables.spells[SpellSlot.E].Cast();
                         LastCastTime = Environment.TickCount;
                         return;
                     }
                }

                if (HealthPrediction.GetHealthPrediction(ObjectManager.Player, 300) <= 0 || ObjectManager.Player.HealthPercent < 8)
                {
                    if ((Environment.TickCount - LastCastTime > 250))
                    {
                        Variables.spells[SpellSlot.E].Cast();
                        LastCastTime = Environment.TickCount;
                    }
                }
            }
        }
    }
}
