namespace ElDianaRevamped.Components.Spells
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;

    using ElDianaRevamped.Enumerations;
    using ElDianaRevamped.Utils;
    using EloBuddy;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     Base class for the spells.
    /// </summary>
    internal class ISpell
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ISpell" /> class.
        ///     The initialize.
        /// </summary>
        [SuppressMessage("ReSharper", "DoNotCallOverridableMethodsInConstructor", Justification = "Input is known.")]
        internal ISpell()
        {
            try
            {
                this.SpellObject = new Spell(this.SpellSlot, this.Range, this.DamageType);

                if (this.Targeted)
                {
                    this.SpellObject.SetTargetted(this.Delay, this.Speed);
                }
                else
                {
                    this.SpellObject.SetSkillshot(
                        this.Delay,
                        this.Width,
                        this.Speed,
                        this.Collision,
                        this.SkillshotType);
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@ISpell.cs: Can not initialize the base class - {0}", e);
                throw;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the spell is an AoE spell.
        /// </summary>
        [DefaultValue(false)]
        internal virtual bool Aoe { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the spell has collision.
        /// </summary>
        [DefaultValue(false)]
        internal virtual bool Collision { get; set; }

        /// <summary>
        ///     Gets or sets the damage type.
        /// </summary>
        internal virtual TargetSelector.DamageType DamageType { get; set; }

        /// <summary>
        ///     Gets or sets the delay.
        /// </summary>
        internal virtual float Delay { get; set; }

        /// <summary>
        ///     Gets or sets the range.
        /// </summary>
        internal virtual float Range { get; set; }

        /// <summary>
        ///     Gets or sets the skillshot type.
        /// </summary>
        internal virtual SkillshotType SkillshotType { get; set; }

        /// <summary>
        ///     Gets or sets the speed.
        /// </summary>
        internal virtual float Speed { get; set; }

        /// <summary>
        ///     Gets or sets the spell object.
        /// </summary>
        internal Spell SpellObject { get; set; }

        /// <summary>
        ///     Gets or sets the spell slot.
        /// </summary>
        internal virtual SpellSlot SpellSlot { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the spell is targeted.
        /// </summary>
        [DefaultValue(false)]
        internal virtual bool Targeted { get; set; }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        internal virtual float Width { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The on on combo callback.
        /// </summary>
        internal virtual void OnCombo()
        {
        }

        /// <summary>
        /// The on lane clear callback.
        /// </summary>
        internal virtual void OnLaneClear()
        {
        }

        /// <summary>
        /// The on jungle clear callback.
        /// </summary>
        internal virtual void OnJungleClear()
        {
        }

        /// <summary>
        /// The on last hit callback.
        /// </summary>
        internal virtual void OnLastHit()
        {
        }

        /// <summary>
        /// The on mixed callback.
        /// </summary>
        internal virtual void OnMixed()
        {
        }

        /// <summary>
        ///     The on update callback.
        /// </summary>
        internal virtual void OnUpdate()
        {
        }

        #endregion
    }
}