using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Entity;
using DZAIO_Reborn.Helpers.Modules;
using LeagueSharp;
using LeagueSharp.Common;
using Geometry = LeagueSharp.Common.Geometry;
using Variables = DZAIO_Reborn.Core.Variables;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Sivir.Modules
{
    class EzrealQRoot : IModule
    {
        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.ezreal.extra.autoQRoot") &&
                   Variables.Spells[SpellSlot.Q].LSIsReady();

        }

        public DZAIOEnums.ModuleType GetModuleType()
        {
            return DZAIOEnums.ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var heroTarget = HeroManager.Enemies.Where(h => h.LSIsValidTarget(Variables.Spells[SpellSlot.Q].Range) && (h.IsHeavilyImpaired() || h.IsLightlyImpaired()))
                .OrderBy(h => Variables.Spells[SpellSlot.Q].GetDamage(h))
                .LastOrDefault();

            if (heroTarget != null
                && ObjectManager.Player.LSDistance(heroTarget) <= Orbwalking.GetRealAutoAttackRange(heroTarget) * 1.3f)
            {
                Variables.Spells[SpellSlot.Q].Cast();
            }
        }
    }
}
