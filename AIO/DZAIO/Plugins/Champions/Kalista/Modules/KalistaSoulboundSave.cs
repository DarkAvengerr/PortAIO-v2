using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Modules;
using DZLib.MenuExtensions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Kalista.Modules
{
    class KalistaSoulboundSaver : IModule
    {
        public static AIHeroClient SoulBound { get; set; }

        public void OnLoad()
        {
            
        }

        public bool ShouldGetExecuted()
        {
            return Variables.Spells[SpellSlot.R].LSIsReady() &&
                  Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.kalista.kalista.autoRSoul");
        }

        public DZAIOEnums.ModuleType GetModuleType()
        {
            return DZAIOEnums.ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            if (SoulBound == null)
            {
                SoulBound =
                    HeroManager.Allies.Find(
                        h => h.Buffs.Any(b => b.Caster.IsMe && b.Name.Contains("kalistacoopstrikeally")));

            }

            if (SoulBound?.HealthPercent < 10
                && (SoulBound.LSCountEnemiesInRange(500) > 0))
            {
                Variables.Spells[SpellSlot.R].Cast();
            }
        }
    }
}
