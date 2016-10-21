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
    ///     The spell w.
    /// </summary>
    internal class SpellW : ISpell
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the damage type.
        /// </summary>
        internal override TargetSelector.DamageType DamageType => TargetSelector.DamageType.Magical;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 250f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.W;

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
                if (this.SpellObject == null || ObjectManager.Player.IsDashing())
                {
                    return;
                }

                var target = Misc.GetTarget(this.Range, this.DamageType);
                if (target != null)
                {
                    if (ObjectManager.Player.IsDashing())
                    {
                        Logging.AddEntry(LoggingEntryTrype.Debug, "@SpellW.cs: Player is dashing");
                        return;
                    }

                    this.SpellObject.Cast();
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellW.cs: Can not run OnCombo - {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     The on lane clear callback.
        /// </summary>
        internal override void OnLaneClear()
        {
            var minion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, this.Range + 50);
            var minions = this.SpellObject.GetCircularFarmLocation(minion, this.SpellObject.Width);

            if (minion != null && MyMenu.RootMenu.Item("laneclear.r.siege").IsActive())
            {
                if (minions.MinionsHit >= 3)
                {
                    this.SpellObject.Cast();
                }
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