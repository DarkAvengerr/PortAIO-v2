namespace ElVarusRevamped.Components.Spells
{
    using System;
    using System.Linq;

    using ElVarusRevamped.Enumerations;
    using ElVarusRevamped.Utils;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using SharpDX;

    /// <summary>
    ///     The spell R.
    /// </summary>
    internal class SpellR : ISpell
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
        internal override float Range => 1100f;

        /// <summary>
        ///     Gets or sets the skillshot type.
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.SkillshotLine;

        /// <summary>
        ///     Gets the speed.
        /// </summary>
        internal override float Speed => 1950f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.R;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width => 120f;

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
                    if (MyMenu.RootMenu.Item("combousermultiple").IsActive())
                    {
                        var spreadRadius = MyMenu.RootMenu.Item("combor.r.radius").GetValue<Slider>().Value;
                        var enemiesHit = HeroManager.Enemies.Where(e => e.Distance(target) <= spreadRadius && !e.IsDead).ToList();
                        if (enemiesHit.Count >= MyMenu.RootMenu.Item("combor.count").GetValue<Slider>().Value)
                        {
                            this.SpellObject.Cast(target);
                        }
                    }

                    if (MyMenu.RootMenu.Item("combousersolo").IsActive())
                    {
                        if (target.HealthPercent
                            < MyMenu.RootMenu.Item("combor.count.solo").GetValue<Slider>().Value)
                        {

                            var alliesCount = 
                                HeroManager.Allies.Count(
                                    a => a.IsValid && !a.IsDead && !a.IsMe
                                        && a.Distance(ObjectManager.Player.Position.Extend(
                                                target.Position,
                                                ObjectManager.Player.Distance(target) / 2f)) <= MyMenu.RootMenu.Item("combor.r.allies.range").GetValue<Slider>().Value);

                            var enemiesCount =
                                HeroManager.Enemies.Count(
                                    h =>
                                        h.IsValid && !h.IsDead && h.IsVisible && h.NetworkId != target.NetworkId
                                        && h.Distance(
                                            ObjectManager.Player.Position.Extend(
                                                target.Position,
                                                ObjectManager.Player.Distance(target) / 2f)) <= MyMenu.RootMenu.Item("combor.r.enemies.range").GetValue<Slider>().Value);

                            if (!MyMenu.RootMenu.Item("combo" + target.ChampionName + "use").IsActive())
                            {
                                return;
                            }

                            if (alliesCount > MyMenu.RootMenu.Item("combor.count.allies").GetValue<Slider>().Value || enemiesCount > MyMenu.RootMenu.Item("combor.count.enemies").GetValue<Slider>().Value)
                            {
                                return;
                            }

                            this.SpellObject.Cast(target);
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
        ///     The on update callback.
        /// </summary>
        internal override void OnUpdate()
        {
            var target = Misc.GetTarget(this.Range + this.Width, this.DamageType);
            if (target != null)
            {
                if (this.SpellObject.IsReady() && target.IsValidTarget(this.Range))
                {
                    if (MyMenu.RootMenu.Item("combo.semi.r").GetValue<KeyBind>().Active)
                    {
                        this.SpellObject.Cast(target);
                    }
                }
            }
        }

        /// <summary>
        ///     The on mixed callback.
        /// </summary>
        internal override void OnMixed()
        {
            this.OnCombo();
        }

        #endregion
    }
}