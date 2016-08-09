using System;
using System.Linq;
using DZLib.Logging;
using iSeriesReborn.Champions.Kalista.Skills;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.ModuleHelper;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Kalista.Modules
{
    class TwitchEKS : IModule
    {
        private float LastCastTime = 0f;

        public string GetName()
        {
            return "Twitch_EKS";
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldRun()
        {
            return Variables.spells[SpellSlot.E].IsReady() &&
                   MenuExtensions.GetItemValue<bool>("iseriesr.twitch.misc.kse");
        }

        public void Run()
        {
            var killableVenomTarget = HeroManager.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(Variables.spells[SpellSlot.E].Range) && Variables.spells[SpellSlot.E].GetDamage(enemy) > enemy.Health + 15);

            if (killableVenomTarget != null && (killableVenomTarget.NetworkId != Variables.Orbwalker.GetTarget().NetworkId) && (Environment.TickCount - LastCastTime > 250))
            {
                Variables.spells[SpellSlot.E].Cast();
                LastCastTime = Environment.TickCount;
            }

        }
    }
}
