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
    ///     The spell E.
    /// </summary>
    internal class SpellE : ISpell
    {
        #region Properties

        /// <summary>
        ///     Gets the targeted mode.
        /// </summary>
        internal override bool Targeted => true;

        /// <summary>
        ///     Gets or sets the damage type.
        /// </summary>
        internal override TargetSelector.DamageType DamageType => TargetSelector.DamageType.Magical;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 725f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.E;

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
                    if (MyMenu.RootMenu.Item("combo.e.tower").IsActive() && target.UnderTurret(true))
                    {
                        return;
                    }

                    if (ObjectManager.Player.IsChannelingImportantSpell())
                    {
                        return;
                    }

                    if (DaggerManager.ExistingDaggers != null)
                    {
                        var dagger = DaggerManager.ExistingDaggers;
                        var closestDagger = dagger.FirstOrDefault(
                                d =>
                                    d.DaggerPos.Distance(target.ServerPosition) <= d.Object.BoundingRadius + 200
                                    && d.Object.IsValid && d.Object.IsVisible);

                        if (dagger != null)
                        {
                            if (closestDagger != null && Utils.TickCount - this.SpellObject.LastCastAttemptT > 0)
                            {
                                this.SpellObject.Cast(closestDagger.DaggerPos);
                                this.SpellObject.LastCastAttemptT = Utils.TickCount;

                                if (!MyMenu.RootMenu.Item("combo.e.daggers").IsActive())
                                {
                                    if (target.Distance(closestDagger.DaggerPos) > 500f)
                                    {
                                        this.SpellObject.Cast(target.Position);
                                        this.SpellObject.LastCastAttemptT = Utils.TickCount;
                                    }
                                }
                            }

                            if (closestDagger == null)
                            {
                                if (!MyMenu.RootMenu.Item("combo.e.daggers").IsActive() && Utils.TickCount - this.SpellObject.LastCastAttemptT > 500 
                                    && Utils.TickCount - Misc.SpellQ.SpellObject.LastCastAttemptT > 750)
                                {
                                    this.SpellObject.Cast(target.Position);
                                    this.SpellObject.LastCastAttemptT = Utils.TickCount;
                                }
                            }
                        }
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
            var target = Misc.GetTarget(this.Range, this.DamageType);
            if (target != null)
            {
                if (MyMenu.RootMenu.Item("combo.e.tower").IsActive() && target.UnderTurret(true))
                {
                    return;
                }

                if (ObjectManager.Player.IsChannelingImportantSpell())
                {
                    return;
                }

                if (DaggerManager.ExistingDaggers != null)
                {
                    var dagger =
                        DaggerManager.ExistingDaggers
                            .FirstOrDefault(
                                d =>
                                    d.DaggerPos.Distance(target.ServerPosition)
                                    <= d.Object.BoundingRadius + 200 && d.Object.IsValid);

                    if (dagger != null)
                    {
                        this.SpellObject.Cast(dagger.DaggerPos);
                    }
                }
            }
        }

        /// <summary>
        ///     The on update callback.
        /// </summary>
        internal override void OnUpdate()
        {
            if (MyMenu.RootMenu.Item("wardjump.key").GetValue<KeyBind>().Active)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 250), false);
                FleeManager.JumpHandler(Game.CursorPos);
            }
        }

        #endregion
    }
}