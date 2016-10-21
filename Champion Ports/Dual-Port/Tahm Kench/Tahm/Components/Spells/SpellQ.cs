namespace ElTahmKench.Components.Spells
{
    using System;
    using System.Linq;

    using ElTahmKench.Enumerations;
    using ElTahmKench.Utils;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
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
        ///     Gets the delay.
        /// </summary>
        internal override float Delay => 0.25f;

        /// <summary>
        ///     Spell has collision.
        /// </summary>
        internal override bool Collision => true;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 800f;

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
        internal override SpellSlot SpellSlot => SpellSlot.Q;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width => 70f;

        #endregion

        #region Methods

        /// <summary>
        ///     The on combo callback.
        /// </summary>
        internal override void OnCombo()
        {
            try
            {
                var target = HeroManager.Enemies.Where(x => x.Distance(ObjectManager.Player) <= this.Range + x.BoundingRadius )
                    .OrderBy(obj => obj.Distance(ObjectManager.Player.ServerPosition))
                    .FirstOrDefault();

                if (Misc.HasDevouredBuff && Misc.LastDevouredType == DevourType.Enemy) target = null;
                if (target != null)
                {
                    if (ObjectManager.Player.Distance(target) <= this.Range)
                    {
                        var prediction = this.SpellObject.GetPrediction(target);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            this.SpellObject.Cast(prediction.CastPosition);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellQ.cs: Can not run OnCombo - {0}", e);
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

        /// <summary>
        ///     The on last hit callback.
        /// </summary>
        internal override void OnLastHit()
        {
            var minion =
               MinionManager.GetMinions(this.Range)
                   .Where(obj => this.SpellObject.IsKillable(obj))
                   .MinOrDefault(obj => obj.Health);

            if (minion != null)
            {
                this.SpellObject.Cast(minion);
            }
        }

        #endregion
    }
}