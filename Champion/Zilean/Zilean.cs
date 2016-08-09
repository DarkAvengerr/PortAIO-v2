using EloBuddy; namespace ElZilean
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Net;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Zilean
    {
        #region Constructors and Destructors

        static Zilean()
        {
            Spells = new List<InitiatorSpell>
                         {
                             new InitiatorSpell { ChampionName = "Monkeyking", SDataName = "monkeykingnimbus" },
                             new InitiatorSpell { ChampionName = "Monkeyking", SDataName = "monkeykingdecoy" },
                             new InitiatorSpell { ChampionName = "Monkeyking", SDataName = "monkeykingspintowin" },
                             new InitiatorSpell { ChampionName = "Olaf", SDataName = "olafragnarok" },
                             new InitiatorSpell { ChampionName = "Gragas", SDataName = "gragase" },
                             new InitiatorSpell { ChampionName = "Hecarim", SDataName = "hecarimult" },
                             new InitiatorSpell { ChampionName = "Hecarim", SDataName = "HecarimRamp" },
                             new InitiatorSpell { ChampionName = "Ekko", SDataName = "ekkoe" },
                             new InitiatorSpell { ChampionName = "Malphite", SDataName = "ufslash " },
                             new InitiatorSpell { ChampionName = "Vi", SDataName = "viq" },
                             new InitiatorSpell { ChampionName = "Vi", SDataName = "vir" },
                             new InitiatorSpell { ChampionName = "Volibear", SDataName = "volibearq" },
                             new InitiatorSpell { ChampionName = "Lissandra", SDataName = "lissandrae" },
                             new InitiatorSpell { ChampionName = "Gnar", SDataName = "gnare" },
                             new InitiatorSpell { ChampionName = "Fiora", SDataName = "fioraq" },
                             new InitiatorSpell { ChampionName = "Sion", SDataName = "sionr" },
                             new InitiatorSpell { ChampionName = "Zac", SDataName = "zace" },
                             new InitiatorSpell { ChampionName = "KhaZix", SDataName = "khazixe" },
                             new InitiatorSpell { ChampionName = "KhaZix", SDataName = "khazixelong" },
                             new InitiatorSpell { ChampionName = "Kennen", SDataName = "kennenlightningrush" },
                             new InitiatorSpell { ChampionName = "Jax", SDataName = "jaxleapstrike" },
                             new InitiatorSpell { ChampionName = "Leona", SDataName = "leonazenithblademissle" },
                             new InitiatorSpell { ChampionName = "Shen", SDataName = "shene" },
                             new InitiatorSpell { ChampionName = "Ryze", SDataName = "ryzer" },
                             new InitiatorSpell { ChampionName = "Lucian", SDataName = "luciane" },
                             new InitiatorSpell { ChampionName = "Elise", SDataName = "elisespidereinitial" },
                             new InitiatorSpell { ChampionName = "Diana", SDataName = "dianateleport" },
                             new InitiatorSpell { ChampionName = "Akali", SDataName = "akalishadowdance" },
                             new InitiatorSpell { ChampionName = "Renekton", SDataName = "renektonsliceanddice" },
                             new InitiatorSpell { ChampionName = "Thresh", SDataName = "threshqleap" },
                             new InitiatorSpell { ChampionName = "Rengar", SDataName = "rengarr" },
                             new InitiatorSpell { ChampionName = "Shyvana", SDataName = "shyvanatransformcast" },
                             new InitiatorSpell { ChampionName = "Shyvana", SDataName = "shyvanatransformleap" },
                             new InitiatorSpell { ChampionName = "Shyvana", SDataName = "ShyvanaImmolationAura" },
                             new InitiatorSpell { ChampionName = "Udyr", SDataName = "udyrbearstance" },
                             new InitiatorSpell { ChampionName = "Kassadin", SDataName = "riftwalk" },
                             new InitiatorSpell { ChampionName = "JarvanIV", SDataName = "jarvanivdragonstrike" },
                             new InitiatorSpell { ChampionName = "Irelia", SDataName = "ireliagatotsu" },
                             new InitiatorSpell { ChampionName = "DrMundo", SDataName = "Sadism" },
                             new InitiatorSpell { ChampionName = "MasterYi", SDataName = "Highlander" },
                             new InitiatorSpell { ChampionName = "Shaco", SDataName = "Deceive" },
                             new InitiatorSpell { ChampionName = "Ahri", SDataName = "AhriTumble" },
                             new InitiatorSpell { ChampionName = "LeeSin", SDataName = "blindmonkqtwo" },
                             new InitiatorSpell { ChampionName = "Yasuo", SDataName = "yasuorknockupcombow" },
                             new InitiatorSpell { ChampionName = "Evelynn", SDataName = "evelynnw" },
                             new InitiatorSpell { ChampionName = "FiddleSticks", SDataName = "Crowstorm" },
                             new InitiatorSpell { ChampionName = "Sivir", SDataName = "SivirR" }
                         };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        public static List<InitiatorSpell> Spells { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the E spell
        /// </summary>
        /// <value>
        ///     The E spell
        /// </value>
        private static Spell E { get; set; }

        /// <summary>
        ///     Check if Zilean has speed passive
        /// </summary>
        private static bool HasSpeedBuff => Player.Buffs.Any(x => x.Name.ToLower().Contains("timewarp"));

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The Smitespell
        /// </value>
        private static Spell IgniteSpell { get; set; }

        /// <summary>
        ///     Gets or sets the menu
        /// </summary>
        /// <value>
        ///     The menu
        /// </value>
        private static Menu Menu { get; set; }

        /// <summary>
        ///     Gets or sets the orbwalker
        /// </summary>
        /// <value>
        ///     The orbwalker
        /// </value>
        private static Orbwalking.Orbwalker Orbwalker { get; set; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private static AIHeroClient Player => ObjectManager.Player;

        /// <summary>
        ///     Gets or sets the Q spell
        /// </summary>
        /// <value>
        ///     The Q spell
        /// </value>
        private static Spell Q { get; set; }

        /// <summary>
        ///     Gets or sets the R spell.
        /// </summary>
        /// <value>
        ///     The R spell
        /// </value>
        private static Spell R { get; set; }

        /// <summary>
        ///     Gets or sets the W spell
        /// </summary>
        /// <value>
        ///     The W spell
        /// </value>
        private static Spell W { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Fired when the game loads.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnGameLoad()
        {
            try
            {
                if (Player.ChampionName != "Zilean")
                {
                    return;
                }

                var igniteSlot = Player.GetSpellSlot("summonerdot");
                if (igniteSlot != SpellSlot.Unknown)
                {
                    IgniteSpell = new Spell(igniteSlot, 600f);
                }

                foreach (var ally in HeroManager.Allies)
                {
                    IncomingDamageManager.AddChampion(ally);
                }

                IncomingDamageManager.Skillshots = true;

                Q = new Spell(SpellSlot.Q, 900f - 100f);
                W = new Spell(SpellSlot.W, Orbwalking.GetRealAutoAttackRange(Player));
                E = new Spell(SpellSlot.E, 700f);
                R = new Spell(SpellSlot.R, 900f);

                Q.SetSkillshot(0.7f, 140f - 25f, int.MaxValue, false, SkillshotType.SkillshotCircle);

                GenerateMenu();

                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
                Orbwalking.BeforeAttack += BeforeAttack;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the menu
        /// </summary>
        /// <value>
        ///     Creates the menu
        /// </value>
        private static void GenerateMenu()
        {
            try
            {
                Menu = new Menu("ElZilean", "ElZilean", true);

                var targetselectorMenu = new Menu("Target Selector", "Target Selector");
                {
                    TargetSelector.AddToMenu(targetselectorMenu);
                }

                Menu.AddSubMenu(targetselectorMenu);

                var orbwalkMenu = new Menu("Orbwalker", "Orbwalker");
                {
                    Orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);
                }

                Menu.AddSubMenu(orbwalkMenu);

                var comboMenu = new Menu("Combo", "Combo");
                {
                    comboMenu.AddItem(new MenuItem("ElZilean.Combo.Q", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElZilean.Combo.Focus.Bomb", "Focus target with Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElZilean.Combo.W", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElZilean.Combo.W2", "Always reset Q").SetValue(false))
                        .SetTooltip("Always reset Q even when the target is not marked");
                    comboMenu.AddItem(new MenuItem("ElZilean.Combo.E", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElZilean.Ignite", "Use Ignite").SetValue(true));
                    comboMenu.AddItem(
                        new MenuItem("ElZilean.DoubleBombMouse", "Double bomb to mouse").SetValue(
                            new KeyBind("Y".ToCharArray()[0], KeyBindType.Press)));
                }

                Menu.AddSubMenu(comboMenu);

                var harassMenu = new Menu("Harass", "Harass");
                {
                    harassMenu.AddItem(new MenuItem("ElZilean.Harass.Q", "Use Q").SetValue(true));
                    harassMenu.AddItem(new MenuItem("ElZilean.Harass.W", "Use W").SetValue(true));
                }
                Menu.AddSubMenu(harassMenu);

                var ultimateMenu = new Menu("Ultimate", "Ultimate");
                {
                    ultimateMenu.AddItem(new MenuItem("min-health", "Health percentage").SetValue(new Slider(20, 1)));
                    ultimateMenu.AddItem(
                        new MenuItem("min-damage", "Heal on % incoming damage").SetValue(new Slider(20, 1)));
                    ultimateMenu.AddItem(new MenuItem("ElZilean.Ultimate.R", "Use R").SetValue(true));
                    ultimateMenu.AddItem(new MenuItem("blank-line", ""));
                    foreach (var x in HeroManager.Allies)
                    {
                        ultimateMenu.AddItem(new MenuItem($"R{x.ChampionName}", "Use R on " + x.ChampionName))
                            .SetValue(true);
                    }
                }
                Menu.AddSubMenu(ultimateMenu);

                var laneclearMenu = new Menu("Laneclear", "Laneclear");
                {
                    laneclearMenu.AddItem(new MenuItem("ElZilean.laneclear.Q", "Use Q").SetValue(true));
                    laneclearMenu.AddItem(new MenuItem("ElZilean.laneclear.QMouse", "Cast Q to mouse").SetValue(false))
                        .SetTooltip("Cast Q towards your mouse position");
                    laneclearMenu.AddItem(new MenuItem("ElZilean.laneclear.W", "Use W").SetValue(true));
                    laneclearMenu.AddItem(
                        new MenuItem("ElZilean.laneclear.Mana", "Minimum mana").SetValue(new Slider(20)));
                }

                Menu.AddSubMenu(laneclearMenu);

                var initiatorMenu = new Menu("Initiators", "Initiators");
                {
                    // todo filter out champs that have no speed stuff
                    foreach (var ally in HeroManager.Allies)
                    {
                        initiatorMenu.AddItem(
                            new MenuItem($"Initiator{ally.CharData.BaseSkinName}", "Initiator E: " + ally.ChampionName))
                            .SetValue(true);
                    }
                }

                Menu.AddSubMenu(initiatorMenu);

                var fleeMenu = new Menu("Flee", "Flee");
                {
                    fleeMenu.AddItem(
                        new MenuItem("ElZilean.Flee.Key", "Flee key").SetValue(
                            new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
                    fleeMenu.AddItem(new MenuItem("ElZilean.Flee.Mana", "Minimum mana").SetValue(new Slider(20)));
                }

                Menu.AddSubMenu(fleeMenu);

                var drawingsMenu = new Menu("Drawings", "Drawings");
                {
                    drawingsMenu.AddItem(new MenuItem("ElZilean.Draw.Off", "Disable drawings").SetValue(false));
                    drawingsMenu.AddItem(new MenuItem("ElZilean.Draw.Q", "Draw Q").SetValue(new Circle()));
                }

                Menu.AddSubMenu(drawingsMenu);

                var miscMenu = new Menu("Misc", "Misc");
                {
                    miscMenu.AddItem(new MenuItem("ElZilean.Combo.AA", "Don't AA before Q").SetValue(true));
                    miscMenu.AddItem(new MenuItem("ElZilean.Q.Stun", "Auto Q on stunned targets").SetValue(false));
                    miscMenu.AddItem(new MenuItem("ElZilean.Q.Interrupt", "Interrupt spells with Q").SetValue(true));
                    miscMenu.AddItem(new MenuItem("ElZilean.E.Slow", "Speed up slowed allies").SetValue(true));
                }

                Menu.AddSubMenu(miscMenu);

                Menu.AddToMainMenu();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        /// <summary>
        ///     
        /// </summary>
        /// <param name="args"></param>
        private static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (IsActive("ElZilean.Combo.AA"))
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    if (Q.IsReady())
                    {
                        args.Process = false;
                    }
                }
            }
        }

        /// <summary>
        ///     The ignite killsteal logic
        /// </summary>
        private static void HandleIgnite()
        {
            if (Player.GetSpellSlot("summonerdot") == SpellSlot.Unknown)
            {
                return;
            }

            var kSableEnemy =
                HeroManager.Enemies.FirstOrDefault(
                    hero =>
                    hero.IsValidTarget(550f) && !hero.HasBuff("summonerdot") && !hero.IsZombie
                    && Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite) >= hero.Health);

            if (kSableEnemy != null)
            {
                Player.Spellbook.CastSpell(IgniteSpell.Slot, kSableEnemy);
            }
        }

        /// <summary>
        ///     Gets the active menu item
        /// </summary>
        /// <value>
        ///     The menu item
        /// </value>
        private static bool IsActive(string menuName)
        {
            return Menu.Item(menuName).IsActive();
        }

        private static void MouseCombo()
        {
            if (IsActive("ElZilean.Combo.Q") && Q.IsReady())
            {
                Q.Cast(Game.CursorPos);
                LeagueSharp.Common.Utility.DelayAction.Add(100, () => W.Cast());
            }
        }

        /// <summary>
        ///     Combo logic
        /// </summary>
        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (IsActive("ElZilean.Combo.Q") && Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                var pred = Q.GetPrediction(target);
                if (pred.Hitchance >= HitChance.VeryHigh)
                {
                    Q.Cast(pred.CastPosition);
                }
            }

            // Check if target has a bomb
            var isBombed = HeroManager.Enemies.Find(x => x.HasBuff("ZileanQEnemyBomb") && x.IsValidTarget(Q.Range));
            if (!isBombed.IsValidTarget())
            {
                return;
            }

            if (isBombed != null && isBombed.IsValidTarget(Q.Range))
            {
                if (Q.Instance.CooldownExpires - Game.Time < 3)
                {
                    return;
                }

                if (IsActive("ElZilean.Combo.W"))
                {
                    W.Cast();
                }
            }

            if (IsActive("ElZilean.Combo.W") && IsActive("ElZilean.Combo.W2") && W.IsReady() && !Q.IsReady())
            {
                if (HeroManager.Enemies.Any(x => x.Health > Q.GetDamage(x) && x.IsValidTarget(Q.Range)))
                {
                    return;
                }

                W.Cast();
            }

            if (IsActive("ElZilean.Combo.E") && E.IsReady())
            {
                if (Player.GetEnemiesInRange(E.Range).Any())
                {
                    var closestEnemy =
                        Player.GetEnemiesInRange(E.Range)
                            .OrderByDescending(h => (h.PhysicalDamageDealtPlayer + h.MagicDamageDealtPlayer))
                            .FirstOrDefault();

                    if (closestEnemy == null || closestEnemy.HasBuffOfType(BuffType.Stun))
                    {
                        return;
                    }

                    E.Cast(closestEnemy);
                }
            }

            if (IsActive("ElZilean.Ignite") && isBombed != null)
            {
                if (Player.GetSpellSlot("summonerdot") == SpellSlot.Unknown)
                {
                    return;
                }

                if (Q.GetDamage(isBombed) + IgniteSpell.GetDamage(isBombed) > isBombed.Health)
                {
                    if (isBombed.IsValidTarget(Q.Range))
                    {
                        Player.Spellbook.CastSpell(IgniteSpell.Slot, isBombed);
                    }
                }
            }
        }

        /// <summary>
        ///     Called when the game draws itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnDraw(EventArgs args)
        {
            if (IsActive("ElZilean.Draw.Off"))
            {
                return;
            }

            if (Menu.Item("ElZilean.Draw.Q").GetValue<Circle>().Active)
            {
                if (Q.Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.DodgerBlue);
                }
            }
        }

        /// <summary>
        ///     E Flee to mouse
        /// </summary>
        private static void OnFlee()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (E.IsReady() && Player.Mana > Menu.Item("ElZilean.Flee.Mana").GetValue<Slider>().Value)
            {
                E.Cast();
            }

            if (!E.IsReady() && W.IsReady())
            {
                if (HasSpeedBuff)
                {
                    return;
                }

                W.Cast();
            }
        }

        /// <summary>
        ///     Harass logic
        /// </summary>
        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (IsActive("ElZilean.Harass.Q") && Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                var pred = Q.GetPrediction(target);
                if (pred.Hitchance >= HitChance.VeryHigh)
                {
                    Q.Cast(pred.UnitPosition);
                }
            }

            if (IsActive("ElZilean.Harass.W") && W.IsReady() && !Q.IsReady())
            {
                W.Cast();
            }

            // Check if target has a bomb
            var isBombed =
                HeroManager.Enemies.FirstOrDefault(x => x.HasBuff("ZileanQEnemyBomb") && x.IsValidTarget(Q.Range));
            if (!isBombed.IsValidTarget())
            {
                return;
            }

            if (IsActive("ElZilean.Harass.W"))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(100, () => W.Cast());
            }
        }

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender == null || !sender.IsValidTarget(Q.Range) || !sender.IsEnemy)
            {
                return;
            }

            if (sender.IsValid && args.DangerLevel == Interrupter2.DangerLevel.High && IsActive("ElZilean.Q.Interrupt"))
            {
                if (Q.IsReady() && sender.IsValidTarget(Q.Range))
                {
                    var prediction = Q.GetPrediction(sender);
                    if (prediction.Hitchance >= HitChance.VeryHigh)
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                }
                LeagueSharp.Common.Utility.DelayAction.Add(100, () => W.Cast());
            }
        }

        /// <summary>
        ///     The laneclear logic
        /// </summary>
        private static void OnLaneclear()
        {
            var minion = MinionManager.GetMinions(Player.Position, Q.Range + Q.Width);
            if (minion == null)
            {
                return;
            }

            if (Player.ManaPercent < Menu.Item("ElZilean.laneclear.Mana").GetValue<Slider>().Value)
            {
                return;
            }

            var farmLocation =
                MinionManager.GetBestCircularFarmLocation(
                    MinionManager.GetMinions(Q.Range).Select(x => x.ServerPosition.To2D()).ToList(),
                    Q.Width,
                    Q.Range);

            if (farmLocation.MinionsHit == 0)
            {
                return;
            }

            if (IsActive("ElZilean.laneclear.Q") && IsActive("ElZilean.laneclear.QMouse") && Q.IsReady())
            {
                Q.Cast(Game.CursorPos);
            }

            if (IsActive("ElZilean.laneclear.Q") && Q.IsReady() && !IsActive("ElZilean.laneclear.QMouse")
                && farmLocation.MinionsHit >= 3)
            {
                Q.Cast(farmLocation.Position.To3D());
            }

            if (IsActive("ElZilean.laneclear.W") && W.IsReady())
            {
                W.Cast();
            }
        }

        /// <summary>
        ///     Fired when the game processes a spell cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var hero = sender;
            if (hero == null || !sender.IsAlly || !(sender is AIHeroClient))
            {
                return;
            }

            if (!Menu.Item($"Initiator{sender.CharData.BaseSkinName}").IsActive())
            {
                return;
            }

            var initiatorChampionSpell =
                Spells.FirstOrDefault(x => x.SDataName.Equals(args.SData.Name, StringComparison.OrdinalIgnoreCase));

            if (initiatorChampionSpell != null)
            {
                if (args.Start.Distance(Player.Position) <= E.Range && args.End.Distance(Player.Position) <= E.Range
                    && HeroManager.Enemies.Any(
                        e =>
                        e.IsValidTarget(E.Range, false) && !e.IsDead
                        && (e.Position.Distance(args.End) < 600f || e.Position.Distance(args.Start) < 800f)))
                {
                    if (E.IsReady() && E.IsInRange(hero))
                    {
                        E.CastOnUnit(hero);
                    }
                }
            }
        }

        /// <summary>
        ///     Called when the game updates
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsDead)
                {
                    return;
                }

                if (IsActive("ElZilean.Combo.Focus.Bomb"))
                {
                    var passiveTarget = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget() && x.HasBuff("ZileanQEnemyBomb") && x.IsValidTarget(Q.Range + 100));

                    if (passiveTarget != null)
                    {
                        Orbwalker.ForceTarget(passiveTarget);
                    }
                }


                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        OnCombo();
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        OnHarass();
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        OnLaneclear();
                        break;
                }

                if (IsActive("ElZilean.Ignite"))
                {
                    HandleIgnite();
                }

                if (Menu.Item("ElZilean.DoubleBombMouse").GetValue<KeyBind>().Active)
                {
                    MouseCombo();
                }

                if (Menu.Item("ElZilean.Flee.Key").GetValue<KeyBind>().Active)
                {
                    OnFlee();
                }

                if (IsActive("ElZilean.E.Slow"))
                {
                    foreach (var slowedAlly in
                        HeroManager.Allies.Where(x => x.HasBuffOfType(BuffType.Slow) && x.IsValidTarget(Q.Range, false))
                        )
                    {
                        if (E.IsReady() && E.IsInRange(slowedAlly))
                        {
                            E.CastOnUnit(slowedAlly);
                        }
                    }
                }

                if (IsActive("ElZilean.Q.Stun"))
                {
                    var target =
                        HeroManager.Enemies.FirstOrDefault(
                            h =>
                            h.IsValidTarget(Q.Range) && h.HasBuffOfType(BuffType.Slow)
                            || h.HasBuffOfType(BuffType.Knockup) || h.HasBuffOfType(BuffType.Charm)
                            || h.HasBuffOfType(BuffType.Stun));

                    if (target != null)
                    {
                        if (Q.IsReady() && target.IsValidTarget(Q.Range))
                        {
                            var prediction = Q.GetPrediction(target);
                            if (prediction.Hitchance >= HitChance.VeryHigh)
                            {
                                Q.Cast(prediction.CastPosition);
                                LeagueSharp.Common.Utility.DelayAction.Add(100, () => W.Cast());
                            }
                        }
                    }
                }

                foreach (var ally in HeroManager.Allies.Where(a => a.IsValidTarget(R.Range, false)))
                {
                    if (!Menu.Item($"R{ally.ChampionName}").IsActive() || ally.IsRecalling() || ally.IsInvulnerable
                        || !ally.IsValidTarget(R.Range, false))
                    {
                        return;
                    }

                    var enemies = ally.CountEnemiesInRange(750f);
                    var totalDamage = IncomingDamageManager.GetDamage(ally) * 1.1f;
                    if (ally.HealthPercent <= Menu.Item("min-health").GetValue<Slider>().Value && !ally.IsDead
                        && enemies >= 1 && ally.IsValidTarget(R.Range, false))
                    {
                        if ((int)(totalDamage / ally.Health) > Menu.Item("min-damage").GetValue<Slider>().Value
                            || ally.HealthPercent < Menu.Item("min-health").GetValue<Slider>().Value)
                        {
                            if (ally.Buffs.Any( b => b.DisplayName == "judicatorintervention" || b.DisplayName == "undyingrage" || b.DisplayName == "kindredrnodeathbuff" || b.DisplayName == "zhonyasringshield" || b.DisplayName == "willrevive"))
                            {
                                return;
                            }

                            R.Cast(ally);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        /// <summary>
        ///     Represents a spell that an item should be casted on.
        /// </summary>
        public class InitiatorSpell
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the name of the champion.
            /// </summary>
            /// <value>
            ///     The name of the champion.
            /// </value>
            public string ChampionName { get; set; }

            /// <summary>
            ///     Gets or sets the name of the s data.
            /// </summary>
            /// <value>
            ///     The name of the s data.
            /// </value>
            public string SDataName { get; set; }

            #endregion
        }
    }
}