using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Modules;
using LeagueSharp;
using LeagueSharp.Common;
using Geometry = LeagueSharp.Common.Geometry;
using Variables = DZAIO_Reborn.Core.Variables;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Kalista.Modules
{
    class KalistaESlow : IModule
    {
        private float LastECastTime = 0f;

        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.kalista.kalista.autoESlow") &&
                   Variables.Spells[SpellSlot.E].IsReady();

        }

        public DZAIOEnums.ModuleType GetModuleType()
        {
            return DZAIOEnums.ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var killableMinions =
                LeagueSharp.SDK.GameObjects.EnemyMinions.Where(
                    m =>
                        m.IsValidTarget(Variables.Spells[SpellSlot.E].Range)
                        && m.HasRend()
                        && m.CanBeRendKilled());

            var rendHero = HeroManager.Enemies.Where(h => h.IsValidTarget(Variables.Spells[SpellSlot.E].Range) 
                && h.HasRend()
                && h.GetRendBuff().Count >= Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.kalista.combo.e.stacks").Value)
                .OrderBy(KalistaHelper.GetRendDamage)
                .LastOrDefault();

            if (Environment.TickCount - LastECastTime >= 250f
                && rendHero != null
                && killableMinions.Any()
                && rendHero.HealthPercent >= 35
                && ObjectManager.Player.Distance(rendHero) <= Orbwalking.GetRealAutoAttackRange(rendHero) * 1.1f)
            {
                Variables.Spells[SpellSlot.E].Cast();
                LastECastTime = Environment.TickCount;
            }
        }
    }
}
