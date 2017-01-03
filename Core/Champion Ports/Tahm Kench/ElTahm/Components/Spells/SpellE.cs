namespace ElTahmKench.Components.Spells
{
    using System;
    using System.Linq;

    using ElTahmKench.Enumerations;
    using ElTahmKench.Utils;
    using EloBuddy;
    using LeagueSharp;
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
        internal override float Delay => 0f;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 900f;

        /// <summary>
        ///     Gets or sets the skillshot type.
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.SkillshotLine;

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

        internal override void OnUpdate()
        {
            if (!MyMenu.RootMenu.Item("shieldeuse").IsActive())
            {
                return;
            }

            if (ObjectManager.Player.ManaPercent < MyMenu.RootMenu.Item("shieldemana").GetValue<Slider>().Value)
            {
                return;
            }

            if (ObjectManager.Player.HealthPercent <= MyMenu.RootMenu.Item("ehealthpercentage").GetValue<Slider>().Value && ObjectManager.Player.CountEnemiesInRange(this.Range) > 0)
            {
                this.SpellObject.Cast();
            }
        }

        #endregion
    }
}