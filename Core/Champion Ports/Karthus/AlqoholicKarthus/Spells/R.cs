using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicKarthus.Spells
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class R : SpellBase
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
        internal override float Delay => 3000f;

        /// <summary>
        ///     Spell Range
        /// </summary>
        internal override float Range => 20000f;

        /// <summary>
        ///     Skillshot Type
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.SkillshotCircle;

        /// <summary>
        /// </summary>
        internal override float Speed => float.MaxValue;

        /// <summary>
        ///     Gets or sets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.R;

        /// <summary>
        ///     Gets or sets a value indicating whether the spell is targeted.
        /// </summary>
        internal override bool Targeted => false;

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        internal override float Width => float.MaxValue;

        #endregion

        #region Methods

        internal override void Combo(int predictionMode)
        {
            try
            {
                this.SpellObject.Cast();
            }
            catch (Exception e)
            {
                Console.WriteLine("@R.cs: Cannot call Combo - {0}", e);
                throw;
            }
        }

        #endregion
    }
}