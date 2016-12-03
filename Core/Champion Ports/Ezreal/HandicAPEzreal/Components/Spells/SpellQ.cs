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
    ///     The spell Q.
    /// </summary>
    internal class SpellQ : ISpell
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the damage type.
        /// </summary>
        internal override TargetSelector.DamageType DamageType => TargetSelector.DamageType.Physical;

        /// <summary>
        ///     Gets the delay.
        /// </summary>
        internal override float Delay => 0.25f;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 1150f;

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
        internal override float Width => 60f;

        /// <summary>
        ///     Gets the Collision.
        /// </summary>
        internal override bool Collision => true;

        #endregion

        #region Methods

        /// <summary>
        ///     The on combo callback.
        /// </summary>
        internal override void OnCombo()
        {
            try
            {
                var target = Misc.GetTarget(this.Range, this.DamageType);
                if (target != null)
                {
                    var prediction = this.SpellObject.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.VeryHigh || prediction.Hitchance == HitChance.Immobile || !target.CanMove)
                    {
                        this.SpellObject.Cast(target);
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

        /// <summary>
        ///     The on lane clear callback.
        /// </summary>
        internal override void OnLaneClear()
        {
            var minion =
                MinionManager.GetMinions(this.Range)
                    .Where(obj => this.SpellObject.IsKillable(obj))
                    .MinOrDefault(obj => obj.Health);

            if (minion != null)
            {
                if (MyMenu.RootMenu.Item("lasthit.mode").GetValue<StringList>().SelectedIndex == 0)
                {
                    this.SpellObject.Cast(minion);
                }
                else
                {
                    if (minion.Distance(ObjectManager.Player) > Misc.EzrealAutoAttackRange
                        && ObjectManager.Player.Distance(minion) <= this.Range)
                    {
                        this.SpellObject.Cast(minion);
                    }
                }
            }
        }

        /// <summary>
        ///     The on jungle clear callback.
        /// </summary>
        internal override void OnJungleClear()
        {
            var minion =
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, this.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                .MinOrDefault(obj => obj.MaxHealth);

            if (minion != null)
            {
                this.SpellObject.Cast(minion);
            }
        }

        #endregion
    }
}