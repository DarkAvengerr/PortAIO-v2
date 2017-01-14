using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicKarthus.Spells
{
    #region Using Directives

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class W : SpellBase
    {
        #region Properties

        /// <summary>
        ///     Aoe boolean
        /// </summary>
        internal override bool Aoe => true;

        /// <summary>
        ///     Collision boolean
        /// </summary>
        internal override bool Collision => false;

        /// <summary>
        ///     TargetSelector Damagetype
        /// </summary>
        internal override TargetSelector.DamageType DamageType => TargetSelector.DamageType.Magical;

        /// <summary>
        ///     Spell Delay
        /// </summary>
        internal override float Delay => 0.5f;

        /// <summary>
        ///     Spell Range
        /// </summary>
        internal override float Range => 1000f;

        /// <summary>
        ///     Skillshot Type
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.SkillshotCircle; //:thinking:

        /// <summary>
        /// </summary>
        internal override float Speed => float.MaxValue;

        /// <summary>
        ///     Gets or sets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.W;

        /// <summary>
        ///     Gets or sets a value indicating whether the spell is targeted.
        /// </summary>
        internal override bool Targeted => false;

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        internal override float Width => 70f;

        #endregion

        #region Methods

        internal override void Combo(int predictionMode)
        {
            var target = TargetSelector.GetTarget(this.Range, this.DamageType);

            this.SpellObject.Cast(target);
        }

        #endregion
    }
}