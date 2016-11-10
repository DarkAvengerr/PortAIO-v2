using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElKatarinaDecentralized.Components.Spells
{
    using System;
    using System.Linq;

    using ElKatarinaDecentralized.Enumerations;
    using ElKatarinaDecentralized.Utils;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    /// <summary>
    ///     The spell R.
    /// </summary>
    internal class SpellR : ISpell
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the damage type.
        /// </summary>
        internal override TargetSelector.DamageType DamageType => TargetSelector.DamageType.Magical;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => MyMenu.RootMenu.Item("combo.r.range").GetValue<Slider>().Value;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.R;

        /// <summary>
        ///     Cancel ult.
        /// </summary>
        private static bool CancellingUlt;

        #endregion

        #region Methods

        /// <summary>
        ///     The on combo callback.
        /// </summary>
        internal override void OnCombo()
        {
            try
            {
                if (this.SpellObject == null)
                {
                    return;
                }

                var target = Misc.GetTarget(this.Range, this.DamageType);
                if (target != null)
                {
                    switch (MyMenu.RootMenu.Item("combo.r.s").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            var enemy =
                                HeroManager.Enemies.FirstOrDefault(
                                    h => h.IsValidTarget(this.Range) && h.GetCalculatedRDamage(MyMenu.RootMenu.Item("combo.r.ticks").GetValue<Slider>().Value) > h.Health);
                            
                            if (enemy != null)
                            {
                                this.SpellObject.Cast();
                            }

                            break;

                        case 1:
                            if (target.IsValidTarget(this.Range))
                            {
                                this.SpellObject.Cast();
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellR.cs: Can not run OnCombo - {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     The on mixed callback.
        /// </summary>
        internal override void OnMixed()
        {
            this.OnCombo();
        }

        /// <summary>
        ///     The on update callback.
        /// </summary>
        internal override void OnUpdate()
        {
            var importantSpell = ObjectManager.Player.IsChannelingImportantSpell();
            if (importantSpell)
            {
                if (MyMenu.RootMenu.Item("combo.r.no.enemies").IsActive() && ObjectManager.Player.CountEnemiesInRange(550f) == 0 && !CancellingUlt)
                {
                    CancellingUlt = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(
                       300, () =>
                       {
                           CancellingUlt = false;
                           if (ObjectManager.Player.CountEnemiesInRange(550f) == 0) // use max range
                           {
                               EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, ObjectManager.Player.ServerPosition.Randomize(10, 20), false);
                               Logging.AddEntry(LoggingEntryTrype.Debug, "Cancel R");
                           }
                       });
                }
            }

            if (importantSpell)
            {
                return;
            }

            if (MyMenu.RootMenu.Item("combo.disable.evade").IsActive() && EvadeDisabler.EvadeDisabled)
            {
                EvadeDisabler.EnableEvade();
                Logging.AddEntry(LoggingEntryTrype.Debug, "@SpellR.CS: Enable Evade");
            }
        }


        #endregion
    }
}