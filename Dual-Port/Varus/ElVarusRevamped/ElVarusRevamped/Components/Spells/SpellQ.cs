namespace ElVarusRevamped.Components.Spells
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using ElVarusRevamped.Enumerations;
    using ElVarusRevamped.Utils;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
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
        internal override float Delay => 250f;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 925f;

        /// <summary>
        ///     Gets or sets the skillshot type.
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.SkillshotLine;

        /// <summary>
        ///     Gets the speed.
        /// </summary>
        internal override float Speed => 1800f;

        /// <summary>   
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.Q;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width => 70f;

        /// <summary>
        ///     Gets the min range.
        /// </summary>
        internal override int MinRange => 925;

        /// <summary>
        ///     Gets the max range.
        /// </summary>
        internal override int MaxRange => 1700;

        /// <summary>
        ///     Gets the delta T
        /// </summary>
        internal override float DeltaT => 1.5f;

        /// <summary>
        ///     Gets the spellname.
        /// </summary>
        internal override string SpellName => "VarusQ";

        /// <summary>
        ///     Gets the buffname.
        /// </summary>
        internal override string BuffName => "VarusQ";

        /// <summary>
        ///     Sets the charged spell.
        /// </summary>
        internal override bool Charged => true;

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

                var target = Misc.GetTarget((this.MaxRange + this.Width) * 1.1f, this.DamageType);
                if (target != null)
                {                  
                    if (!this.SpellObject.IsCharging)
                    {
                        if (MyMenu.RootMenu.Item("comboqalways").IsActive()
                            || Misc.QIsKillable(target, Misc.GetQCollisionsCount(target, this.SpellObject.GetPrediction(target).CastPosition)) 
                            || (Misc.BlightedQuiver.Level > 0 && Misc.GetWStacks(target) >= MyMenu.RootMenu.Item("combow.count").GetValue<Slider>().Value))
                        {
                            this.SpellObject.StartCharging();
                        }
                    }

                    if (this.SpellObject.IsCharging)
                    {
                        if (this.Range >= this.MaxRange || target.Distance(ObjectManager.Player) < this.Range + 250
                            || (this.SpellObject.GetPrediction(target).Hitchance >= HitChance.VeryHigh)
                            && target.Distance(ObjectManager.Player) > this.Range + 250
                            && target.Distance(ObjectManager.Player) < this.MaxRange || ObjectManager.Player.HealthPercent <= MyMenu.RootMenu.Item("comboq.fast").GetValue<Slider>().Value)
                        {
                            if (!this.SpellObject.IsCharging)
                            {
                                Logging.AddEntry(LoggingEntryTrype.Info, "Return Charging");
                                return;
                            }

                            if ((!MyMenu.RootMenu.Item("comboqalways").IsActive()
                                 && Misc.LastE + 200 < Environment.TickCount) || 
                                 Misc.QIsKillable(target, Misc.GetQCollisionsCount(target, this.SpellObject.GetPrediction(target).CastPosition)))
                            {
                                this.SpellObject.Cast(target);
                                Misc.LastQ = Environment.TickCount;
                            }
                            else
                            {
                                this.SpellObject.Cast(target);
                            }
                        }
                    }
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
            var target = Misc.GetTarget((this.MaxRange + this.Width) * 1.1f, this.DamageType);
            if (target != null)
            {
                if (!this.SpellObject.IsCharging)
                {
                    if (MyMenu.RootMenu.Item("mixedqusealways").IsActive()
                        || target.Distance(ObjectManager.Player) > Orbwalking.GetRealAutoAttackRange(target) * 1.2f
                        || (Misc.BlightedQuiver.Level > 0 && Misc.GetWStacks(target) >= MyMenu.RootMenu.Item("mixedqusealways.count").GetValue<Slider>().Value))
                    {
                        this.SpellObject.StartCharging();
                    }
                }

                if (this.SpellObject.IsCharging)
                {
                   if(this.Range >= this.MaxRange || target.Distance(ObjectManager.Player) < this.Range + 250 
                        || (this.SpellObject.GetPrediction(target).Hitchance >= HitChance.VeryHigh)
                            && target.Distance(ObjectManager.Player) > this.Range + 250
                            && target.Distance(ObjectManager.Player) < this.MaxRange || target.Distance(ObjectManager.Player) < this.Range)
                    {
                        this.SpellObject.Cast(target);
                    }
                }
            }
        }

        /// <summary>
        ///     The on last hit callback.
        /// </summary>
        internal override void OnLastHit()
        {
            if (MyMenu.RootMenu.Item("lasthit.mode").GetValue<StringList>().SelectedIndex == 0)
            {
                var minion =
                MinionManager.GetMinions(this.MaxRange + this.Width)
                    .Where(obj => this.SpellObject.IsKillable(obj) && obj.Distance(ObjectManager.Player) > Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) && obj.Distance(ObjectManager.Player) < this.SpellObject.ChargedMaxRange).MinOrDefault(obj => obj.Health);

                if (minion != null)
                {
                    if (!this.SpellObject.IsCharging)
                    {
                        this.SpellObject.StartCharging();
                    }

                    if (this.SpellObject.IsCharging)
                    {
                        this.SpellObject.Cast(minion);
                    }
                }
            }
            else
            {
                var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, this.Range);
                if (allMinions != null)
                {
                    foreach (var minion in allMinions.Where(minion => this.SpellObject.IsKillable(minion) && minion.IsValidTarget(this.MaxRange)))
                    {
                        var killcount = 0;

                        foreach (var colminion in allMinions)
                        {
                            if (this.SpellObject.IsKillable(colminion))
                            {
                                killcount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (killcount >= MyMenu.RootMenu.Item("lasthit.count.clear").GetValue<Slider>().Value)
                        {
                            this.SpellObject.Cast(minion);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     The on lane clear callback.
        /// </summary>
        internal override void OnLaneClear()
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, this.MaxRange + this.Width);
            if (minions != null)
            {
                var minion = this.SpellObject.GetLineFarmLocation(minions, this.MaxRange + this.Width);
                if (minion.MinionsHit >= MyMenu.RootMenu.Item("lasthit.count").GetValue<Slider>().Value)
                {
                    if (minion.Position.IsValid())
                    {
                        if (!this.SpellObject.IsCharging)
                        {
                            this.SpellObject.StartCharging();
                        }

                        if (this.SpellObject.IsCharging)
                        {
                            this.SpellObject.Cast(minion.Position);
                        }
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

            if (minions != null && MyMenu.RootMenu.Item("jungleclearuse").IsActive())
            {
                if (!this.SpellObject.IsCharging)
                {
                    if (Misc.GetWStacks(minions) >= 3)
                    {
                        this.SpellObject.StartCharging();
                    }
                }

                if (this.SpellObject.IsCharging)
                {
                    this.SpellObject.Cast(minions.ServerPosition);
                }
            }
        }

        #endregion
    }
}
