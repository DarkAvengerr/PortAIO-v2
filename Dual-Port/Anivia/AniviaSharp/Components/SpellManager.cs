// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpellManager.cs" company="LeagueSharp">
//   legacy@joduska.me
// </copyright>
// <summary>
//   The spell manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace AniviaSharp.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AniviaSharp.Components.Spells;
    using AniviaSharp.Enumerations;
    using AniviaSharp.Utils;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The spell manager.
    /// </summary>
    internal class SpellManager
    {
        #region Fields

        /// <summary>
        ///     The spells.
        /// </summary>
        private readonly List<ISpell> spells = new List<ISpell>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpellManager" /> class.
        /// </summary>
        internal SpellManager()
        {
            try
            {
                this.LoadSpells(new List<ISpell>() { new SpellR(), new SpellE(), new SpellQ() });
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellManager.cs: Can not initialize the spells - {0}", e);
                throw;
            }

            Game.OnUpdate += this.Game_OnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The is the spell active method.
        /// </summary>
        /// <param name="spellSlot">
        ///     The spell slot.
        /// </param>
        /// <param name="orbwalkingMode">
        ///     The orbwalking mode.
        /// </param>
        /// <returns>
        ///     <see cref="bool" />
        /// </returns>
        private static bool IsSpellActive(SpellSlot spellSlot, Orbwalking.OrbwalkingMode orbwalkingMode)
        {
            if (Program.Orbwalker.ActiveMode != orbwalkingMode || !spellSlot.IsReady())
            {
                return false;
            }

            try
            {
                var orbwalkerModeLower = Program.Orbwalker.ActiveMode.ToString().ToLower();
                var spellSlotNameLower = spellSlot.ToString().ToLower();

                return MyMenu.RootMenu.Item(orbwalkerModeLower + spellSlotNameLower + "use").GetValue<bool>()
                       && MyMenu.RootMenu.Item(orbwalkerModeLower + spellSlotNameLower + "mana")
                              .GetValue<Slider>()
                              .Value <= ObjectManager.Player.ManaPercent;
            }
            catch (Exception e)
            {
                Logging.AddEntry(
                    LoggingEntryTrype.Error, 
                    "@SpellManager.cs: Can not get spell active state for slot {0} - {1}", 
                    spellSlot.ToString(), 
                    e);
                throw;
            }
        }

        /// <summary>
        ///     The game on update callback.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        private void Game_OnUpdate(EventArgs args)
        {
            this.spells.Where(spell => IsSpellActive(spell.SpellSlot, Orbwalking.OrbwalkingMode.Combo))
                .ToList()
                .ForEach(spell => spell.OnCombo());

            this.spells.Where(spell => IsSpellActive(spell.SpellSlot, Orbwalking.OrbwalkingMode.LaneClear))
                .ToList()
                .ForEach(spell => spell.OnLaneClear());

            this.spells.Where(spell => IsSpellActive(spell.SpellSlot, Orbwalking.OrbwalkingMode.LastHit))
                .ToList()
                .ForEach(spell => spell.OnLastHit());

            this.spells.Where(spell => IsSpellActive(spell.SpellSlot, Orbwalking.OrbwalkingMode.Mixed))
                .ToList()
                .ForEach(spell => spell.OnMixed());

            this.spells.ToList().ForEach(spell => spell.OnUpdate());
        }

        /// <summary>
        ///     The load spells method.
        /// </summary>
        /// <param name="spellList">
        ///     The spells.
        /// </param>
        private void LoadSpells(IEnumerable<ISpell> spellList)
        {
            foreach (var spell in spellList)
            {
                MyMenu.GenerateSpellMenu(spell.SpellSlot);
                this.spells.Add(spell);
            }
        }

        #endregion
    }
}