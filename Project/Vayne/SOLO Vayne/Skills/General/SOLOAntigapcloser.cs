using DZLib.Logging;
using LeagueSharp;
using LeagueSharp.Common;
using SoloVayne.Utility;
using SOLOVayne.Utility.General;
using ActiveGapcloser = SOLOVayne.Utility.General.ActiveGapcloser;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Skills.General
{
    class SOLOAntigapcloser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SOLOAntigapcloser"/> class.
        /// </summary>
        public SOLOAntigapcloser()
        {
            CustomAntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptable;
        }

        /// <summary>
        /// Called when an interruptable skill is casted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Interrupter2.InterruptableTargetEventArgs"/> instance containing the event data.</param>
        private void OnInterruptable(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            var interrupterEnabled = MenuExtensions.GetItemValue<bool>("solo.vayne.misc.miscellaneous.interrupter");

            if (!interrupterEnabled
                || !Variables.spells[SpellSlot.E].IsReady()
                || !sender.IsValidTarget())
            {
                return;
            }

            if (args.DangerLevel == Interrupter2.DangerLevel.High)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(MenuExtensions.GetItemValue<Slider>("solo.vayne.misc.miscellaneous.delay").Value,
                    () =>
                    {
                            Variables.spells[SpellSlot.E].Cast(sender);
                    });
            }
        }

        /// <summary>
        /// Called when an enemy gapcloser is casted on the player.
        /// </summary>
        /// <param name="gapcloser">The gapcloser.</param>
        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var antigapcloserEnabled = MenuExtensions.GetItemValue<bool>("solo.vayne.misc.miscellaneous.antigapcloser");
            var endPosition = gapcloser.End;

            if (!antigapcloserEnabled || !Variables.spells[SpellSlot.E].IsReady() || !gapcloser.Sender.IsValidTarget() ||
                ObjectManager.Player.Distance(endPosition) > 400)
            {
                return;
            }

            //Smart
            var ShouldBeRepelled = CustomAntiGapcloser.SpellShouldBeRepelledOnSmartMode(gapcloser.SData.Name);

            if (ShouldBeRepelled)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(MenuExtensions.GetItemValue<Slider>("solo.vayne.misc.miscellaneous.delay").Value,
                    () =>
                    {
                            Variables.spells[SpellSlot.E].Cast(gapcloser.Sender);
                    });
            }
            else
            {
                //Use Q
                var extendedPosition = ObjectManager.Player.ServerPosition.Extend(endPosition, -300f);
                if (!extendedPosition.UnderTurret(true) &&
                    !(extendedPosition.CountEnemiesInRange(400f) >= 2 && extendedPosition.CountAlliesInRange(400f) < 3))
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(MenuExtensions.GetItemValue<Slider>("solo.vayne.misc.miscellaneous.delay").Value,
                    () =>
                    {
                            Variables.spells[SpellSlot.Q].Cast(extendedPosition);
                    });
                }
            }
        }
    }
    
}
