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
    class KalistaSoulboundSaver : IModule
    {
        public string GetName()
        {
            return "Kalista_SoulboundSaver";
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldRun()
        {
            return Variables.spells[SpellSlot.E].LSIsReady() &&
                   MenuExtensions.GetItemValue<bool>("iseriesr.kalista.misc.savesoulbound");
        }

        public void Run()
        {
            //TODO More Advanced logic here with incoming damages. No Time now. Gotta go fast.
            var SoulBound = KalistaHooks.SoulBound;

            if (SoulBound?.HealthPercent < 7
                && (SoulBound.LSCountEnemiesInRange(500) > 0))
            {
                Variables.spells[SpellSlot.R].Cast();
            }
        }
    }
}
