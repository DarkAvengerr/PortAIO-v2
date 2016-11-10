using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElKatarinaDecentralized.Components.Spells
{
    using System;
    using System.Linq;

    using ElKatarinaDecentralized.Enumerations;
    using ElKatarinaDecentralized.Utils;

    using LeagueSharp;
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
        ///     Gets the targeted mode.
        /// </summary>
        internal override bool Targeted => true;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 625f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.Q;

        /// <summary>
        ///     The last spell casting.
        /// </summary>
        internal int lastSpellCastTime;

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

                if (ObjectManager.Player.IsChannelingImportantSpell())
                {
                    return;
                }

                var target = Misc.GetTarget(this.Range, this.DamageType);
                if (target != null)
                {
                    this.SpellObject.CastOnUnit(target);
                    this.lastSpellCastTime = Utils.TickCount;
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
                this.SpellObject.CastOnUnit(minion);
            }
        }

        /// <summary>
        ///     The on lane clear callback.
        /// </summary>
        internal override void OnLaneClear()
        {
            var minion =
               MinionManager.GetMinions(this.Range)
                   .MinOrDefault(obj => obj.Health);

            if (minion != null)
            {
                this.SpellObject.CastOnUnit(minion);
            }
        }

        #endregion
    }
}