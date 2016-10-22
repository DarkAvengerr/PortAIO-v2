using System;
using System.Linq;
using DZLib.Logging;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SoloVayne.Skills.Tumble;
using SoloVayne.Utility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Modules.General
{
    class AutoR : ISOLOModule
    {
        private TumbleLogicProvider Provider;

        /// <summary>
        /// Called when the module is loaded.
        /// </summary>
        public void OnLoad()
        {
            Provider = new TumbleLogicProvider();
            Obj_AI_Base.OnSpellCast += OnSpellCast;
        }

        /// <summary>
        /// Shoulds the module get executed.
        /// </summary>
        /// <returns></returns>
        public bool ShouldGetExecuted()
        {
            return ObjectManager.Player.GetEnemiesInRange(2300f).Count(en => en.IsValidTarget() && !(en.HealthPercent < 5)) >= 2;
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
        /// Called when the sender is done doing the windup time for a spell/AA
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs"/> instance containing the event data.</param>
        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (sender.IsMe && args.Slot == SpellSlot.R)
                {
                    var QPosition = Provider.GetSOLOVayneQPosition();
                    if (QPosition != Vector3.Zero)
                    {
                        Variables.spells[SpellSlot.Q].Cast(QPosition);
                        return;
                    }

                    var secondaryQPosition = ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 300f);

                    if (!secondaryQPosition.UnderTurret(true))
                    {
                        Variables.spells[SpellSlot.Q].Cast(secondaryQPosition);
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.AddToLog(new LogItem("AutoQifR", e, LogSeverity.Error));
            }
            
        }
    }
}
