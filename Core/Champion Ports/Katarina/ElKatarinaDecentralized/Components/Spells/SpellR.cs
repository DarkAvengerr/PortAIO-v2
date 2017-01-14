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
        private static bool _cancellingUlt;

        /// <summary>
        ///     Last stealth ult.
        /// </summary>
        private static int _lastStealthedUlt;

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
                                if (Misc.SpellW.SpellObject.IsReady() && target.IsValidTarget(this.Range - 150)) // just check melee range
                                {
                                    Misc.SpellW.SpellObject.Cast();
                                }
                                else if (Misc.SpellW.SpellObject.IsReady())
                                {
                                    Misc.SpellQ.SpellObject.CastOnUnit(target);
                                }
                                else
                                {
                                    this.SpellObject.Cast();
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellR.cs: Can not run OnCombo - {0}", e);
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
            if (MyMenu.RootMenu.Item("combo.stealth").IsActive() && this.SpellObject.IsReady() && ObjectManager.Player.CountEnemiesInRange(this.Range) == 0 
                && this.SpellObject.Cast())
            {
                _lastStealthedUlt = Utils.TickCount;
                return;
            }

            var importantSpell = ObjectManager.Player.IsChannelingImportantSpell();
            if (importantSpell)
            {
                if (MyMenu.RootMenu.Item("combo.r.no.enemies").IsActive() && ObjectManager.Player.CountEnemiesInRange(this.Range + 379) == 0 
                    && !_cancellingUlt && Utils.TickCount - _lastStealthedUlt > 2500)
                {
                    _cancellingUlt = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(
                       300, () =>
                       {
                           _cancellingUlt = false;
                           if (ObjectManager.Player.CountEnemiesInRange(550f) == 0)
                           {
                               EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, ObjectManager.Player.ServerPosition.Randomize(10, 20), false);
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
            }
        }


        #endregion
    }
}