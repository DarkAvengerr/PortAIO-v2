namespace ElTahmKench.Components
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using EloBuddy;
    using ElTahmKench.Components.Spells;
    using ElTahmKench.Enumerations;
    using ElTahmKench.Utils;

    using LeagueSharp;
    using LeagueSharp.Common;

    using LeagueSharp.Data;
    using LeagueSharp.Data.DataTypes;

    /// <summary>
    ///     The spell manager.
    /// </summary>
    internal class SpellManager
    {
        #region Fields

        /// <summary>
        ///     The spells.
        /// </summary>
        internal readonly List<ISpell> spells = new List<ISpell>();

        /// <summary>
        ///     Gets or sets the leaguesharp.data spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        internal List<SpellDatabaseEntry> Spells { get; } = new List<SpellDatabaseEntry>();

        /// <summary>
        ///     Gets or sets the champion spells.
        /// </summary>
        internal Dictionary<string, List<SpellSlot>> ChampionSpells { get; } = new Dictionary<string, List<SpellSlot>>();

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
                Misc.SpellW = new SpellW();
                Misc.SpellQ = new SpellQ();

                HeroManager.Allies.ForEach(x => Misc.BuffIndexesHandled.Add(x.NetworkId, new List<int>()));

                // Loads the dangerous ults from LeagueSharp.Data
                var dangerousSpells = 
                    Data.Get<SpellDatabase>()
                    .Spells.Where(
                        x => 
                        x.DangerValue >= 5 
                        && HeroManager.Enemies.Any(
                            y => x.ChampionName.Equals(y.ChampionName, StringComparison.InvariantCultureIgnoreCase)))
                    .Select(x => x.SpellName)
                    .ToList();

                // Adds the spells
                foreach (var source in dangerousSpells)
                {
                    var spellName = source;
                    this.Spells.Add(Data.Get<SpellDatabase>().Spells.First(x => x.SpellName.Equals(source)));
                    Logging.AddEntry(LoggingEntryType.Info, "Loaded spells: {0}", spellName);
                    if (!dangerousSpells.Contains(spellName))
                    {
                        continue;
                    }
                }

                foreach (var spell in this.Spells)
                {
                    if (!this.ChampionSpells.ContainsKey(spell.ChampionName))
                    {
                        this.ChampionSpells[spell.ChampionName] = new List<SpellSlot>();
                    }

                    this.ChampionSpells[spell.ChampionName].Add(spell.Slot);
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellManager.cs: Can not initialize the spells - {0}", e);
                throw;
            }

            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += this.Game_OnUpdate;
            Obj_AI_Base.OnBuffGain += this.OnBuffAdd;
            Obj_AI_Base.OnBuffLose += this.OnBuffRemove;
            Obj_AI_Base.OnProcessSpellCast += this.ObjAiBaseOnProcessSpellCast;
        }

        private static void OnDraw(EventArgs args)
        {
            if (MyMenu.RootMenu.Item("drawquse").IsActive() && Misc.SpellQ.SpellObject.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Misc.SpellQ.Range, Color.DeepSkyBlue);
            }
        }

        /// <summary>
        ///     Fired when the game processes a spell cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private void ObjAiBaseOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (!sender.IsEnemy)
                {
                    return;
                }

                if (!args.Target.IsValid<AIHeroClient>() || args.Target.IsEnemy || args.Target.IsMe || !Misc.SpellW.SpellObject.IsInRange(args.Target))
                {
                    return;
                }

                var target = (AIHeroClient)args.Target;

                if (!MyMenu.RootMenu.Item("allydangerousults").IsActive())
                {
                    return;
                }

                // todo : Generate menu items to toggle spells [optional].
                if (!this.Spells.Any(x => x.SpellName.Equals(args.SData.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return;
                }

                if (Misc.SpellW.SpellSlot.IsReady() && Misc.SpellW.SpellObject.IsInRange(target) && !target.IsMe)
                {
                    Misc.SpellW.SpellObject.CastOnUnit(target);
                    Logging.AddEntry(LoggingEntryType.Debug, "@SpellManager.cs: ObjAiBaseOnProcessSpellCast");
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellManager.cs: ObjAiBaseOnProcessSpellCast: {0}", e);
                throw;
            }
        }

        /// <summary>
        ///    The OnBuffRemove event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private void OnBuffRemove(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            try
            {
                if (!sender.IsMe)
                {
                    return;
                }

                if (args.Buff.Name.Equals(Misc.DevouredBuffName))
                {
                    Misc.LastDevouredType = DevourType.None;
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellManager.cs: OnBuffRemove: {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     The OnBuffAdd event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private void OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            try
            {
                if (args.Buff.Name.Equals(Misc.DevouredCastBuffName))
                {
                    var hero = sender as AIHeroClient;
                    if (hero != null)
                    {
                        Misc.LastDevouredType = hero.IsAlly ? DevourType.Ally : DevourType.Enemy;
                        return;
                    }

                    var minion = sender as Obj_AI_Minion;
                    if (minion != null)
                    {
                        Misc.LastDevouredType = DevourType.Minion;
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@SpellManager.cs: OnBuffAdd: {0}", e);
                throw;
            }
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

                if ((orbwalkerModeLower.Equals("lasthit")
                    && (!spellSlotNameLower.Equals("q"))) || (orbwalkerModeLower.Equals("mixed") && (spellSlotNameLower.Equals("e"))
                    || (orbwalkerModeLower.Equals("combo") && (spellSlotNameLower.Equals("e")))))
                {
                    return false;
                }

                return MyMenu.RootMenu.Item(orbwalkerModeLower + spellSlotNameLower + "use").GetValue<bool>()
                       && MyMenu.RootMenu.Item(orbwalkerModeLower + spellSlotNameLower + "mana")
                              .GetValue<Slider>()
                              .Value <= ObjectManager.Player.ManaPercent;
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
        /// <param name="args">
        ///     The args.
        /// </param>
        private void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || MenuGUI.IsChatOpen || ObjectManager.Player.IsRecalling() || ObjectManager.Player.InFountain()) return;

            this.spells.Where(spell => IsSpellActive(spell.SpellSlot, Orbwalking.OrbwalkingMode.Combo))
                .ToList()
                .ForEach(spell => spell.OnCombo());

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
