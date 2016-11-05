namespace ElNamiDecentralized.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ElNamiDecentralized.Components.Spells;
    using ElNamiDecentralized.Enumerations;
    using ElNamiDecentralized.Utils;

    using EloBuddy;
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
                this.LoadSpells(new List<ISpell>() { new SpellQ(), new SpellW(), new SpellE(), new SpellR() });
                Misc.SpellQ = new SpellQ();
                Misc.SpellE = new SpellE();
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellManager.cs: Can not initialize the spells - {0}", e);
                throw;
            }

            Game.OnUpdate += this.Game_OnUpdate;
            Orbwalking.BeforeAttack += this.BeforeAttack;
            GameObject.OnCreate += ObjSpellMissileOnOnCreate;
            AntiGapcloser.OnEnemyGapcloser += this.OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += this.OnInterruptableTarget;
        }

        /// <summary>
        ///     Called Before attack.
        /// </summary>
        /// <param name="args"></param>
        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target is Obj_AI_Minion)
            {
                if (MyMenu.RootMenu.Item("support.mode").IsActive())
                {
                    var allyCheck = HeroManager.Allies.Count(a => a.Distance(ObjectManager.Player) <= 1500 && !a.IsMe);
                    if (allyCheck > 0)
                    {
                        args.Process = false;
                    }
                }
            }
        }

        #endregion

        #region Methods


        /// <summary>
        ///     Called on missle creation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void ObjSpellMissileOnOnCreate(GameObject sender, EventArgs args)
        {
            if (!Misc.SpellE.SpellSlot.IsReady() || !MyMenu.RootMenu.Item("tide.activated").IsActive())
            {
                return;
            }

            if ((MyMenu.RootMenu.Item("tide.mode").GetValue<StringList>().SelectedIndex == 1
                 && Program.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo))
            {
                return;
            }

            if (!sender.IsValid<MissileClient>())
            {
                return;
            }

            // Credits to H3H3.
            var missile = (MissileClient)sender;
            if (!missile.SpellCaster.IsValid<AIHeroClient>() || !missile.SpellCaster.IsAlly || missile.SpellCaster.IsMe
                || missile.SpellCaster.IsMelee())
            {
                return;
            }

            if (!missile.Target.IsValid<AIHeroClient>() || !missile.Target.IsEnemy)
            {
                return;
            }

            var caster = (AIHeroClient)missile.SpellCaster;
            if (ObjectManager.Player.ManaPercent > MyMenu.RootMenu.Item("tide.mana").GetValue<Slider>().Value && Misc.SpellE.SpellObject.IsInRange(caster))
            {
                Misc.SpellE.SpellObject.CastOnUnit(caster);
            }
        }

        /// <summary>
        ///        Called on gapcloser.
        /// </summary>
        /// <param name="gapcloser"></param>
        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            try
            {
                if (!Misc.SpellQ.SpellSlot.IsReady() || !MyMenu.RootMenu.Item("gapcloser.q").IsActive())
                {
                    return;
                }

                if (ObjectManager.Player.ManaPercent
                    <= MyMenu.RootMenu.Item("interrupt.q.mana").GetValue<Slider>().Value)
                {
                    return;
                }

                if (gapcloser.End.Distance(ObjectManager.Player.Position) <= Misc.SpellQ.Range)
                {
                    Misc.SpellQ.SpellObject.Cast(gapcloser.End);
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellManager.cs: AntiGapcloser - {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     OnInterruptableTarget.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            try
            {
                if (!Misc.SpellQ.SpellSlot.IsReady() || !MyMenu.RootMenu.Item("interrupt.q").IsActive())
                {
                    return;
                }

                if (ObjectManager.Player.ManaPercent
                    <= MyMenu.RootMenu.Item("interrupt.q.mana").GetValue<Slider>().Value)
                {
                    return;
                }

                if (args.DangerLevel != Interrupter2.DangerLevel.High || !sender.IsValidTarget(Misc.SpellQ.Range))
                {
                    return;
                }

                Misc.SpellQ.SpellObject.Cast(sender);
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellManager.cs: OnInterruptableTarget - {0}", e);
                throw;
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

                if ((orbwalkerModeLower.Equals("mixed")
                    && (spellSlotNameLower.Equals("r"))))
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
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellManager.cs: Can not get spell active state for slot {0} - {1}", spellSlot.ToString(), e);
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
