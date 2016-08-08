using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Modules;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Kalista.Modules
{
    class KalistaEDeath : IModule
    {
        private float LastECastTime = 0f;

        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.kalista.kalista.autoEDeath") &&
                   Variables.Spells[SpellSlot.E].LSIsReady();

        }

        public DZAIOEnums.ModuleType GetModuleType()
        {
            return DZAIOEnums.ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
                if (HealthPrediction.GetHealthPrediction(ObjectManager.Player, 250) <= 0 
                    || 
                    (ObjectManager.Player.HealthPercent < 5 
                    && ObjectManager.Player.LSGetEnemiesInRange(Orbwalking.GetRealAutoAttackRange(null)).Any(h => h.HealthPercent > 4 * ObjectManager.Player.HealthPercent))
                    )
                {
                    
                    if ((Environment.TickCount - LastECastTime >= 250f) 
                        && HeroManager.Enemies.Any(m => m.LSIsValidTarget(Variables.Spells[SpellSlot.E].Range) && m.HasRend())
                        )
                    {
                        Variables.Spells[SpellSlot.E].Cast();
                        LastECastTime = Environment.TickCount;
                    }
                }
            }
        }
}
