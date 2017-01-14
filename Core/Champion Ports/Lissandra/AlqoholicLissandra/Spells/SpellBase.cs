using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicLissandra.Spells
{
    #region Using Directives

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    /// <summary>
    ///     Abstract class for Spell
    /// </summary>
    internal abstract class SpellBase
    {
        #region Fields

        /// <summary>
        ///     Spell Object
        /// </summary>
        private Spell spellObject;

        #endregion

        #region Properties

        /// <summary>
        ///     Aoe boolean
        /// </summary>
        internal virtual bool Aoe { get; set; } = false;

        /// <summary>
        ///     Collision boolean
        /// </summary>
        internal virtual bool Collision { get; set; } = false;

        /// <summary>
        ///     TargetSelector Damagetype
        /// </summary>
        internal virtual TargetSelector.DamageType DamageType { get; set; }

        /// <summary>
        ///     Spell Delay
        /// </summary>
        internal virtual float Delay { get; set; }

        /// <summary>
        ///     Spell Range
        /// </summary>
        internal virtual float Range { get; set; }

        /// <summary>
        ///     Skillshot Type
        /// </summary>
        internal virtual SkillshotType SkillshotType { get; set; }

        /// <summary>
        /// </summary>
        internal virtual float Speed { get; set; }

        /// <summary>
        ///     Gets or Sets the SpellObject
        /// </summary>
        internal Spell SpellObject
        {
            get
            {
                if (this.spellObject != null)
                {
                    return this.spellObject;
                }

                this.spellObject = new Spell(this.SpellSlot, this.Range, this.DamageType);

                if (this.Targeted)
                {
                    this.spellObject.SetTargetted(this.Delay, this.Speed);
                }
                else
                {
                    this.spellObject.SetSkillshot(
                        this.Delay,
                        this.Width,
                        this.Speed,
                        this.Collision,
                        this.SkillshotType);
                }

                return this.spellObject;
            }
            set
            {
                this.spellObject = value;
            }
        }

        /// <summary>
        ///     Gets or sets the spell slot.
        /// </summary>
        internal virtual SpellSlot SpellSlot { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the spell is targeted.
        /// </summary>
        internal virtual bool Targeted { get; set; } = false;

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        internal virtual float Width { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Spell Combo Logic
        /// </summary>
        internal virtual void Combo()
        {
        }

        /// <summary>
        ///     Spell Farm Logic
        /// </summary>
        internal virtual void Farm()
        {
        }

        /// <summary>
        ///     Spell Harass Logic
        /// </summary>
        internal virtual void Harass()
        {
        }

        /// <summary>
        ///     Last Hit Logic
        /// </summary>
        internal virtual void LastHit()
        {
        }

        #endregion
    }
}