// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpellQ.cs" company="LeagueSharp">
//   legacy@joduska.me
// </copyright>
// <summary>
//   The spell Q.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace AniviaSharp.Components.Spells
{
    using System;
    using System.Linq;

    using AniviaSharp.Enumerations;
    using AniviaSharp.Utils;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The spell Q.
    /// </summary>
    internal class SpellQ : ISpell
    {
        #region Properties

        /// <summary>
        ///     Gets a value indicating whether aoe.
        /// </summary>
        internal override bool Aoe
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Gets or sets the damage type.
        /// </summary>
        internal override TargetSelector.DamageType DamageType
        {
            get
            {
                return TargetSelector.DamageType.Magical;
            }
        }

        /// <summary>
        ///     Gets the delay.
        /// </summary>
        internal override float Delay
        {
            get
            {
                return 250f;
            }
        }

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range
        {
            get
            {
                return 1100f;
            }
        }

        /// <summary>
        ///     Gets or sets the skillshot type.
        /// </summary>
        internal override SkillshotType SkillshotType
        {
            get
            {
                return SkillshotType.SkillshotLine;
            }
        }

        /// <summary>
        ///     Gets the speed.
        /// </summary>
        internal override float Speed
        {
            get
            {
                return 850f;
            }
        }

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot
        {
            get
            {
                return SpellSlot.Q;
            }
        }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width
        {
            get
            {
                return 110f;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The on combo callback.
        /// </summary>
        internal override void OnCombo()
        {
            try
            {
                if (this.SpellObject == null || this.SpellObject.Instance.ToggleState != 1)
                {
                    return;
                }

                var target = Misc.GetTarget(this.Range, this.DamageType);

                if (target != null)
                {
                    this.SpellObject.Cast(target, false, this.Aoe);
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

        /// <summary>
        ///     The on update callback.
        /// </summary>
        internal override void OnUpdate()
        {
            try
            {
                if (this.SpellObject == null || this.SpellObject.Instance.ToggleState != 2)
                {
                    return;
                }

                var missile =
                    ObjectManager.Get<MissileClient>()
                        .FirstOrDefault(obj => obj.SData.Name == "FlashFrostSpell" && obj.SpellCaster.IsMe);

                if (missile != null)
                {
                    if (
                        HeroManager.Enemies.Any(
                            hero => hero.IsValidTarget() & hero.ServerPosition.Distance(missile.Position) <= this.Width))
                    {
                        this.SpellObject.Cast();
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellQ.cs: Can not run OnUpdate - {0}", e);
                throw;
            }
        }

        #endregion
    }
}