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
    ///     The spell Q.
    /// </summary>
    internal class SpellQ : ISpell
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the damage type.
        /// </summary>
        internal override TargetSelector.DamageType DamageType => TargetSelector.DamageType.Magical;

        /// <summary>
        ///     Gets the aoe.
        /// </summary>
        internal override bool Aoe => false;

        /// <summary>
        ///     Gets the delay.
        /// </summary>
        internal override float Delay => 450f;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 850f;

        /// <summary>
        ///     Gets or sets the skillshot type.
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.SkillshotCircle;

        /// <summary>
        ///     Gets the speed.
        /// </summary>
        internal override float Speed => 2500f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.Q;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width => 150f;

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

                var target = Misc.GetTarget(this.Range + this.Width, this.DamageType);
                if (target != null)
                {
                    // lul 0
                    var qTargets =
                        HeroManager.Enemies.Where(x => x.IsValidTarget(this.Range + this.Width))
                            .OrderByDescending(h => h.Distance(ObjectManager.Player));
                    // lul 1
                    if (qTargets.Any())
                    {
                        // lul 2
                        foreach (var target1 in qTargets)
                        {
                            var pred = this.SpellObject.GetPrediction(target1);
                            if (pred.Hitchance >= HitChance.VeryHigh && pred.CastPosition.Distance(ObjectManager.Player.ServerPosition) < this.Range)
                            {
                                // lul 3
                                this.SpellObject.Cast(pred.CastPosition.To2D());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellQ.cs: Can not run OnCombo - {0}", e);
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