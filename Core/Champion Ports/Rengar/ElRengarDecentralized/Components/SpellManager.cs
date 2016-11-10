using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElRengarDecentralized.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ElRengarDecentralized.Components.Spells;
    using ElRengarDecentralized.Enumerations;
    using ElRengarDecentralized.Utils;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

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
                this.LoadSpells(new List<ISpell>() { new SpellQ(), new SpellW(), new SpellE() });
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellManager.cs: Can not initialize the spells - {0}", e);
                throw;
            }

            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += this.Game_OnUpdate;
            Orbwalking.AfterAttack += AfterAttack;
            //Obj_AI_Base.OnBuffGain += BuffManager.Obj_AI_Base_OnBuffAdd;
            //Obj_AI_Base.OnBuffLose += BuffManager.Obj_AI_Base_OnBuffLose;
            Obj_AI_Base.OnProcessSpellCast += BuffManager.OnProcessSpellCast;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets called after attack.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="target"></param>
        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var targ = target as Obj_AI_Base;
            if (!unit.IsMe || targ == null)
            {
                return;
            }

            if (Program.Orbwalker.ActiveMode.Equals(Orbwalking.OrbwalkingMode.None) || Program.Orbwalker.ActiveMode.Equals(Orbwalking.OrbwalkingMode.LastHit))
            {
                return;
            }

            Program.Orbwalker.SetOrbwalkingPoint(Vector3.Zero);
            var comboMode = Program.Orbwalker.ActiveMode.ToString().ToLower();
            if (comboMode.Equals("laneclear") && !MyMenu.RootMenu.Item("FarmEnabled").IsActive())
            {
                return;
            }

            if (ItemManager.IsActive())
            {
                ItemManager.CastItems(targ);
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="args"></param>
        private static void OnDraw(EventArgs args)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (MyMenu.RootMenu.Item("combo.prio").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    Drawing.DrawText(
                        Drawing.Width * 0.70f,
                        Drawing.Height * 0.95f,
                        Color.Yellow,
                        "Prioritized spell: E");
                    break;
                case 1:
                    Drawing.DrawText(
                        Drawing.Width * 0.70f,
                        Drawing.Height * 0.95f,
                        Color.White,
                        "Prioritized spell: W");
                    break;
                case 2:
                    Drawing.DrawText(
                        Drawing.Width * 0.70f,
                        Drawing.Height * 0.95f,
                         Color.White,
                        "Prioritized spell: Q");
                    break;
            }
        }

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

                return MyMenu.RootMenu.Item(orbwalkerModeLower + spellSlotNameLower + "use").GetValue<bool>();

            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellManager.cs: Can not get spell active state for slot {0} - {1}", spellSlot.ToString(), e);
                throw;
            }
        }

        /// <summary>
        ///     The game on update callback.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || MenuGUI.IsChatOpen || Shop.IsOpen) return;

            // clean this soon
            switch (Program.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    {
                        this.spells.Where(spell => IsSpellActive(spell.SpellSlot, Orbwalking.OrbwalkingMode.Combo))
                            .ToList()
                            .ForEach(spell => spell.OnCombo());
                        break;
                    }

                case Orbwalking.OrbwalkingMode.Mixed:
                    {
                        this.spells.Where(spell => IsSpellActive(spell.SpellSlot, Orbwalking.OrbwalkingMode.Mixed))
                            .ToList()
                            .ForEach(spell => spell.OnMixed());
                        break;
                    }

                case Orbwalking.OrbwalkingMode.LaneClear:
                    {
                        this.spells.Where(spell => IsSpellActive(spell.SpellSlot, Orbwalking.OrbwalkingMode.LaneClear))
                            .ToList()
                            .ForEach(spell => spell.OnLaneClear());

                        this.spells.Where(spell => IsSpellActive(spell.SpellSlot, Orbwalking.OrbwalkingMode.LaneClear))
                            .ToList()
                            .ForEach(spell => spell.OnJungleClear());
                        break;
                    }

                case Orbwalking.OrbwalkingMode.LastHit:
                    {
                        this.spells.Where(spell => IsSpellActive(spell.SpellSlot, Orbwalking.OrbwalkingMode.LastHit))
                            .ToList()
                            .ForEach(spell => spell.OnLastHit());
                        break;
                    }
            }

            this.spells.ToList().ForEach(spell => spell.OnUpdate());
            PrioritizeManager.SwitchCombo();
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
