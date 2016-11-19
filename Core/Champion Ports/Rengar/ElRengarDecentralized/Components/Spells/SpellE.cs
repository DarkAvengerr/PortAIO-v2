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
    ///     The spell E.
    /// </summary>
    internal class SpellE : ISpell
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
        internal override float Range => 1000f;

        /// <summary>
        ///     Gets or sets the skillshot type.
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.SkillshotLine;

        /// <summary>
        ///     Gets the speed.
        /// </summary>
        internal override float Speed => 1500f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.E;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width => 70f;

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
                if (this.SpellObject == null)
                {
                    return;
                }

                if (BuffManager.HasUltimate)
                {
                    return;
                }

                var target = Misc.GetTarget(this.Range, this.DamageType);
                if (target != null)
                {
                    if (Misc.GetFerocityStacks() == 4
                        && MyMenu.RootMenu.Item("combo.prio").GetValue<StringList>().SelectedIndex != 0)
                    {
                        return;
                    }

                    var prediction = this.SpellObject.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.VeryHigh)
                    {
                        this.SpellObject.Cast(target);
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellE.cs: Can not run OnCombo - {0}", e);
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
            if (Misc.GetFerocityStacks() == 4)
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
        ///     The on update callback.
        /// </summary>
        internal override void OnUpdate()
        {
            if ((Program.Orbwalker.ActiveMode.Equals(Orbwalking.OrbwalkingMode.Combo)
                 || Program.Orbwalker.ActiveMode.Equals(Orbwalking.OrbwalkingMode.Mixed)))
            {

                if (BuffManager.HasUltimate)
                {
                    return;
                }

                var targetx =
                        HeroManager.Enemies
                            .FirstOrDefault(h => h.IsValidTarget(this.Range));

                if (targetx != null)
                {
                    if (ObjectManager.Player.IsDashing())
                    {
                        this.SpellObject.CastIfHitchanceEquals(targetx, HitChance.Medium);
                    }
                }
            }

            if (Misc.GetFerocityStacks() == 4 && !BuffManager.HasUltimate)
            {
                if (MyMenu.RootMenu.Item("comborootstunned").IsActive())
                {
                    var target =
                        HeroManager.Enemies
                            .FirstOrDefault(h => h.IsValidTarget(this.Range) && h.GetStunDuration() >= this.SpellObject.Delay);

                    if (target != null)
                    {
                        this.SpellObject.Cast(target);
                    }
                }
            }
        }

        /// <summary>
        ///     The on lane clear callback.
        /// </summary>
        internal override void OnLaneClear()
        {
            if (Misc.GetFerocityStacks() == 4)
            {
                return;
            }

            var minion =
                MinionManager.GetMinions(this.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(
                        x =>
                            x.Health < this.SpellObject.GetDamage(x)
                            && HealthPrediction.GetHealthPrediction(
                                x,
                                (int)(ObjectManager.Player.Distance(x, false) / this.SpellObject.Speed),
                                (int)(this.SpellObject.Delay * 1000 + Game.Ping / 2)) > 0);

            if (minion != null)
            {
                this.SpellObject.Cast(minion);
            }
        }

        /// <summary>
        ///     The on jungle clear callback.
        /// </summary>
        internal override void OnJungleClear()
        {
            if (Misc.GetFerocityStacks() == 4)
            {
                return;
            }

            var minion =
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, this.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                .MinOrDefault(obj => obj.MaxHealth);

            if (minion != null)
            {
                this.SpellObject.Cast(minion.Position);
            }
        }

        #endregion
    }
}