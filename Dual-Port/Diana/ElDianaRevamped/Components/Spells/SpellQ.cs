namespace ElDianaRevamped.Components.Spells
{
    using System;
    using System.Linq;

    using ElDianaRevamped.Enumerations;
    using ElDianaRevamped.Utils;
    using EloBuddy;

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
        ///     Gets the delay.
        /// </summary>
        internal override float Delay => 250f;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 850f;

        /// <summary>
        ///     Gets or sets the skillshot type.
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.SkillshotCircle;

        /// <summary>
        ///     Gets the speed.
        /// </summary>
        internal override float Speed => 1400f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.Q;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width => 195f; // end point is 195 the other line is 50

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

                var target = Misc.GetTarget(this.Range + this.Width, this.DamageType);
                if (target != null)
                {
                    var pred = Prediction.GetPrediction(target, this.Delay, target.Distance(ObjectManager.Player) < 500 ? 100 : this.Width, this.Speed);
                    if (pred.Hitchance >= HitChance.VeryHigh)
                    {
                        this.SpellObject.Cast(pred.CastPosition);
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
                if (MyMenu.RootMenu.Item("lasthit.mode").GetValue<StringList>().SelectedIndex == 0)
                {
                    this.SpellObject.Cast(minion);
                }
                else
                {
                    if (Vector3.Distance(minion.ServerPosition, ObjectManager.Player.ServerPosition)
                        > Orbwalking.GetRealAutoAttackRange(ObjectManager.Player)
                        && ObjectManager.Player.Distance(minion) <= this.Range)
                    {
                        this.SpellObject.Cast(minion);
                    }
                }
            }
        }

        /// <summary>
        ///     The on lane clear callback.
        /// </summary>
        internal override void OnLaneClear()
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, this.SpellObject.Range + this.SpellObject.Width);
            var minion = this.SpellObject.GetCircularFarmLocation(minions, this.SpellObject.Width);
            var minionsHit = minion.MinionsHit;

            if (minions != null)
            {
                if (minionsHit >= MyMenu.RootMenu.Item("lasthit.count").GetValue<Slider>().Value)
                {
                    this.SpellObject.Cast(minion.Position);
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