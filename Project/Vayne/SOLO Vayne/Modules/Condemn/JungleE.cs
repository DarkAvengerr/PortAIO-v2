using System.Linq;
using DZLib.Logging;
using LeagueSharp;
using LeagueSharp.Common;
using SoloVayne.Utility;
using SoloVayne.Utility.Entities;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Modules.Condemn
{
    class JungleE : ISOLOModule
    {
        /// <summary>
        /// Called when the module gets loaded.
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
            return MenuExtensions.GetItemValue<bool>("solo.vayne.laneclear.condemn.jungle") 
                && Variables.spells[SpellSlot.E].IsReady() 
                && (Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                && ObjectManager.Player.ManaPercent >= 40;
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
        /// Called when the module gets executed.
        /// </summary>
        public void OnExecute()
        {
            var currentTarget = Variables.Orbwalker.GetTarget();

            if (currentTarget is Obj_AI_Minion && GameObjects.JungleLarge.Contains(currentTarget))
            {
                for (int i = 0; i < 450; i += 65)
                {
                    var endPos = currentTarget.Position.Extend(ObjectManager.Player.ServerPosition, -i);

                    if (endPos.IsWall())
                    {
                        Variables.spells[SpellSlot.E].Cast(currentTarget as Obj_AI_Base);
                        return;
                    }
                }
            }
        }
    }
}
