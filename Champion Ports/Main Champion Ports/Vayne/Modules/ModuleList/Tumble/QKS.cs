using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Skills.Tumble;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace VayneHunter_Reborn.Modules.ModuleList.Tumble
{
    class QKS : IModule
    {
        public void OnLoad()
        {
           
        }

        public bool ShouldGetExecuted()
        {
            return MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.tumble.qinrange") && Variables.spells[SpellSlot.Q].IsReady();
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
                var currentTarget = TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 240f, TargetSelector.DamageType.Physical);

                if (!currentTarget.IsValidTarget() || currentTarget.ServerPosition.Distance(ObjectManager.Player.ServerPosition) <=
                    Orbwalking.GetRealAutoAttackRange(null))
                {
                    return;
                }

                if (HealthPrediction.GetHealthPrediction(currentTarget, (int) (280f)) <
                    ObjectManager.Player.GetAutoAttackDamage(currentTarget) +
                    Variables.spells[SpellSlot.Q].GetDamage(currentTarget)
                    && HealthPrediction.GetHealthPrediction(currentTarget, (int)(280f)) > 0)
                {
                    var extendedPosition = ObjectManager.Player.ServerPosition.Extend(
                        currentTarget.ServerPosition, 300f);
                    if (extendedPosition.IsGoodEndPosition())
                    {
                        Variables.spells[SpellSlot.Q].Cast(extendedPosition);
                        //TargetSelector.SetTarget(currentTarget);
                    }
                }
        }
    }
}
