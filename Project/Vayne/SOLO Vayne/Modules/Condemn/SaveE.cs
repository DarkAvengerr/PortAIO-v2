using DZLib.Logging;
using LeagueSharp;
using LeagueSharp.Common;
using SoloVayne.Skills.Tumble;
using SoloVayne.Utility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Modules.Condemn
{
    class SaveE : ISOLOModule
    {
        /// <summary>
        /// Called when the modules is loaded.
        /// </summary>
        public void OnLoad()
        {

        }

        /// <summary>
        /// Shoulds the module get executed.
        /// </summary>
        /// <returns></returns>
        public bool ShouldGetExecuted()
        {
            return MenuExtensions.GetItemValue<bool>("solo.vayne.misc.condemn.save") 
                && Variables.spells[SpellSlot.E].IsReady() 
                && (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                && ObjectManager.Player.HealthPercent < 7;
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
        /// Called when the module is executed
        /// </summary>
        public void OnExecute()
        {
            var meleeEnemyClose = TumbleHelper.GetClosestEnemy(ObjectManager.Player.ServerPosition);
            if (meleeEnemyClose != null && meleeEnemyClose.IsMelee && meleeEnemyClose.Distance(ObjectManager.Player, true) < 350f * 350f)
            {
                var health = meleeEnemyClose.Health;
                var healthPercent = meleeEnemyClose.HealthPercent;
                if (1 / ObjectManager.Player.AttackDelay > 1.25)
                {
                    if (health >
                        ObjectManager.Player.GetAutoAttackDamage(meleeEnemyClose) * 2 +
                        Variables.spells[SpellSlot.W].GetDamage(meleeEnemyClose) || healthPercent > 30)
                    {
                        if (
                            meleeEnemyClose.GetComboDamage(
                                ObjectManager.Player,
                                new [] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R }) >
                            ObjectManager.Player.Health + 10 ||
                            meleeEnemyClose.GetAutoAttackDamage(ObjectManager.Player) * 2 >
                            ObjectManager.Player.Health + 10)
                        {
                            Variables.spells[SpellSlot.E].Cast(meleeEnemyClose);
                        }
                    }
                }
            }
        }
    }
}
