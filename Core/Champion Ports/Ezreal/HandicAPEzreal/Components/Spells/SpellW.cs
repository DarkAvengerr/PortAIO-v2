using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HandicapEzreal.Components.Spells
{
    using System;
    using System.Linq;

    using HandicapEzreal.Enumerations;
    using HandicapEzreal.Utils;

    using LeagueSharp;
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
        internal override float Delay => 0.25f;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 1000f;

        /// <summary>
        ///     Gets or sets the skillshot type.
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.SkillshotLine;

        /// <summary>
        ///     Gets the speed.
        /// </summary>
        internal override float Speed => 2000f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.W;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width => 80f;

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
                    var prediction = this.SpellObject.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.Medium)
                    {
                        this.SpellObject.Cast(prediction.CastPosition);
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellW.cs: Can not run OnCombo - {0}", e);
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