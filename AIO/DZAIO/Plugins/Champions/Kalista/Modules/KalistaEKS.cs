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
    class KalistaEKS : IModule
    {
        private float LastECastTime = 0f;

        public void OnLoad()
        {
           
        }

        public bool ShouldGetExecuted()
        {
            return Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.kalista.kalista.autoEKS") &&
                   Variables.Spells[SpellSlot.E].LSIsReady();

        }

        public DZAIOEnums.ModuleType GetModuleType()
        {
            return DZAIOEnums.ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var target = HeroManager.Enemies.FirstOrDefault(enemy => enemy.CanBeRendKilled() && enemy.LSIsValidTarget(Variables.Spells[SpellSlot.E].Range));

            if (target != null 
                && (Environment.TickCount - LastECastTime >= 250f)
                && (target.NetworkId != Variables.Orbwalker.GetTarget().NetworkId))
            {
                Variables.Spells[SpellSlot.E].Cast();
                LastECastTime = Environment.TickCount;
            }
        }
    }
}
