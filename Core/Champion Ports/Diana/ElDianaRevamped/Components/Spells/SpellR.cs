namespace ElDianaRevamped.Components.Spells
{
    using System;
    using System.Linq;

    using ElDianaRevamped.Enumerations;
    using ElDianaRevamped.Utils;
    using EloBuddy;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The spell r.
    /// </summary>
    internal class SpellR : ISpell
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the damage type.
        /// </summary>
        internal override TargetSelector.DamageType DamageType => TargetSelector.DamageType.Magical;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 825f;

        /// <summary>
        ///     Gets the speed.
        /// </summary>
        internal override float Speed => 1640f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.R;

        /// <summary>
        ///     Gets a value indicating whether the spell is targeted.
        /// </summary>
        internal override bool Targeted => true;

        /// <summary>
        ///     Gets a value with the enemies in range.
        /// </summary>
        private int EnemiesInRange => ObjectManager.Player.GetEnemiesInRange(this.Range * 2).Count;

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
                    if (target.HasBuff("dianamoonlight") || target.Health <= this.SpellObject.GetDamage(target))
                    {
                        this.SpellObject.CastOnUnit(target);
                    }

                    if (MyMenu.RootMenu.Item("comboRSecure").IsActive())
                    {
                        if (target.HasBuff("dianamoonlight") && (!target.UnderTurret(true) || (MyMenu.RootMenu.Item("comboRSecureHealth").GetValue<Slider>().Value <= ObjectManager.Player.HealthPercent)))
                        {
                            this.SpellObject.CastOnUnit(target);
                        }

                        if ((!target.UnderTurret(true) || (MyMenu.RootMenu.Item("comboRSecureHealth").GetValue<Slider>().Value <= ObjectManager.Player.HealthPercent)))
                        {
                            if (this.EnemiesInRange <= MyMenu.RootMenu.Item("comboRSecureRange").GetValue<Slider>().Value && !new SpellQ().SpellSlot.IsReady())
                            {
                                if (target.Health < this.SpellObject.GetDamage(target))
                                {
                                    this.SpellObject.CastOnUnit(target);
                                }
                            }
                        }

                        if (this.EnemiesInRange <= MyMenu.RootMenu.Item("comboRSecureRange").GetValue<Slider>().Value)
                        {
                            if (target.Health < this.SpellObject.GetDamage(target))
                            {
                                this.SpellObject.CastOnUnit(target);
                            }
                        }
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
        ///     The on mixed.
        /// </summary>
        internal override void OnMixed()
        {
            this.OnCombo();
        }

        /// <summary>
        ///     The on lane clear callback.
        /// </summary>
        internal override void OnLaneClear()
        {
            var minion =
                MinionManager.GetMinions(this.Range)
                    .Where(obj => obj.CharData.BaseSkinName.EndsWith("MinionSiege") && obj.HasBuff("dianamoonlight") && this.SpellObject.IsKillable(obj))
                    .MinOrDefault(obj => obj.Health);

            if (minion != null && MyMenu.RootMenu.Item("laneclear.r.siege").IsActive())
            {
                this.SpellObject.Cast(minion);
            }
        }

        /// <summary>
        ///     The on jungle clear callback.
        /// </summary>
        internal override void OnJungleClear()
        {
            var minion =
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, this.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                    .Where(obj => this.SpellObject.IsKillable(obj) && obj.HasBuff("dianamoonlight"))
                    .MinOrDefault(obj => obj.Health);

            if (minion != null)
            {
                this.SpellObject.Cast(minion);
            }
        }

        #endregion
    }
}