using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Entity;
using DZAIO_Reborn.Helpers.Modules;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Ahri.Modules
{
    class AhriQKS : IModule
    {
        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.ahri.extra.autoQKS") &&
                   Variables.Spells[SpellSlot.Q].LSIsReady();

        }

        public DZAIOEnums.ModuleType GetModuleType()
        {
            return DZAIOEnums.ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var target = TargetSelector.GetTarget(Variables.Spells[SpellSlot.Q].Range, TargetSelector.DamageType.Magical);

            if (target.LSIsValidTarget()
                && TargetSelector.GetPriority(target) > 1)
            {
                
                if (target.Health + 5 < Variables.Spells[SpellSlot.Q].GetDamage(target) 
                    && (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo 
                    || !Variables.Orbwalker.GetTarget().LSIsValidTarget()))
                {
                    Variables.Spells[SpellSlot.Q].CastIfHitchanceEquals(target, HitChance.High);
                }
            }
        }
    }
}
