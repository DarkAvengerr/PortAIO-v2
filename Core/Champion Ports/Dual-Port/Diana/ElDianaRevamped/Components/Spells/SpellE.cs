namespace ElDianaRevamped.Components.Spells
{
    using System;
    using System.Linq;

    using ElDianaRevamped.Enumerations;
    using ElDianaRevamped.Utils;
    using EloBuddy;
    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The spell e.
    /// </summary>
    internal class SpellE : ISpell
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the damage type.
        /// </summary>
        internal override TargetSelector.DamageType DamageType => TargetSelector.DamageType.Magical;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 425f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.E;

        /// <summary>
        ///     Gets a value indicating whether the spell is targeted.
        /// </summary>
        internal override bool Targeted => true;

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
                    if (ObjectManager.Player.IsDashing())
                    {
                        return;
                    }

                    this.SpellObject.Cast();
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellE.cs: Can not run OnCombo - {0}", e);
                throw;
            }
        }


        /// <summary>
        ///     The on mixed.
        /// </summary>
        internal override void OnMixed()
        {
            this.OnCombo();
        }

        #endregion
    }
}