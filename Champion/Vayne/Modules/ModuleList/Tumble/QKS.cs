using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Skills.Tumble;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;

namespace VayneHunter_Reborn.Modules.ModuleList.Tumble
{
    class QKS : IModule
    {
        public void OnLoad()
        {
           
        }

        public bool ShouldGetExecuted()
        {
            return MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.tumble.qinrange") && Variables.spells[SpellSlot.Q].LSIsReady();
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
                var currentTarget = TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 240f, TargetSelector.DamageType.Physical);
                if (!currentTarget.LSIsValidTarget())
                {
                    return;
                }

                if (currentTarget.ServerPosition.LSDistance(ObjectManager.Player.ServerPosition) <=
                    Orbwalking.GetRealAutoAttackRange(null))
                {
                    return;
                }

                if (HealthPrediction.GetHealthPrediction(currentTarget, (int) (250 + Game.Ping / 2f)) <
                    ObjectManager.Player.LSGetAutoAttackDamage(currentTarget) +
                    Variables.spells[SpellSlot.Q].GetDamage(currentTarget)
                    && HealthPrediction.GetHealthPrediction(currentTarget, (int)(250 + Game.Ping / 2f)) > 0)
                {
                    var extendedPosition = ObjectManager.Player.ServerPosition.LSExtend(
                        currentTarget.ServerPosition, 300f);
                    if (extendedPosition.IsSafe())
                    {
                        Orbwalking.ResetAutoAttackTimer();
                        Variables.spells[SpellSlot.Q].Cast(extendedPosition);
                        TargetSelector.SetTarget(currentTarget);
                    }
                }
        }
    }
}
