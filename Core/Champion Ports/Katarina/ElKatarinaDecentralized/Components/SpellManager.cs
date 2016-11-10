using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElKatarinaDecentralized.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ElKatarinaDecentralized.Components.Spells;
    using ElKatarinaDecentralized.Enumerations;
    using ElKatarinaDecentralized.Utils;

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

        /// <summary>
        ///     
        /// </summary>
        private static AIHeroClient KSTarget;

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
                Misc.SpellR = new SpellR();
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellManager.cs: Can not initialize the spells - {0}", e);
                throw;
            }

            Game.OnUpdate += this.Game_OnUpdate;
            GameObject.OnCreate += DaggerManager.OnCreate;
            GameObject.OnDelete += DaggerManager.OnDelete;
            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        #endregion

        #region Methods


        /// <summary>
        ///     
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender == null || !sender.IsValid || !sender.IsMe)
            {
                return;
            }

            if (args.Slot.Equals(SpellSlot.R) && MyMenu.RootMenu.Item("combo.disable.evade").IsActive())
            {
                EvadeDisabler.DisableEvade(3500);
                Logging.AddEntry(LoggingEntryTrype.Debug, "@Spellmanager.CS: Disable Evade");
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (MyMenu.RootMenu.Item("combo.disable.movement").IsActive() && sender.IsMe && (ObjectManager.Player.IsChannelingImportantSpell() || Misc.HasUltimate))
            {
                args.Process = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void KillstealHandler()
        {
            if (!MyMenu.RootMenu.Item("ks.activated").IsActive())
            {
                return;
            }

            var channeling = ObjectManager.Player.IsChannelingImportantSpell();
            if (channeling && !MyMenu.RootMenu.Item("ks.r.cancel.r").IsActive() && ObjectManager.Player.CountEnemiesInRange(Misc.SpellE.Range) < 1)
            {
                return;
            }

            if (KSTarget != null && !KSTarget.IsValidTarget(Misc.SpellE.Range))
            {
                KSTarget = null;
            }

            if (ObjectManager.Player.HealthPercent < MyMenu.RootMenu.Item("ks.rhp").GetValue<Slider>().Value)
            {
                return;
            }

            foreach (var enemy in
                HeroManager.Enemies.Where(
                    h =>
                        h.IsValidTarget(Misc.SpellE.Range + Misc.SpellQ.Range) && !h.IsZombie
                        && (KSTarget == null || KSTarget.NetworkId == h.NetworkId)).OrderBy(h => h.Health))
            {

                if (MyMenu.RootMenu.Item("ks.tower").IsActive() && enemy.UnderTurret(true))
                {
                    return;
                }

                if (Misc.SpellE.SpellObject.IsInRange(enemy))
                {
                    if (Misc.SpellQ.SpellObject.IsCastable(enemy, true) && Misc.SpellQ.SpellSlot.IsReady() && MyMenu.RootMenu.Item("ks.q").IsActive() && enemy.IsValidTarget(Misc.SpellQ.Range))
                    {
                        Misc.SpellQ.SpellObject.CastOnUnit(enemy);
                        KSTarget = enemy;
                    }

                    if (Misc.SpellE.SpellObject.IsCastable(enemy, true) && MyMenu.RootMenu.Item("ks.e").IsActive() && Misc.SpellE.SpellSlot.IsReady() && enemy.IsValidTarget(Misc.SpellE.Range))
                    {
                        Misc.SpellE.SpellObject.Cast(enemy);
                        KSTarget = enemy;
                    }

                    if (MyMenu.RootMenu.Item("ks.e").IsActive() && MyMenu.RootMenu.Item("ks.q").IsActive())
                    {
                        if (Misc.SpellE.SpellSlot.IsReady() && Misc.SpellQ.SpellSlot.IsReady())
                        {
                            if (Misc.SpellQ.SpellObject.GetDamage(enemy) + Misc.SpellE.SpellObject.GetDamage(enemy)
                                > enemy.Health && enemy.IsValidTarget(Misc.SpellQ.Range))
                            {
                                Misc.SpellE.SpellObject.Cast(enemy);
                                KSTarget = enemy;
                            }
                        }
                    }

                    if (!Misc.SpellQ.SpellObject.IsCastable(enemy, true)
                        || !Misc.SpellE.SpellObject.IsCastable(enemy, true))
                    {
                        if (enemy.GetCalculatedRDamage(MyMenu.RootMenu.Item("ks.r.ticks").GetValue<Slider>().Value) > enemy.Health && MyMenu.RootMenu.Item("ks.r").IsActive()
                       && Misc.SpellR.SpellSlot.IsReady() && enemy.IsValidTarget(Misc.SpellR.Range))
                        {
                            Misc.SpellR.SpellObject.CastOnUnit(enemy);
                            KSTarget = enemy;
                        }
                    }
                }
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

                if ((orbwalkerModeLower.Equals("lasthit")
                    && (spellSlotNameLower.Equals("e") || spellSlotNameLower.Equals("w")
                        || spellSlotNameLower.Equals("r"))) || (orbwalkerModeLower.Equals("laneclear") && (spellSlotNameLower.Equals("e") || spellSlotNameLower.Equals("w") || spellSlotNameLower.Equals("r"))))
                {
                    return false;
                }

                return MyMenu.RootMenu.Item(orbwalkerModeLower + spellSlotNameLower + "use").GetValue<bool>();
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
            KillstealHandler();
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
