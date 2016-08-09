namespace GFUELElise
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using EloBuddy;

    internal class Elise
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the E spell
        /// </summary>
        /// <value>
        ///     The E spell
        /// </value>
        private static Spell E { get; set; }

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The Smitespell
        /// </value>
        private static Spell IgniteSpell { get; set; }

        /// <summary>
        ///     Gets the set player state human/spider
        /// </summary>
        /// <value>
        ///     The player state
        /// </value>
        private static bool IsHuman
        {
            get
            {
                return Player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseHumanQ"
                       || Player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseHumanW"
                       || Player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseHumanE";
            }
        }

        /// <summary>
        ///     Gets the set player state human/spider
        /// </summary>
        /// <value>
        ///     The player state
        /// </value>
        private static bool IsSpider
        {
            get
            {
                return Player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseSpiderQCast"
                       || Player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseSpiderW"
                       || Player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseSpiderEInitial";
            }
        }

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
        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

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
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The Smitespell
        /// </value>
        private static Spell SmiteSpell { get; set; }

        /// <summary>
        ///     Gets or sets the spider E spell
        /// </summary>
        /// <value>
        ///     The spider E spell
        /// </value>
        private static Spell SpiderE { get; set; }

        /// <summary>
        ///     Gets or sets the spider Q spell
        /// </summary>
        /// <value>
        ///     The spider Q spell
        /// </value>
        private static Spell SpiderQ { get; set; }

        /// <summary>
        ///     Gets or sets the spider W spell
        /// </summary>
        /// <value>
        ///     The spider W spell
        /// </value>
        private static Spell SpiderW { get; set; }

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
                if (Player.ChampionName != "Elise")
                {
                    return;
                }

                var smiteSlot = Player.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("smite")
                                    ? SpellSlot.Summoner1
                                    : Player.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("smite")
                                          ? SpellSlot.Summoner2
                                          : SpellSlot.Unknown;

                if (smiteSlot != SpellSlot.Unknown)
                {
                    SmiteSpell = new Spell(smiteSlot);
                }

                var igniteSlot = Player.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("summonerdot")
                                     ? SpellSlot.Summoner1
                                     : Player.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("summonerdot")
                                           ? SpellSlot.Summoner2
                                           : SpellSlot.Unknown;

                if (igniteSlot != SpellSlot.Unknown)
                {
                    IgniteSpell = new Spell(igniteSlot, 600f);
                }

                Q = new Spell(SpellSlot.Q, 625f);
                W = new Spell(SpellSlot.W, 950f);
                E = new Spell(SpellSlot.E, 1075f);
                R = new Spell(SpellSlot.R, 0);

                SpiderQ = new Spell(SpellSlot.Q, 475f);
                SpiderW = new Spell(SpellSlot.W, Orbwalking.GetRealAutoAttackRange(Player));
                SpiderE = new Spell(SpellSlot.E, 750f);

                W.SetSkillshot(0.25f, 100f, 1000, true, SkillshotType.SkillshotLine);
                E.SetSkillshot(0.25f, 55f, 1300, true, SkillshotType.SkillshotLine);

                GenerateMenu();

                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
                Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Methods

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!IsSpider)
            {
                if (IsActive("GFUELElise.Misc.Antigapcloser") && E.IsReady())
                {
                    if (gapcloser.Sender.IsValidTarget(E.Range))
                    {
                        E.Cast(gapcloser.Sender);
                    }
                }
            }
        }

        private static void AutoE()
        {
            if (IsHuman)
            {
                if (E.IsReady() && IsActive("GFUELElise.Auto.E"))
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && !x.IsDead))
                    {
                        var pred = E.GetPrediction(enemy);
                        if (pred.Hitchance >= HitChance.Immobile)
                        {
                            E.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Combo logic
        /// </summary>
        private static void DoCombo()
        {
            try
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                if (target == null)
                {
                    return;
                }

                if (IsHuman)
                {

                    if (IsActive("GFUELElise.Combo.Q") && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.CastOnUnit(target);
                    }

                    if (IsActive("GFUELElise.Combo.E") && target.Distance(Player.Position) <= E.Range && E.IsReady())
                    {
                        var prediction = E.GetPrediction(target);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            E.Cast(prediction.CastPosition);
                        }
                    }

                    if (target.HasBuff("buffelisecocoon") && SpiderQ.IsReady() && target.IsValidTarget(SpiderQ.Range))
                    {
                        R.Cast();
                    }

                    if (IsActive("GFUELElise.Combo.W") && W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        var prediction = W.GetPrediction(target);
                        if (prediction.CollisionObjects.Count == 0)
                        {
                            W.Cast(prediction.CastPosition);
                        }
                    }

                    if (IsActive("GFUELElise.Combo.R"))
                    {
                        if (Player.ManaPercent < Menu.Item("GFUELElise.R.nein").GetValue<Slider>().Value)
                        {
                            R.Cast();
                        }

                        if (Player.Distance(target) <= 750 && R.IsReady()
                            && (!Q.IsReady() && !W.IsReady() && !E.IsReady()
                                || !Q.IsReady() && !W.IsReady() && !E.IsReady()))
                        {
                            R.Cast();
                        }

                        if (SpiderQ.IsReady() && target.IsValidTarget(SpiderQ.Range)
                            && target.IsValidTarget(SpiderQ.Range))
                        {
                            R.Cast();
                        }
                    }
                }

                if (IsSpider)
                {
                    if (IsActive("GFUELElise.ComboSpider.Q") && SpiderQ.IsReady())
                    {
                        if (target.IsValidTarget(SpiderQ.Range))
                        {
                            SpiderQ.Cast(target);
                        }
                    }

                    if (IsActive("GFUELElise.ComboSpider.W") && Player.Distance(target) <= 140 && SpiderW.IsReady())
                    {
                        if (target.IsValidTarget(SpiderW.Range))
                        {
                            SpiderW.Cast();
                        }
                    }

                    if (IsActive("GFUELElise.ComboSpider.E") && Player.Distance(target) <= SpiderE.Range
                        && Player.Distance(target) > SpiderQ.Range && SpiderE.IsReady())
                    {
                        if (target.IsValidTarget(SpiderQ.Range))
                        {
                            return;
                        }
                        SpiderE.Cast(target);
                    }

                    if (IsActive("GFUELElise.Combo.R"))
                    {
                        if (target.IsValidTarget(SpiderQ.Range) || (E.IsReady() && target.IsValidTarget(E.Range)))
                        {
                            return;
                        }

                        if (Player.ManaPercent < Menu.Item("GFUELElise.R.nein").GetValue<Slider>().Value)
                        {
                            return;
                        }

                        if (R.IsReady() && !target.IsValidTarget(SpiderQ.Range) && !SpiderE.IsReady())
                        {
                            R.Cast();
                        }

                        if (!SpiderQ.IsReady() && !SpiderW.IsReady() && R.IsReady())
                        {
                            R.Cast();
                        }

                        if (!SpiderQ.IsReady() && !SpiderE.IsReady() && !SpiderW.IsReady()
                            || !SpiderQ.IsReady() && Q.IsReady() && Q.GetDamage(target) > target.Health)
                        {
                            R.Cast();
                        }
                    }

                    if (IsActive("GFUELElise.ComboSpider.E") && Player.Distance(target) > SpiderQ.Range
                        && SpiderE.IsReady())
                    {
                        SpiderE.Cast(target);
                    }
                }

                if (IsActive("GFUELElise.Combo.R"))
                {
                    if (!Q.IsReady() && !W.IsReady() && !R.IsReady()
                        || (Q.IsReady() && Q.GetDamage(target) >= target.Health)
                        || W.IsReady() && W.GetDamage(target) >= target.Health)
                    {
                        if (SpiderQ.IsReady() && target.IsValidTarget(SpiderQ.Range))
                        {
                            return;
                        }

                        R.Cast();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     Harass logic
        /// </summary>
        private static void DoHarass()
        {
            try
            {
                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                if (target == null)
                {
                    return;
                }

                if (!IsSpider)
                {
                    if (IsActive("GFUELElise.Harass.Q") && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.Cast(target);
                    }

                    if (IsActive("GFUELElise.Harass.W") && W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        var prediction = W.GetPrediction(target);
                        if (prediction.CollisionObjects.Count == 0)
                        {
                            W.Cast(target.ServerPosition);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void DoJungleclear()
        {
            try
            {
                var minion =
                    MinionManager.GetMinions(
                        Player.ServerPosition,
                        Q.Range + W.Width,
                        MinionTypes.All,
                        MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth).FirstOrDefault();

                if (minion == null)
                {
                    return;
                }

                if (!IsSpider)
                {
                    if (IsActive("GFUELElise.jungleclear.Q") && Q.IsReady())
                    {
                        Q.Cast(minion);
                    }

                    if (IsActive("GFUELElise.jungleclear.W") && W.IsReady() && minion.IsValidTarget(W.Range))
                    {
                        W.Cast(minion.Position);
                    }

                    if (IsActive("GFUELElise.jungleclear.SwitchR") && (!Q.IsReady() && !W.IsReady())
                        || Player.ManaPercent < Menu.Item("GFUELElise.jungleclear.Mana").GetValue<Slider>().Value)
                    {
                        R.Cast();
                    }
                }

                if (IsSpider)
                {
                    if (IsActive("GFUELElise.jungleclear.SpiderQ") && SpiderQ.IsReady())
                    {
                        SpiderQ.Cast(minion);
                    }

                    if (IsActive("GFUELElise.jungleclear.SpiderW") && W.IsReady() && minion.IsValidTarget(SpiderW.Range))
                    {
                        SpiderW.Cast();
                    }

                    if (IsActive("GFUELElise.jungleclear.SwitchR") && R.IsReady() && Q.IsReady() && !SpiderQ.IsReady()
                        && !SpiderW.IsReady())
                    {
                        R.Cast();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void DoLaneclear()
        {
            try
            {
                var minion = MinionManager.GetMinions(Player.Position, Q.Range + W.Width).FirstOrDefault();
                if (minion == null)
                {
                    return;
                }

                if (!IsSpider)
                {
                    if (Player.ManaPercent < Menu.Item("GFUELElise.laneclear.Mana").GetValue<Slider>().Value)
                    {
                        if (IsActive("GFUELElise.laneclear.SwitchR") && R.IsReady())
                        {
                            R.Cast();
                        }
                    }

                    if (IsActive("GFUELElise.laneclear.Q") && Q.IsReady())
                    {
                        Q.Cast(minion);
                    }

                    if (IsActive("GFUELElise.laneclear.W") && W.IsReady() && minion.IsValidTarget(W.Range))
                    {
                        W.Cast(minion.Position);
                    }
                    if (IsActive("GFUELElise.laneclear.SwitchR") && (!Q.IsReady() && !W.IsReady())
                        || Player.ManaPercent < Menu.Item("GFUELElise.laneclear.Mana").GetValue<Slider>().Value)
                    {
                        R.Cast();
                    }
                }

                if (IsSpider)
                {
                    if (IsActive("GFUELElise.laneclear.SpiderQ") && SpiderQ.IsReady())
                    {
                        SpiderQ.Cast(minion);
                    }

                    if (IsActive("GFUELElise.laneclear.SpiderW") && W.IsReady() && minion.IsValidTarget(SpiderW.Range))
                    {
                        SpiderW.Cast();
                    }

                    if (IsActive("GFUELElise.laneclear.SwitchR") && R.IsReady() && Q.IsReady() && !SpiderQ.IsReady()
                        && !SpiderW.IsReady())
                    {
                        R.Cast();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

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
                Menu = new Menu("GFUEL ELISE", "GFUELELISE", true);

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
                    comboMenu.AddItem(new MenuItem("GFUELElise.Combo.Q", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("GFUELElise.Combo.W", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("GFUELElise.Combo.E", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("GFUELElise.ComboSpider.Q", "Use Spider Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("GFUELElise.ComboSpider.W", "Use Spider W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("GFUELElise.ComboSpider.E", "Use Spider E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("GFUELElise.Combo.R", "Switch forms automatic").SetValue(true));
                    comboMenu.AddItem(
                        new MenuItem("GFUELElise.Combo.Semi.E", "Cast E").SetValue(
                            new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                    comboMenu.AddItem(
                        new MenuItem("GFUELElise.R.nein", "Dont switch to human if mana under").SetValue(
                            new Slider(10, 0, 100)));
                }

                Menu.AddSubMenu(comboMenu);

                var harassMenu = new Menu("Harass", "Harass");
                {
                    harassMenu.AddItem(new MenuItem("GFUELElise.Harass.Q", "Use Q").SetValue(true));
                    harassMenu.AddItem(new MenuItem("GFUELElise.Harass.W", "Use W").SetValue(true));
                }

                Menu.AddSubMenu(harassMenu);

                var laneclearMenu = new Menu("Laneclear", "Laneclear");
                {
                    laneclearMenu.AddItem(new MenuItem("GFUELElise.laneclear.Q", "Use Q").SetValue(true));
                    laneclearMenu.AddItem(new MenuItem("GFUELElise.laneclear.W", "Use W").SetValue(true));

                    laneclearMenu.AddItem(new MenuItem("GFUELElise.laneclear.SpiderQ", "Use Spider Q").SetValue(true));
                    laneclearMenu.AddItem(new MenuItem("GFUELElise.laneclear.SpiderW", "Use Spider W").SetValue(true));
                    laneclearMenu.AddItem(new MenuItem("GFUELElise.laneclear.SwitchR", "Switch R").SetValue(true));

                    laneclearMenu.AddItem(
                        new MenuItem("GFUELElise.laneclear.Mana", "Minimum mana").SetValue(new Slider(20, 0, 100)));
                }

                Menu.AddSubMenu(laneclearMenu);

                var jungleclearMenu = new Menu("Jungleclear", "Jungleclear");
                {
                    jungleclearMenu.AddItem(new MenuItem("GFUELElise.jungleclear.Q", "Use Q").SetValue(true));
                    jungleclearMenu.AddItem(new MenuItem("GFUELElise.jungleclear.W", "Use W").SetValue(true));

                    jungleclearMenu.AddItem(
                        new MenuItem("GFUELElise.jungleclear.SpiderQ", "Use Spider Q").SetValue(true));
                    jungleclearMenu.AddItem(
                        new MenuItem("GFUELElise.jungleclear.SpiderW", "Use Spider W").SetValue(true));
                    jungleclearMenu.AddItem(new MenuItem("GFUELElise.jungleclear.SwitchR", "Switch R").SetValue(true));

                    jungleclearMenu.AddItem(
                        new MenuItem("GFUELElise.jungleclear.Mana", "Minimum mana").SetValue(new Slider(20, 0, 100)));
                }

                Menu.AddSubMenu(jungleclearMenu);

                var smiteMenu = new Menu("Smite", "Smite");
                {
                    smiteMenu.AddItem(
                        new MenuItem("GFUELElise.Smite.Nope", "Do not use smite in human").SetValue(false));
                    smiteMenu.AddItem(new MenuItem("GFUELElise.Smite", "Use smite in spider combo").SetValue(true));
                }

                Menu.AddSubMenu(smiteMenu);

                var miscellaneousMenu = new Menu("Miscellaneous", "Miscellaneous");
                {
                    miscellaneousMenu.AddItem(new MenuItem("GFUELElise.Auto.E", "Auto E immobile").SetValue(true));
                    miscellaneousMenu.AddItem(new MenuItem("GFUELElise.Draw.Off", "Disable drawings").SetValue(false));
                    miscellaneousMenu.AddItem(new MenuItem("GFUELElise.Draw.Q", "Draw Q").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("GFUELElise.Draw.W", "Draw W").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("GFUELElise.Draw.E", "Draw E").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(
                        new MenuItem("GFUELElise.Misc.Antigapcloser", "Use E - Antigapcloser").SetValue(true));
                    miscellaneousMenu.AddItem(
                        new MenuItem("GFUELElise.Misc.Interrupter", "Use E - interrupter").SetValue(true));

                    var dmgAfterE = new MenuItem("ElDiana.DrawComboDamage", "Draw combo damage").SetValue(true);
                    var drawFill =
                        new MenuItem("ElDiana.DrawColour", "Fill colour", true).SetValue(
                            new Circle(true, Color.Goldenrod));
                    miscellaneousMenu.AddItem(drawFill);
                    miscellaneousMenu.AddItem(dmgAfterE);

                    DamageIndicator.DamageToUnit = GetComboDamage;
                    DamageIndicator.Enabled = dmgAfterE.GetValue<bool>();
                    DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                    DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

                    dmgAfterE.ValueChanged +=
                        delegate(object sender, OnValueChangeEventArgs eventArgs)
                            {
                                DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                            };

                    drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
                        {
                            DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                            DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                        };
                }

                Menu.AddSubMenu(miscellaneousMenu);

                Menu.AddToMainMenu();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (!Player.Spellbook.IsAutoAttacking)
            {
                damage += (float)ObjectManager.Player.GetAutoAttackDamage(enemy, true);
            }

            if (Q.IsReady())
            {
                damage += Q.GetDamage(enemy);
            }

            if (W.IsReady())
            {
                damage += W.GetDamage(enemy);
            }

            if (SpiderQ.IsReady())
            {
                damage += SpiderQ.GetDamage(enemy);
            }

            if (SpiderW.IsReady())
            {
                damage += SpiderW.GetDamage(enemy);
            }

            return damage;
        }

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!IsSpider)
            {
                if (IsActive("GFUELElise.Misc.Interrupter") && E.IsReady())
                {
                    if (sender.IsValidTarget(E.Range))
                    {
                        E.Cast(sender);
                    }
                }
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

        /// <summary>
        ///     Called when the game draws itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnDraw(EventArgs args)
        {
            try
            {
                if (Player.IsDead)
                {
                    return;
                }

                if (IsActive("GFUELElise.Draw.Off"))
                {
                    return;
                }

                if (Menu.Item("GFUELElise.Draw.Q").GetValue<Circle>().Active)
                {
                    if (Q.Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.OrangeRed);
                    }
                }

                if (Menu.Item("GFUELElise.Draw.W").GetValue<Circle>().Active)
                {
                    if (W.Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.DeepSkyBlue);
                    }
                }

                if (Menu.Item("GFUELElise.Draw.E").GetValue<Circle>().Active)
                {
                    if (E.Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.DeepSkyBlue);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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

                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        DoCombo();
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        DoHarass();
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        DoJungleclear();
                        DoLaneclear();
                        break;
                }

                if (Menu.Item("GFUELElise.Combo.Semi.E").GetValue<KeyBind>().Active)
                {
                    SemiE();
                }

                if (IsActive("GFUELElise.Auto.E"))
                {
                    AutoE();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void Orbwalk(Vector3 pos, AIHeroClient target = null)
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, pos);
        }

        private static void SemiE()
        {
            Orbwalk(Game.CursorPos);

            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            Orbwalking.Orbwalk(target ?? null, Game.CursorPos);

            if (!E.IsReady() || !target.IsValidTarget(E.Range))
            {
                return;
            }

            var prediction = E.GetPrediction(target);
            E.Cast(prediction.CastPosition);
        }

        #endregion
    }
}