using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicLissandra.Spells
{
    #region Using Directives

    using AlqoholicLissandra.Menu;

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
        internal override float Delay => 0f;

        /// <summary>
        ///     Spell Range
        /// </summary>
        internal override float Range => 425f;

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
        internal override SpellSlot SpellSlot => SpellSlot.W;

        /// <summary>
        ///     Gets or sets a value indicating whether the spell is targeted.
        /// </summary>
        internal override bool Targeted => false;

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        internal override float Width => 425f;

        #endregion

        #region Methods

        internal override void Combo()
        {
            var target = TargetSelector.GetTarget(this.Range, this.DamageType);

            if (target != null && !target.MagicImmune)
            {
                Spells.W.SpellObject.Cast();
            }
        }

        /// <summary>
        ///     Spell Farm Logic
        /// </summary>
        internal override void Farm()
        {
            var minions =
                Spells.W.SpellObject.GetCircularFarmLocation(
                    MinionManager.GetMinions(ObjectManager.Player.Position, this.Range));

            var minHit = AlqoholicMenu.MainMenu.Item("laneclearwminhit").GetValue<Slider>().Value;

            if (minions.MinionsHit >= minHit)
            {
                Spells.W.SpellObject.Cast();
            }
        }

        #endregion
    }
}