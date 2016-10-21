namespace ElTahmKench.Components.Spells
{
    using System;
    using System.Linq;

    using ElTahmKench.Enumerations;
    using ElTahmKench.Utils;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using SharpDX;

    /// <summary>
    ///     The spell W.
    /// </summary>
    internal class SpellW : ISpell
    {
        #region Properties

        public float minionWRange = 700f;

        /// <summary>
        ///     Gets a value indicating whether the combo mode is active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if combo mode is active; otherwise, <c>false</c>.
        /// </value>
        public bool ComboModeActive => Orbwalking.Orbwalker.Instances.Any(x => x.ActiveMode == Orbwalking.OrbwalkingMode.Combo);

        /// <summary>
        ///     Gets or sets the damage type.
        /// </summary>
        internal override TargetSelector.DamageType DamageType => TargetSelector.DamageType.Magical;

        /// <summary>
        ///     Gets the delay.
        /// </summary>
        internal override float Delay => 0.5f;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range
            =>
            Misc.HasDevouredBuff && Misc.LastDevouredType == DevourType.Minion
                ? 700f
                : 250f;
        /// <summary>
        ///     Gets or sets the skillshot type.
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.SkillshotLine;

        /// <summary>
        ///     Gets the speed.
        /// </summary>
        internal override float Speed => 950f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.W;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width => 75f;

        /// <summary>
        ///     Spell has collision.
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
                var target = HeroManager.Enemies.Where(x => x.IsValidTarget(this.Range) && Misc.GetPassiveStacks(x) == 3 && (!x.IsInvulnerable || !x.MagicImmune))
                    .OrderBy(obj => obj.Distance(ObjectManager.Player.ServerPosition))
                    .FirstOrDefault();

                if (target == null)
                {
                    return;
                }

                if (ObjectManager.Player.Distance(target) + target.BoundingRadius <= this.Range + ObjectManager.Player.BoundingRadius && Misc.LastDevouredType == DevourType.None)
                {
                    this.SpellObject.CastOnUnit(target);
                }
                else
                {
                    if (MyMenu.RootMenu.Item("combominionuse").IsActive())
                    {
                        if (!target.IsValidTarget(this.Range + 400) || (Misc.HasDevouredBuff && Misc.LastDevouredType != DevourType.Minion))
                        {
                            return;
                        }

                        // Check if the Player does not have the devourer buff.
                        if (!Misc.HasDevouredBuff)
                        {
                            // Get the minions in range
                            var minion = MinionManager.GetMinions(this.Range, team: MinionTeam.NotAlly).Where(n => !n.CharData.BaseSkinName.ToLower().Contains("spiderling")).OrderBy(obj => obj.Distance(ObjectManager.Player.ServerPosition)).FirstOrDefault();
                            // Check if there are any minions.
                            if (minion != null)
                            {
                                // Cast W on the minion.
                                this.SpellObject.CastOnUnit(minion);
                            }
                        }
                        // Check if player has the devoured buff and that the last devoured type is a minion.
                        else if (Misc.HasDevouredBuff && Misc.LastDevouredType == DevourType.Minion)
                        {
                            var prediction = this.SpellObject.GetPrediction(target);
                            if (prediction.Hitchance >= HitChance.High)
                            {
                                // Spit the minion to the target location.
                                this.SpellObject.Cast(prediction.CastPosition);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellW.cs: Can not run OnCombo - {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     The on update callback.
        /// </summary>
        internal override void OnUpdate()
        {
            if (!this.SpellSlot.IsReady())
            {
                return;
            }

            if (MyMenu.RootMenu.Item("allylowhpults").IsActive())
            {
                foreach (var ally in HeroManager.Allies.Where(a => a.Distance(ObjectManager.Player) < 500f && !a.IsDead && !a.IsZombie && !a.IsMe))
                {
                    if (ObjectManager.Player.CountEnemiesInRange(900f) > 0 && ally.HealthPercent <= MyMenu.RootMenu.Item("allylowhpultsslider").GetValue<Slider>().Value)
                    {
                        if (MyMenu.RootMenu.Item("walktotarget").IsActive())
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, ally.ServerPosition);
                        }

                        if (this.SpellObject.IsInRange(ally))
                        {
                            this.SpellObject.CastOnUnit(ally);
                        }
                    }
                }
            }

            if (!MyMenu.RootMenu.Item("allycc").IsActive())
            {
                return;
            }

            foreach (var ally in HeroManager.Allies.Where(a => a.Distance(ObjectManager.Player) < 500f && !a.IsMe))
            {
                foreach (var buff in
                    ally.Buffs.Where(
                        x =>
                            Misc.DevourerBuffTypes.Contains(x.Type) && x.Caster.Type == GameObjectType.AIHeroClient && x.Caster.IsEnemy))
                {
                    if (!MyMenu.RootMenu.Item($"buffscc{buff.Type}").IsActive() || !MyMenu.RootMenu.Item($"won{ally.ChampionName}").IsActive() || Misc.BuffIndexesHandled[ally.NetworkId].Contains(buff.Index) || !this.SpellSlot.IsReady())
                    {
                        continue;
                    }

                    Misc.BuffIndexesHandled[ally.NetworkId].Add(buff.Index);
                    if (MyMenu.RootMenu.Item("walktotarget").IsActive())
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, ally.ServerPosition);
                    }

                    if (this.SpellObject.IsInRange(ally))
                    {
                        this.SpellObject.CastOnUnit(ally);
                    }

                    Misc.BuffIndexesHandled[ally.NetworkId].Remove(buff.Index);
                }
            }
        }

        /// <summary>
        ///     The on mixed callback.
        /// </summary>
        internal override void OnMixed()
        {
            // Check if the Player does not have the devourer buff.
            if (!Misc.HasDevouredBuff)
            {
                // Gets the minion in melee range.
                var minion = MinionManager.GetMinions(this.Range, team: MinionTeam.NotAlly).OrderBy(obj => obj.Distance(ObjectManager.Player.ServerPosition)).FirstOrDefault();
                // check if there are any minions.
                if (minion != null)
                {
                    // Cast W on the minion.
                    this.SpellObject.CastOnUnit(minion);
                }
            }
            // Check if player has the devoured buff and that the last devoured type is a minion.
            else if (Misc.HasDevouredBuff && Misc.LastDevouredType == DevourType.Minion)
            {
                var target =
                    HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(this.minionWRange));

                if (target != null)
                {
                    var prediction = this.SpellObject.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        // Spit the minion to the target location.
                        this.SpellObject.Cast(prediction.CastPosition);
                    }
                }
            }
        }

        #endregion
    }
}