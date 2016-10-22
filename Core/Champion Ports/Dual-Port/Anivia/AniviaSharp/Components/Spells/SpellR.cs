// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpellR.cs" company="LeagueSharp">
//   legacy@joduska.me
// </copyright>
// <summary>
//   The spell R.
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

    using SharpDX;

    /// <summary>
    ///     The spell r.
    /// </summary>
    internal class SpellR : ISpell
    {
        #region Fields

        /// <summary>
        ///     The last R cast position.
        /// </summary>
        private Vector3 lastCastPosition;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpellR" /> class.
        /// </summary>
        internal SpellR()
        {
            Spellbook.OnCastSpell += (sender, args) =>
                {
                    if (sender.Owner.IsMe && args.Slot == SpellSlot.R)
                    {
                        this.lastCastPosition = args.StartPosition;
                    }
                };
        }

        #endregion

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
                return 600f;
            }
        }

        /// <summary>
        ///     Gets or sets the skillshot type.
        /// </summary>
        internal override SkillshotType SkillshotType
        {
            get
            {
                return SkillshotType.SkillshotCircle;
            }
        }

        /// <summary>
        ///     Gets the speed.
        /// </summary>
        internal override float Speed
        {
            get
            {
                return float.MaxValue;
            }
        }

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot
        {
            get
            {
                return SpellSlot.R;
            }
        }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width
        {
            get
            {
                return 251f;
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
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellR.cs: Can not run OnCombo - {0}", e);
                throw;
            }
        }

        /// <summary>
        /// The on mixed.
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

                if (
                    !ObjectManager.Get<Obj_AI_Base>()
                         .Any(
                             obj =>
                             obj.IsValidTarget() && obj.Distance(this.lastCastPosition) <= this.Width * 2
                             && obj.HasBuff("chilled")))
                {
                    this.SpellObject.Cast();
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellR.cs: Can not run OnUpdate - {0}", e);
                throw;
            }
        }

        #endregion
    }
}