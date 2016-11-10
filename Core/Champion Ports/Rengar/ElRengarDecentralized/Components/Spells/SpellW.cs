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
    ///     The spell W.
    /// </summary>
    internal class SpellW : ISpell
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the damage type.
        /// </summary>
        internal override TargetSelector.DamageType DamageType => TargetSelector.DamageType.Magical;

        /// <summary>
        ///     Gets the targeted type.
        /// </summary>
        internal override bool Targeted => true;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 500f;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width => 400f; // check the radius

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.W;

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
                         && MyMenu.RootMenu.Item("combo.prio").GetValue<StringList>().SelectedIndex != 1)
                    {
                        return;
                    }

                    if (ObjectManager.Player.Distance(target) < this.Range + this.Width)
                    {
                        this.SpellObject.Cast();
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
        ///     The on mixed callback.
        /// </summary>
        internal override void OnMixed()
        {
            this.OnCombo();
        }

        /// <summary>
        /// The on update callback.
        /// </summary>
        internal override void OnUpdate()
        {
            if (Misc.GetFerocityStacks() == 4)
            {
                foreach (var buff in ObjectManager.Player.Buffs.Where(x => BuffManager.Buffs.Contains(x.Type) && x.Caster.Type == GameObjectType.AIHeroClient && x.Caster.IsEnemy))
                {
                    if (!MyMenu.RootMenu.Item($"Cleanse{buff.Type}").IsActive() || MyMenu.RootMenu.Item("MinDuration").GetValue<Slider>().Value / 1000f > buff.EndTime - buff.StartTime)
                    {
                        continue;
                    }

                    this.SpellObject.Cast();
                }
            }
        }

        /// <summary>
        ///     The on lane clear callback.
        /// </summary>
        internal override void OnLaneClear()
        {
            var minions =
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    this.SpellObject.Range + this.SpellObject.Width).ToList();

            if (minions.Count == 0)
            {
                return;
            }

            if (minions.Any(x => x.Distance(ObjectManager.Player) < this.Range + this.Width) && minions.Count >= MyMenu.RootMenu.Item("laneclear.w.hit").GetValue<Slider>().Value)
            {
                this.SpellObject.Cast();
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
                        Logging.AddEntry(LoggingEntryType.Debug, "@SpellW.cs: Range too big.");
                        return;
                    }
                }

                this.SpellObject.Cast();
            }
        }

        #endregion
    }
}