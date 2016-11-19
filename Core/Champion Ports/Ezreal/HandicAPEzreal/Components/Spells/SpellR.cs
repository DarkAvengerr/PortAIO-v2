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
    ///     The spell R.
    /// </summary>
    internal class SpellR : ISpell
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
        internal override float Range => 3000f;

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
        internal override SpellSlot SpellSlot => SpellSlot.R;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width => 160f;

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
                    var potentialTarget =
                        HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(this.Range) && !x.IsDead && !x.IsZombie && this.SpellObject.GetDamage(x) > x.Health);

                    if (potentialTarget != null)
                    {
                        if (potentialTarget.CountAlliesInRange(850) == 0 && ObjectManager.Player.Distance(target) > 900)
                        {
                            this.SpellObject.Cast(potentialTarget);
                        }   
                    }

                    if (MyMenu.RootMenu.Item("raoe").IsActive() && ObjectManager.Player.CountEnemiesInRange(1150) == 0)
                    {
                        this.SpellObject.CastIfWillHit(target, MyMenu.RootMenu.Item("rifcanhit").GetValue<Slider>().Value);
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellR.cs: Can not run OnCombo - {0}", e);
                throw;
            }
        }

        #endregion
    }
}