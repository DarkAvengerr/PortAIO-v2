namespace ElVarusRevamped.Components.Spells
{
    using System;
    using System.Linq;

    using ElVarusRevamped.Enumerations;
    using ElVarusRevamped.Utils;
    using EloBuddy;
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
        internal override TargetSelector.DamageType DamageType => TargetSelector.DamageType.Magical;

        /// <summary>
        ///     Gets Aoe
        /// </summary>
        internal override bool Aoe => true;

        /// <summary>
        ///     Gets the delay.
        /// </summary>
        internal override float Delay => 1000f;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 950f;

        /// <summary>
        ///     Gets or sets the skillshot type.
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.SkillshotCircle;

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
        internal override float Width => 235f;

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

                if (Misc.SpellQ.SpellObject.IsCharging)
                {
                    return;
                }

                var target = Misc.GetTarget(this.Range + this.Width, this.DamageType);
                if (target != null)
                {
                    if (MyMenu.RootMenu.Item("comboealways").IsActive() || 
                        this.SpellObject.IsKillable(target) || 
                        (Misc.BlightedQuiver.Level > 0 && Misc.GetWStacks(target) >= MyMenu.RootMenu.Item("comboew.count").GetValue<Slider>().Value))
                    {
                        if ((!MyMenu.RootMenu.Item("comboealways").IsActive()
                             && Misc.LastQ + 200 < Environment.TickCount) || this.SpellObject.IsKillable(target))
                        {
                            this.SpellObject.Cast(target);
                            Misc.LastE = Environment.TickCount;
                        }
                        else
                        {
                            this.SpellObject.Cast(target);
                        }
                    }

                    var multipleTargets = HeroManager.Enemies.Where(x => x.IsValidTarget(this.Range + this.Width) && !x.IsDead && !x.IsZombie);
                    foreach (var targetInRange in multipleTargets)
                    {
                        this.SpellObject.CastIfWillHit(
                            targetInRange,
                            MyMenu.RootMenu.Item("comboe.count.hit").GetValue<Slider>().Value);
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellE.cs: Can not run OnCombo - {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     The on mixed callback.
        /// </summary>
        internal override void OnMixed()
        {
            if (Misc.SpellQ.SpellObject.IsCharging)
            {
                return;
            }

            var target = Misc.GetTarget(this.Range + this.Width, this.DamageType);
            if (target != null)
            {
                if (MyMenu.RootMenu.Item("mixedeusealways").IsActive() ||
                    Misc.GetWStacks(target) >= MyMenu.RootMenu.Item("mixedeusealways.count").GetValue<Slider>().Value)
                {
                    var prediction = this.SpellObject.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.VeryHigh)
                    {
                        this.SpellObject.Cast(prediction.CastPosition);
                    }
                }
            }
        }

        /// <summary>
        ///     The on lane clear callback.
        /// </summary>
        internal override void OnLaneClear()
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, this.Range + this.Width);
            if (minions != null)
            {
                var minion = this.SpellObject.GetCircularFarmLocation(minions, this.Range + this.Width);
                
                if (minion.MinionsHit >= MyMenu.RootMenu.Item("lasthit.count.e").GetValue<Slider>().Value)
                {
                    if (minion.Position.IsValid())
                    {
                        this.SpellObject.Cast(minion.Position);
                    }
                }
            }
        }

        /// <summary>
        ///     The on jungle clear callback.
        /// </summary>
        internal override void OnJungleClear()
        {
            var minions =
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    this.Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (minions != null && MyMenu.RootMenu.Item("jungleclearusee").IsActive())
            {
                // temp
                if (Misc.GetWStacks(minions) >= 1 && minions.IsValid)
                {
                    this.SpellObject.Cast(minions.Position);
                }
            }
        }

        #endregion
    }
}