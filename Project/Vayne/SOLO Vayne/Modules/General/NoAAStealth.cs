using System;
using System.Linq;
using DZLib.Logging;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Modules.General
{
    class NoAAStealth : ISOLOModule
    {

        /// <summary>
        /// Called when the module is loaded.
        /// </summary>
        public void OnLoad()
        {
            Orbwalking.BeforeAttack += BeforeAttack;
        }

        /// <summary>
        /// Shoulds the module get executed.
        /// </summary>
        /// <returns></returns>
        public bool ShouldGetExecuted()
        {
            return ObjectManager.Player.GetEnemiesInRange(1200f).Count() > 1 
                && ObjectManager.Player.HasBuff("vaynetumblefade")
                && MenuExtensions.GetItemValue<bool>("solo.vayne.misc.miscellaneous.noaastealth")
                && !ObjectManager.Player.UnderTurret(true)
                && !(ObjectManager.Get<Obj_AI_Base>().Count(m => string.Equals(m.CharData.BaseSkinName, "VisionWard", StringComparison.CurrentCultureIgnoreCase) && m.IsValidTarget(650f)) > 0);
        }

        /// <summary>
        /// Gets the type of the module.
        /// </summary>
        /// <returns></returns>
        public ModuleType GetModuleType()
        {
            return ModuleType.Other;
        }

        /// <summary>
        /// Called when the module gets executed.
        /// </summary>
        public void OnExecute() { }

        /// <summary>
        /// Called Before the orbwalker attacks.
        /// </summary>
        /// <param name="args">The <see cref="Orbwalking.BeforeAttackEventArgs"/> instance containing the event data.</param>
        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (ShouldGetExecuted())
            {
                args.Process = false;
            }
        }
    }
}
