using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElRengarDecentralized.Components.Spells
{
    using System;
    using System.Linq;

    using ElRengarDecentralized.Enumerations;
    using ElRengarDecentralized.Utils;

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
        ///     Gets the range.
        /// </summary>
        internal override float Range => Misc.RengarAutoAttackRange + 350f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.Q;

        /// <summary>
        ///     Gets the delay.
        /// </summary>
        internal override float Delay => 0.25f;

        /// <summary>
        ///     Gets or sets the skillshot type.
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.SkillshotLine;

        /// <summary>
        ///     Gets the speed.
        /// </summary>
        internal override float Speed => 1500f;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width => 70f;

        /// <summary>
        ///     Gets the AoE.
        /// </summary>
        internal override bool Aoe => true;

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
                    if (BuffManager.HasPassive && target.Distance(ObjectManager.Player) > 400f && !ObjectManager.Player.IsDashing())
                    {
                        return;
                    }

                    if (BuffManager.HasUltimate && !ObjectManager.Player.IsDashing())
                    {
                        return;
                    }

                    if (Misc.GetFerocityStacks() == 4
                         && MyMenu.RootMenu.Item("combo.prio").GetValue<StringList>().SelectedIndex != 2)
                    {
                        return;
                    }

                    if (target.IsValidTarget(this.Range))
                    {
                        this.SpellObject.Cast(target.ServerPosition);
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
        ///     The on update callback.
        /// </summary>
        internal override void OnUpdate()
        {
            if (BuffManager.HasUltimate)
            {
                if (ObjectManager.Player.IsDashing())
                {
                    this.SpellObject.Cast();
                }
            }
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
            var minion = MinionManager.GetMinions(this.Range);
            if (minion != null)
            {
                var killable =
                    minion.FirstOrDefault(obj => obj.Health < ObjectManager.Player.GetSpellDamage(obj, SpellSlot.Q));

                if (killable != null && killable.IsValidTarget(Misc.RengarAutoAttackRange) && this.SpellObject.Cast(killable).IsCasted())
                {
                    return;
                }

                var sortedMinions = minion.MinOrDefault(obj => obj.Health);
                if (sortedMinions != null && this.SpellObject.Cast(sortedMinions).IsCasted()) {}
            }
        }

        /// <summary>
        ///     The on jungle clear callback.
        /// </summary>
        internal override void OnJungleClear()
        {
            var minion =
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, BuffManager.HasPassive ? 600f : this.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                .MinOrDefault(obj => obj.MaxHealth);

            if (minion != null)
            {
                if (BuffManager.HasPassive)
                {
                    if (minion.Distance(ObjectManager.Player) > this.Range)
                    {
                        Logging.AddEntry(LoggingEntryType.Debug, "@SpellQ.cs: Range too big.");
                        return;
                    }
                }

                this.SpellObject.Cast(minion);
            }
        }

        #endregion
    }
}