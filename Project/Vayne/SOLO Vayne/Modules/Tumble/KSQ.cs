using DZLib.Logging;
using LeagueSharp;
using LeagueSharp.Common;
using SoloVayne.Skills.Tumble;
using SoloVayne.Utility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Modules.Condemn
{
    class KSQ : ISOLOModule
    {
        /// <summary>
        /// Called when the module is loaded.
        /// </summary>
        public void OnLoad()
        {

        }

        /// <summary>
        /// Should the module get executed.
        /// </summary>
        /// <returns></returns>
        public bool ShouldGetExecuted()
        {
            return MenuExtensions.GetItemValue<bool>("solo.vayne.misc.tumble.qks") &&
                   Variables.spells[SpellSlot.Q].IsReady();
        }

        /// <summary>
        /// Gets the type of the module.
        /// </summary>
        /// <returns></returns>
        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        /// <summary>
        /// Called when the module is executed.
        /// </summary>
        public void OnExecute()
        {
            var currentTarget = TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 260f, TargetSelector.DamageType.Physical);
            if (currentTarget.IsValidTarget())
            {

                if (currentTarget.ServerPosition.Distance(ObjectManager.Player.ServerPosition) <=
                    Orbwalking.GetRealAutoAttackRange(null))
                {
                    return;
                }

                if (HealthPrediction.GetHealthPrediction(currentTarget, (int) (250 + Game.Ping / 2f + 125f)) <
                    ObjectManager.Player.GetAutoAttackDamage(currentTarget) +
                    Variables.spells[SpellSlot.Q].GetDamage(currentTarget) &&
                    HealthPrediction.GetHealthPrediction(currentTarget, (int) (250 + Game.Ping / 2f + 125f)) > 0)
                {
                    var extendedPosition = ObjectManager.Player.ServerPosition.Extend(
                        currentTarget.ServerPosition, 300f);
                    if (extendedPosition.IsSafe() && !extendedPosition.UnderTurret(true))
                    {
                        Orbwalking.ResetAutoAttackTimer();
                        Variables.spells[SpellSlot.Q].Cast(extendedPosition);
                        TargetSelector.SetTarget(currentTarget);
                    }
                }
            }
        }
    }
}
