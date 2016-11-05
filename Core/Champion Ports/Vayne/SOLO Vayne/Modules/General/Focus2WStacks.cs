using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SoloVayne.Utility;
using SoloVayne.Utility.Entities;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Modules.General
{
    class Focus2WStacks : ISOLOModule
    {
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
            return
                HeroManager.Enemies.Any(
                    m => m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 200) && m.Has2WStacks()) 
                    && !HeroManager.Enemies.Any(m => m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 200) && m.Health + 15 < ObjectManager.Player.GetAutoAttackDamage(m) * 3 + Variables.spells[SpellSlot.W].GetDamage(m));
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
            TargetSelector.SetTarget(HeroManager.Enemies.FirstOrDefault(
                    m => m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 200) && m.Has2WStacks()));
        }
    }
}
