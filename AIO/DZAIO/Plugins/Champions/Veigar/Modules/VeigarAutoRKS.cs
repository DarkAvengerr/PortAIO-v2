using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Entity;
using DZAIO_Reborn.Helpers.Modules;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Veigar.Modules
{
    class VeigarAutoRKS : IModule
    {
        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.veigar.extra.autoRKS") &&
                   Variables.Spells[SpellSlot.R].LSIsReady();

        }

        public DZAIOEnums.ModuleType GetModuleType()
        {
            return DZAIOEnums.ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var target = TargetSelector.GetTarget(Variables.Spells[SpellSlot.R].Range, TargetSelector.DamageType.Magical);

            if (target.LSIsValidTarget() 
                && TargetSelector.GetPriority(target) > 1 )
            {
                if (target.Health + 5 < Variables.Spells[SpellSlot.R].GetDamage(target))
                {
                    Variables.Spells[SpellSlot.R].Cast(target);
                }
                else
                {
                    var nearlyKillableTarget = DZTargetHelper.GetNearlyKillableTarget(Variables.Spells[SpellSlot.R],
                        new[] { SpellSlot.Q }, TargetSelector.DamageType.Magical);
                    if (nearlyKillableTarget.LSIsValidTarget(Variables.Spells[SpellSlot.Q].Range)
                        && nearlyKillableTarget.LSIsValidTarget(Variables.Spells[SpellSlot.R].Range)
                        && TargetSelector.GetPriority(target) > 1)
                    {
                        var QPrediction = Variables.Spells[SpellSlot.Q].GetPrediction(nearlyKillableTarget);

                        if (QPrediction.Hitchance >= HitChance.High)
                        {
                            Variables.Spells[SpellSlot.Q].Cast(QPrediction.CastPosition);
                            Variables.Spells[SpellSlot.R].Cast(nearlyKillableTarget);
                        }
                    }
                }
            }
        }
    }
}
