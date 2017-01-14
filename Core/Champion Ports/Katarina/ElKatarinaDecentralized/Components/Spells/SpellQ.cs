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

    using Color = System.Drawing.Color;

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

                var target =
                    HeroManager.Enemies.Where(x => x.IsValidTarget(this.Range) && !x.IsDead && !x.IsZombie)
                        .OrderBy(x => x.Health)
                        .FirstOrDefault();

                if (target != null)
                {
                    if (MyMenu.RootMenu.Item("combo.q.units").IsActive())
                    {
                        var minions = MinionManager.GetMinions(850f);
                        var countCloseToTarget = minions.Count(m => m.Distance(target) <= 570f);

                        if (target.Distance(ObjectManager.Player) < 750f)
                        {
                            this.SpellObject.CastOnUnit(target);
                            this.SpellObject.LastCastAttemptT = Utils.TickCount;
                        }

                        if (countCloseToTarget >= 3)
                        {
                            var bestMinion =
                                MinionManager.GetMinions(target.ServerPosition, 550f)
                                    .OrderBy(x => x.Distance(target))
                                    .FirstOrDefault();

                            if (bestMinion != null)
                            {
                                this.SpellObject.CastOnUnit(bestMinion);
                                this.SpellObject.LastCastAttemptT = Utils.TickCount;
                            }
                        }

                        if (countCloseToTarget < 3)
                        {
                            this.SpellObject.CastOnUnit(target);
                        }

                        if (target.CountEnemiesInRange(600f) >= 2)
                        {
                            this.SpellObject.CastOnUnit(target);
                            this.SpellObject.LastCastAttemptT = Utils.TickCount;
                        }
                    }
                    else
                    {
                        this.SpellObject.CastOnUnit(target);
                        this.SpellObject.LastCastAttemptT = Utils.TickCount;
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
            var target = Misc.GetTarget(this.Range, this.DamageType);
            if (target != null)
            {
                this.SpellObject.CastOnUnit(target);
                this.SpellObject.LastCastAttemptT = Utils.TickCount;
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