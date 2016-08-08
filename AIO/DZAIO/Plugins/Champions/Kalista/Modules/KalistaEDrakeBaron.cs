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
    class KalistaEDrakeBaron : IModule
    {
        private float LastECastTime = 0f;

        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.kalista.kalista.autoESteal") &&
                   Variables.Spells[SpellSlot.E].LSIsReady();

        }

        public DZAIOEnums.ModuleType GetModuleType()
        {
            return DZAIOEnums.ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var baron =
                   MinionManager.GetMinions(
                       ObjectManager.Player.ServerPosition,
                       Variables.Spells[SpellSlot.E].Range,
                       MinionTypes.All,
                       MinionTeam.Neutral,
                       MinionOrderTypes.MaxHealth)
                       .FirstOrDefault(
                           x => x.LSIsValidTarget() && HealthPrediction.GetHealthPrediction(x, 250) + 5 < Variables.Spells[SpellSlot.E].GetBaronReduction(x) && x.Name.Contains("Baron"));

            var dragon =
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    Variables.Spells[SpellSlot.E].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(
                        x => x.LSIsValidTarget() && HealthPrediction.GetHealthPrediction(x, 250) + 5 < Variables.Spells[SpellSlot.E].GetDragonReduction(x) && x.Name.Contains("Dragon"));

            if (((dragon.LSIsValidTarget() && Variables.Spells[SpellSlot.E].CanCast(dragon))
               || (baron.LSIsValidTarget() && Variables.Spells[SpellSlot.E].CanCast(baron)))
               && (Environment.TickCount - LastECastTime >= 500))
            {
                Variables.Spells[SpellSlot.E].Cast();
                LastECastTime = Environment.TickCount;
            }
        
        }
    }
}
