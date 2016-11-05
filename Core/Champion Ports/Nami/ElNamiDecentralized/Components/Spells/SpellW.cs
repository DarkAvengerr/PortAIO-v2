namespace ElNamiDecentralized.Components.Spells
{
    using System;
    using System.Linq;

    using ElNamiDecentralized.Enumerations;
    using ElNamiDecentralized.Utils;

    using EloBuddy;
    using LeagueSharp.Common;

    using SharpDX;

    /// <summary>
    ///     The spell W.
    /// </summary>
    internal class SpellW : ISpell
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the damage type.
        /// </summary>
        internal override TargetSelector.DamageType DamageType => TargetSelector.DamageType.Magical;

        /// <summary>
        ///     Gets the delay.
        /// </summary>
        internal override float Delay => 250f;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 725f;

        /// <summary>
        ///     Gets or sets the Targeted type.
        /// </summary>
        internal override bool Targeted => true;

        /// <summary>
        ///     Gets the speed.
        /// </summary>
        internal override float Speed => 2500f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.W;


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
                    this.SpellObject.CastOnUnit(target);
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellW.cs: Can not run OnCombo - {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     The on update callback.
        /// </summary>
        internal override void OnUpdate()
        {
            if (!this.SpellSlot.IsReady())
            {
                return;
            }

            if (MyMenu.RootMenu.Item("heal.allies").IsActive())
            {
                var lowHealthAlly =
                    HeroManager.Allies.Where(
                            a =>
                                a.Distance(ObjectManager.Player) < this.Range && !a.IsDead && !a.IsZombie
                                && !a.IsRecalling()
                                && a.HealthPercent
                                <= MyMenu.RootMenu.Item("allies.healthpercantage").GetValue<Slider>().Value)
                        .OrderByDescending(h => (h.PhysicalDamageDealtPlayer + h.MagicDamageDealtPlayer))
                        .FirstOrDefault();

                if (lowHealthAlly != null)
                {
                    this.SpellObject.CastOnUnit(lowHealthAlly);
                }
            }
        }

        /// <summary>
        ///     The on mixed callback.
        /// </summary>
        internal override void OnMixed()
        {
            var target = Misc.GetTarget(this.Range, this.DamageType);
            if (target != null)
            {
                if (MyMenu.RootMenu.Item("harass.mode").GetValue<StringList>().SelectedIndex == 0)
                {
                    var alliesCount =
                        HeroManager.Allies.Count(
                            a =>
                                a.IsValid && !a.IsDead && !a.IsMe
                                && a.Distance(
                                    ObjectManager.Player.Position.Extend(
                                        target.Position,
                                        ObjectManager.Player.Distance(target) / 2f)) <= this.Range);

                    // There is more then 1 ally in range of the enemy position so that means that casting W on the enemy will bounce and heal the ally.
                    if (alliesCount > 0 && target.Distance(ObjectManager.Player) <= this.Range)
                    {
                        // cast W on target
                        this.SpellObject.CastOnUnit(target);
                    }

                    // if there are no allies in range and the enemy health percentage is below the set percentage, start casting W on target.
                    if ((alliesCount == 0) && target.HealthPercent <= MyMenu.RootMenu.Item("smart.harass.health").GetValue<Slider>().Value)
                    {
                        this.SpellObject.CastOnUnit(target);
                    }
                }
                else
                {
                    // YOLO, just go whenever the target is in range.
                    if (target.Distance(ObjectManager.Player) <= this.Range)
                    {
                        this.SpellObject.CastOnUnit(target);
                    }
                }
            }
        }

        #endregion
    }
}