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
    ///     The spell E.
    /// </summary>
    internal class SpellE : ISpell
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
        internal override float Range => 800f;

        /// <summary>
        ///     Gets or sets the Targeted type.
        /// </summary>
        internal override bool Targeted => true;

        /// <summary>
        ///     Gets the speed.
        /// </summary>
        internal override float Speed => 0f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.E;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width => 0f;

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
                    var closeAlly =
                        HeroManager.Allies.Where(a => this.SpellObject.IsInRange(a) && !a.IsMe)
                            .OrderByDescending(h => (h.PhysicalDamageDealtPlayer + h.MagicDamageDealtPlayer))
                            .FirstOrDefault();

                    if (closeAlly != null)
                    {
                        this.SpellObject.CastOnUnit(closeAlly);
                    }

                    if (ObjectManager.Player.CountAlliesInRange(700f) == 0)
                    {
                        this.SpellObject.CastOnUnit(ObjectManager.Player);
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellE.cs: Can not run OnCombo - {0}", e);
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

        #endregion
    }
}