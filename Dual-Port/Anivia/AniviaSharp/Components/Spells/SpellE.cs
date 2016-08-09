// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpellE.cs" company="LeagueSharp">
//   legacy@joduska.me
// </copyright>
// <summary>
//   The spell E.
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
    ///     The spell e.
    /// </summary>
    internal class SpellE : ISpell
    {
        #region Properties

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
                return 0.25f;
            }
        }

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range
        {
            get
            {
                return 650f;
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
                return SpellSlot.E;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the spell is targeted.
        /// </summary>
        internal override bool Targeted
        {
            get
            {
                return true;
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
                if (this.SpellObject == null)
                {
                    return;
                }

                var target = Misc.GetTarget(this.Range, this.DamageType);

                if (target != null)
                {
                    if (target.HasBuffUntil("chilled", ObjectManager.Player.Distance(target) / this.Speed)
                        || target.Health <= this.SpellObject.GetDamage(target))
                    {
                        this.SpellObject.Cast(target, false, this.Aoe);
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellR.cs: Can not run OnCombo - {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     The on lane clear callback.
        /// </summary>
        internal override void OnLaneClear()
        {
            if (!Orbwalking.CanMove(40f))
            {
                return;
            }

            var minion =
                MinionManager.GetMinions(this.Range)
                    .Where(obj => this.SpellObject.IsKillable(obj))
                    .MinOrDefault(obj => obj.Health);

            if (minion != null)
            {
                this.SpellObject.Cast(minion);
            }
        }

        /// <summary>
        ///     The on last hit callback.
        /// </summary>
        internal override void OnLastHit()
        {
            var minion =
                MinionManager.GetMinions(this.Range)
                    .Where(obj => obj.CharData.BaseSkinName.EndsWith("MinionSiege") && this.SpellObject.IsKillable(obj))
                    .MinOrDefault(obj => obj.Health);

            if (minion != null)
            {
                this.SpellObject.Cast(minion);
            }
        }

        /// <summary>
        ///     The on mixed.
        /// </summary>
        internal override void OnMixed()
        {
            this.OnCombo();
            this.OnLastHit();
        }

        #endregion
    }
}