using System.Linq;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Entity;
using DZAIO_Reborn.Helpers.Modules;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Bard.Modules
{
    class BardAutoQ : IModule
    {
        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return (Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.bard.extra.autoQKS")
                || Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.bard.extra.autoQ"))
                && Variables.Spells[SpellSlot.Q].LSIsReady();

        }

        public DZAIOEnums.ModuleType GetModuleType()
        {
            return DZAIOEnums.ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var target = TargetSelector.GetTarget(Variables.Spells[SpellSlot.Q].Range, TargetSelector.DamageType.Magical);

            if (target.LSIsValidTarget()
                && TargetSelector.GetPriority(target) > 1 && Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.bard.extra.autoQKS"))
            {

                if (target.Health + 5 < Variables.Spells[SpellSlot.Q].GetDamage(target)
                    && (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                    || !Variables.Orbwalker.GetTarget().LSIsValidTarget()))
                {
                    Variables.Spells[SpellSlot.Q].CastIfHitchanceEquals(target, HitChance.High);
                }
            }

            var target_ex =
                HeroManager.Enemies.FirstOrDefault(m => m.LSIsValidTarget(Variables.Spells[SpellSlot.Q].Range) && m.IsHeavilyImpaired());

            if (target_ex != null && Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.bard.extra.autoQ"))
            {
                Variables.Spells[SpellSlot.Q].CastIfHitchanceEquals(target, HitChance.High);
            }
        }
    }
}