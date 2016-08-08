using System;
using System.Linq;
using DZLib.Logging;
using iSeriesReborn.Champions.Kalista.Skills;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.Entities;
using iSeriesReborn.Utility.ModuleHelper;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Kalista.Modules
{
    class KalistaESlow : IModule
    {
        private float LastCastTime = 0f;

        public string GetName()
        {
            return "Kalista_ESlow";
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldRun()
        {
            return Variables.spells[SpellSlot.E].LSIsReady() &&
                   MenuExtensions.GetItemValue<bool>("iseriesr.kalista.misc.useeslow");
        }

        public void Run()
        {
            var minions =
                GameObjects.EnemyMinions.Where(
                    minion =>
                        minion.LSIsValidTarget(Variables.spells[SpellSlot.E].Range) && minion.HasRend() && KalistaE.CanBeRendKilled(minion));

            var heroWithRendStack =
                        HeroManager.Enemies.Where(
                            target =>
                                target.LSIsValidTarget(Variables.spells[SpellSlot.E].Range) &&
                                target.HasRend() && target.GetRendBuff().Count >= 3).OrderByDescending(KalistaE.GetRendDamage).FirstOrDefault();

            if (heroWithRendStack != null 
                && minions.Any() 
                &&  heroWithRendStack.LSDistance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(null) * 1.4f 
                && (Environment.TickCount - LastCastTime > 250) && heroWithRendStack.HealthPercent >= 35)
            {
                Variables.spells[SpellSlot.E].Cast();
                LastCastTime = Environment.TickCount;
            }
        }
    }
}
