using DZLib.Logging;
using LeagueSharp;
using LeagueSharp.Common;
using SoloVayne.Skills.Condemn;
using SoloVayne.Utility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Modules.Condemn
{
    class AutoE : ISOLOModule
    {
        private CondemnLogicProvider MyProvider = new CondemnLogicProvider();

        /// <summary>
        /// Called when the module is loaded.
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
            return MenuExtensions.GetItemValue<bool>("solo.vayne.misc.condemn.autoe") &&
                   Variables.spells[SpellSlot.E].IsReady() && (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo);
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
        /// Called when the modules gets executed.
        /// </summary>
        public void OnExecute()
        {
            var target = MyProvider.GetTarget();
            if (target.IsValidTarget())
            {
                Variables.spells[SpellSlot.E].Cast(target);
            }
        }
    }
}
