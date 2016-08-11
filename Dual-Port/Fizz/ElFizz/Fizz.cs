using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElFizz
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal class Fizz
    {
        #region Static Fields

        private static int lastSwitch;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The Smitespell
        /// </value>
        public static Spell IgniteSpell { get; set; }

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
        ///     Gets or sets the jump back position
        /// </summary>
        /// <value>
        ///     The the jump back position
        /// </value>
        private static bool JumpBack { get; set; }

        /// <summary>
        ///     Gets or sets the last harass position
        /// </summary>
        /// <value>
        ///     The last harass position
        /// </value>
        private static Vector3? LastHarassPos { get; set; }

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
                if (Player.ChampionName != "Fizz")
                {
                    return;
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


                Q = new Spell(SpellSlot.Q, 550f);
                W = new Spell(SpellSlot.W, Orbwalking.GetRealAutoAttackRange(Player));
                E = new Spell(SpellSlot.E, 400f);
                R = new Spell(SpellSlot.R, 1300f);

                E.SetSkillshot(0.25f, 330, float.MaxValue, false, SkillshotType.SkillshotCircle);
                R.SetSkillshot(0.25f, 120f, 1350, false, SkillshotType.SkillshotLine);

                GenerateMenu();

                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                Obj_AI_Base.OnSpellCast += OnProcessSpellCast;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Calculates the combo damage
        /// </summary>
        private static float CalculateComboDamage(AIHeroClient target, bool q, bool w, bool e, bool r)
        {
            try
            {
                if (target == null)
                {
                    return 0;
                }

                var damage = 0f;

                if (q && Q.IsReady())
                {
                    damage += Q.GetDamage(target);
                }

                if (w && W.IsReady())
                {
                    damage += W.GetDamage(target);
                }

                if (e && E.IsReady())
                {
                    damage += E.GetDamage(target);
                }

                if (r && R.IsReady())
                {
                    damage += R.GetDamage(target);
                }

                return damage;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return 0;
        }

        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            try
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

                if (E.IsReady())
                {
                    damage += E.GetDamage(enemy);
                }

                if (Player.HasBuff("lichbane"))
                {
                    damage +=
                        (float)
                        Player.CalcDamage(
                            enemy,
                            Damage.DamageType.Magical,
                            (Player.BaseAttackDamage * 0.75)
                            + (Player.BaseAbilityDamage + Player.FlatMagicDamageMod) * 0.5);
                }

                if (R.IsReady())
                {
                    damage += R.GetDamage(enemy);
                }

                return damage;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return 0;
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
                Menu = new Menu("ElFizz", "ElFizz", true);

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
                    comboMenu.SubMenu("Other Settings")
                        .AddItem(
                            new MenuItem("ElFizz.Combo.Gapclose.E", "Gapclose with E if killable with Q").SetValue(true));
                    comboMenu.SubMenu("Other Settings")
                        .AddItem(
                            new MenuItem("ElFizz.Combo.Gapclose.E.Mana", "Minimum mana").SetValue(
                                new Slider(20, 0, 100)));

                    comboMenu.SubMenu("Other Settings")
                        .AddItem(new MenuItem("ElFizz.Combo.R.SingleKill", "Use R Single kill").SetValue(true));

                    comboMenu.AddItem(new MenuItem("ElFizz.Combo.Q", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElFizz.Combo.W", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElFizz.Combo.E", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElFizz.Combo.R", "Use R").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElFizz.Combo.Overkill.R", "Check R overkill").SetValue(false));

                    comboMenu.AddItem(
                        new MenuItem("ElFizz.Combo.Type", "Combo Type").SetValue(
                            new StringList(new[] { "Q -> R -> E", "R -> Q -> E" }, 0)));

                    comboMenu.AddItem(
                        new MenuItem("ElFizz.Combo.Switch", "Switch Combo Type").SetValue(
                            new KeyBind("L".ToCharArray()[0], KeyBindType.Press)));
                }

                Menu.AddSubMenu(comboMenu);

                var harassMenu = new Menu("Harass", "Harass");
                {
                    harassMenu.AddItem(new MenuItem("ElFizz.Harass.Q", "Use Q").SetValue(true));
                    harassMenu.AddItem(new MenuItem("ElFizz.Harass.W", "Use W").SetValue(true));
                    harassMenu.AddItem(new MenuItem("ElFizz.Harass.E", "Use E").SetValue(true));
                    harassMenu.AddItem(
                        new MenuItem("ElFizz.Harass.Mode.E", "E Mode: ").SetValue(
                            new StringList(new[] { "Back to Position", "Towards Enemy" })));
                }
                Menu.AddSubMenu(harassMenu);

                var laneclearMenu = new Menu("Laneclear", "Laneclear");
                {
                    laneclearMenu.AddItem(new MenuItem("ElFizz.laneclear.Q", "Use Q").SetValue(true));
                    laneclearMenu.AddItem(new MenuItem("ElFizz.laneclear.E", "Use E").SetValue(true));
                    laneclearMenu.AddItem(
                        new MenuItem("ElFizz.laneclear.minionshit", "Minimum minions killable (E)").SetValue(
                            new Slider(2, 1, 5)));
                    laneclearMenu.AddItem(
                        new MenuItem("ElFizz.laneclear.Mana", "Minimum mana").SetValue(new Slider(20, 0, 100)));
                }

                Menu.AddSubMenu(laneclearMenu);

                var jungleclearMenu = new Menu("Jungleclear", "Jungleclear");
                {
                    jungleclearMenu.AddItem(new MenuItem("ElFizz.jungleclear.Q", "Use Q").SetValue(true));
                    jungleclearMenu.AddItem(new MenuItem("ElFizz.jungleclear.E", "Use E").SetValue(true));
                    jungleclearMenu.AddItem(
                        new MenuItem("ElFizz.laneclear.minionshit", "Minimum minions killable (E)").SetValue(
                            new Slider(1, 1, 5)));
                    jungleclearMenu.AddItem(
                        new MenuItem("ElFizz.jungleclear.Mana", "Minimum mana").SetValue(new Slider(20, 0, 100)));
                }

                Menu.AddSubMenu(jungleclearMenu);

                var killstealMenu = new Menu("Killsteal", "Killsteal");
                {
                    killstealMenu.AddItem(new MenuItem("ElFizz.killsteal.Active", "Activate killsteal").SetValue(true));
                    killstealMenu.AddItem(new MenuItem("ElFizz.killsteal.Tower", "Don't Q under tower").SetValue(true));
                    killstealMenu.AddItem(new MenuItem("ElFizz.Killsteal.Q", "Use Q").SetValue(true));
                    killstealMenu.AddItem(new MenuItem("ElFizz.Killsteal.R", "Use R").SetValue(true));
                    killstealMenu.AddItem(new MenuItem("ElFizz.Ignite", "Use Ignite").SetValue(true));
                }

                Menu.AddSubMenu(killstealMenu);

                var fleeMenu = new Menu("Flee", "Flee");
                {
                    fleeMenu.AddItem(
                        new MenuItem("ElFizz.Flee.Key", "Flee key").SetValue(
                            new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
                    fleeMenu.AddItem(new MenuItem("ElFizz.Flee.Mana", "Minimum mana").SetValue(new Slider(20, 0, 100)));
                }

                Menu.AddSubMenu(fleeMenu);

                var drawingsMenu = new Menu("Drawings", "Drawings");
                {
                    drawingsMenu.AddItem(new MenuItem("ElFizz.Draw.Off", "Disable drawings").SetValue(false));
                    drawingsMenu.AddItem(new MenuItem("ElFizz.Draw.Q", "Draw Q").SetValue(new Circle()));
                    drawingsMenu.AddItem(new MenuItem("ElFizz.Draw.R", "Draw R").SetValue(new Circle()));
                    drawingsMenu.AddItem(new MenuItem("ElFizz.drawings.ComboMode", "Draw Combo Mode").SetValue(true));
                    var dmgAfterE = new MenuItem("GFUELQuinn.DrawComboDamage", "Draw combo damage").SetValue(true);
                    var drawFill =
                        new MenuItem("ElDiana.DrawColour", "Fill colour", true).SetValue(
                            new Circle(true, Color.Goldenrod));
                    drawingsMenu.AddItem(drawFill);
                    drawingsMenu.AddItem(dmgAfterE);

                    DamageIndicator.DamageToUnit = GetComboDamage;
                    DamageIndicator.Enabled = dmgAfterE.IsActive();
                    DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                    DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

                    dmgAfterE.ValueChanged +=
                        delegate (object sender, OnValueChangeEventArgs eventArgs)
                        {
                            DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                        };

                    drawFill.ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                        DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                    };
                }

                Menu.AddSubMenu(drawingsMenu);

                Menu.AddToMainMenu();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     The E casting position
        /// </summary>
        private static Vector3 GetEPosition(Obj_AI_Base target)
        {
            try
            {
                return Player.Distance(Prediction.GetPrediction(target, 0.2f).UnitPosition) > E.Range
                           ? Player.Position.Extend(Prediction.GetPrediction(target, 0.2f).UnitPosition, E.Range)
                           : target.Position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return new Vector3();
        }

        /// <summary>
        ///     The ignite killsteal logic
        /// </summary>
        private static void HandleIgnite()
        {
            try
            {
                var kSableEnemy =
                    HeroManager.Enemies.FirstOrDefault(
                        hero =>
                        hero.IsValidTarget(550) && ShieldCheck(hero) && !hero.HasBuff("summonerdot") && !hero.IsZombie
                        && Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite) >= hero.Health);

                if (kSableEnemy != null && IgniteSpell.Slot != SpellSlot.Unknown)
                {
                    Player.Spellbook.CastSpell(IgniteSpell.Slot, kSableEnemy);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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
        ///     Combo logic
        /// </summary>
        private static void OnCombo()
        {
            try
            {
                var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                if (target == null)
                {
                    return;
                }

                if (IsActive("ElFizz.Combo.R.SingleKill") && R.IsReady())
                {
                    if (Qkillable(target))
                    {
                        Q.Cast(target);
                        return;
                    }

                    RLogicKill();
                }

                if (IsActive("ElFizz.Combo.Gapclose.E")
                    && Player.ManaPercent < Menu.Item("ElFizz.Combo.Gapclose.E.Mana").GetValue<Slider>().Value)
                {
                    if (target.Distance(Player) > Q.Range && Q.GetDamage(target) > target.Health)
                    {
                        E.Cast(target.Position);
                    }
                }

                switch (Menu.Item("ElFizz.Combo.Type").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        if (IsActive("ElFizz.Combo.Q") && Q.IsReady() && target.IsValidTarget(Q.Range))
                        {
                            if (Q.Cast(target).IsCasted())
                            {
                                if (E.IsReady())
                                {
                                    E.Cast(target);
                                }
                            }

                            if (R.IsReady())
                            {
                                RCastLogic(target);
                            }
                        }
                        break;

                    case 1:
                        if (R.IsReady())
                        {
                            /*if (Qkillable(target))
                            {
                                Q.Cast(target);
                                return;
                            }*/

                            RCastLogic(target);
                        }

                        if (Q.Cast(target).IsCasted())
                        {
                            if (E.IsReady())
                            {
                                E.Cast(target);
                            }
                        }
                        break;
                }


                if (IsActive("ElFizz.Combo.Q") && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }

                if (IsActive("ElFizz.Combo.Q") && IsActive("ElFizz.Ignite") && IgniteSpell.Slot != SpellSlot.Unknown)
                {
                    if (Q.GetDamage(target) + IgniteSpell.GetDamage(target) > target.Health)
                    {
                        if (target.IsValidTarget(Q.Range))
                        {
                            if (Q.Cast(target).IsCasted())
                            {
                                Player.Spellbook.CastSpell(IgniteSpell.Slot, target);
                            }
                        }
                    }
                }

                if (IsActive("ElFizz.Combo.W") && W.IsReady() && !Q.IsReady() && !E.IsReady()
                    && target.IsValidTarget(W.Range))
                {
                    W.Cast(target);
                }

                if (IsActive("ElFizz.Combo.E") && E.IsReady() && target.IsValidTarget(E.Range)
                    && E.Instance.Name.ToLower() == "fizzjump")
                {
                    if (Prediction.GetPrediction(target, 0.5f).UnitPosition.Distance(Player.Position) <= E.Range + 450)
                    {
                        E.Cast(GetEPosition(target));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     Called when the game draws itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnDraw(EventArgs args)
        {
            try
            {
                if (IsActive("ElFizz.Draw.Off"))
                {
                    return;
                }

                if (Menu.Item("ElFizz.Draw.Q").GetValue<Circle>().Active)
                {
                    if (Q.Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.White);
                    }
                }

                if (Menu.Item("ElFizz.Draw.R").GetValue<Circle>().Active)
                {
                    if (R.Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.DeepSkyBlue);
                    }
                }

                if (IsActive("ElFizz.drawings.ComboMode"))
                {
                    switch (Menu.Item("ElFizz.Combo.Type").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            Drawing.DrawText(
                                Drawing.Width * 0.70f,
                                Drawing.Height * 0.95f,
                                Color.White,
                                "Combo: Q -> R -> E");
                            break;

                        case 1:
                            Drawing.DrawText(
                                Drawing.Width * 0.70f,
                                Drawing.Height * 0.95f,
                                Color.Yellow,
                                "Combo: R -> Q -> E");
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     E Flee to mouse
        /// </summary>
        private static void OnFlee()
        {
            try
            {
                if (E.IsReady() && Player.Mana > Menu.Item("ElFizz.Flee.Mana").GetValue<Slider>().Value)
                {
                    var fleePosition = Player.Position.Extend(Game.CursorPos, E.Range);
                    if (fleePosition.IsWall())
                    {
                        var longestDash = Player.Position;
                        if (longestDash.Distance(Game.CursorPos) <= 500f)
                        {
                            E.Cast(fleePosition);
                        }
                    }
                    else
                    {
                        E.Cast(fleePosition);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     Harass logic by Chewymoon (pls no kill)
        /// </summary>
        private static void OnHarass()
        {
            try
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                if (target == null)
                {
                    return;
                }

                if (LastHarassPos == null)
                {
                    LastHarassPos = Player.ServerPosition;
                }

                if (JumpBack && IsActive("ElFizz.Harass.E"))
                {
                    E.Cast((Vector3)LastHarassPos);
                }

                if (W.IsReady() && IsActive("ElFizz.Harass.W") && (Q.IsReady() || Orbwalker.InAutoAttackRange(target)))
                {
                    W.Cast();
                }

                if (Q.IsReady() && IsActive("ElFizz.Harass.Q"))
                {
                    Q.Cast(target);
                }

                if (E.IsReady() && IsActive("ElFizz.Harass.E")
                    && Menu.Item("ElFizz.Harass.Mode.E").GetValue<StringList>().SelectedIndex == 1)
                {
                    E.Cast(target);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     The laneclear "logic"
        /// </summary>
        private static void OnJungleclear()
        {
            try
            {
                var minion =
                    MinionManager.GetMinions(
                        Player.ServerPosition,
                        Q.Range + E.Width,
                        MinionTypes.All,
                        MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth).FirstOrDefault();

                if (minion == null)
                {
                    return;
                }

                if (Player.ManaPercent < Menu.Item("ElFizz.jungleclear.Mana").GetValue<Slider>().Value)
                {
                    return;
                }

                if (IsActive("ElFizz.jungleclear.Q") && Q.IsReady())
                {
                    Q.Cast(minion);
                }

                if (IsActive("ElFizz.jungleclear.E") && E.IsReady())
                {
                    E.Cast(minion.Position);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     Killsteal logic
        /// </summary>
        private static void OnKillsteal()
        {
            try
            {
                foreach (
                    var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (enemy.IsValidTarget(Q.Range) && enemy.Health < Q.GetDamage(enemy))
                    {
                        if (IsActive("ElFizz.killsteal.Tower") && TowerCheck(enemy))
                        {
                            return;
                        }

                        Q.Cast(enemy);
                    }

                    if (Qkillable(enemy))
                    {
                        Q.Cast(enemy);
                        return;
                    }
                    if (IsActive("ElFizz.Killsteal.R"))
                    {
                        RLogicKill();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     The laneclear "logic"
        /// </summary>
        private static void OnLaneclear()
        {
            try
            {
                var minion = MinionManager.GetMinions(Player.Position, Q.Range + E.Width);
                if (minion == null)
                {
                    return;
                }

                if (Player.ManaPercent < Menu.Item("ElFizz.laneclear.Mana").GetValue<Slider>().Value)
                {
                    return;
                }

                if (IsActive("ElFizz.laneclear.Q") && Q.IsReady())
                {
                    if (minion.Where(m => m.Distance(Player.Position) <= Q.Range).Count(x => Q.GetDamage(x) > x.Health)
                        >= 0)
                    {
                        Q.Cast(minion.FirstOrDefault());
                    }
                }

                if (IsActive("ElFizz.laneclear.E") && E.IsReady()
                    && minion.Where(m => m.Distance(Player.Position) <= E.Width).Count(x => E.GetDamage(x) > x.Health)
                    >= Menu.Item("ElFizz.laneclear.minionshit").GetValue<Slider>().Value)
                {
                    E.Cast(minion[0].Position);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     Fired when the game processes a spell cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name.ToLower() == "fizzjump" || args.SData.Name.ToLower() == "fizzpiercingstrike")
                {
                    if (IsActive("ElFizz.Combo.W") && W.IsReady()
                        && (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                            || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed))
                    {
                        W.Cast();
                    }

                    LastHarassPos = null;
                    JumpBack = false;
                }

                if (args.SData.Name.ToLower() == "fizzpiercingstrike")
                {
                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed
                        && Menu.Item("ElFizz.Harass.Mode.E").GetValue<StringList>().SelectedIndex == 0)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(
                            (int)(sender.Spellbook.CastEndTime - Game.Time) + Game.Ping / 2 + 250,
                            () => { JumpBack = true; });
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
                        OnJungleclear();
                        break;
                }

                SwitchCombo();

                if (IsActive("ElFizz.killsteal.Active"))
                {
                    OnKillsteal();
                }

                if (IsActive("ElFizz.Ignite"))
                {
                    HandleIgnite();
                }

                if (Menu.Item("ElFizz.Flee.Key").GetValue<KeyBind>().Active)
                {
                    OnFlee();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     Check the killability
        /// </summary>
        private static bool Qkillable(AIHeroClient target)
        {
            try
            {
                return target.Health < Q.GetDamage(target) && target.IsValidTarget(Q.Range) && Q.IsReady()
                       && Player.Mana > Q.ManaCost;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            return false;
        }

        /// <summary>
        ///     R casting logic
        /// </summary>
        private static void RCastLogic(AIHeroClient target)
        {
            try
            {
                var prediction = R.GetPrediction(target, false, -1, new[] { CollisionableObjects.YasuoWall });
                if (prediction.Hitchance < HitChance.VeryHigh)
                {
                    return;
                }

                if (prediction.Hitchance >= HitChance.VeryHigh)
                {
                    if (!ShieldCheck(target) || IsActive("ElFizz.Combo.Overkill.R") && Player.GetSpellDamage(target, SpellSlot.R) > target.Health + target.AttackShield)
                    {
                        return;
                    }

                    R.Cast(prediction.CastPosition);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     R logic
        /// </summary>
        private static void RLogicKill()
        {
            try
            {
                foreach (
                    var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && !x.IsDead && !x.IsZombie && ShieldCheck(x)))
                {
                    if (enemy.Health < CalculateComboDamage(enemy, false, false, false, true))
                    {
                        RCastLogic(enemy);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     The shield checker
        /// </summary>
        private static bool ShieldCheck(Obj_AI_Base hero)
        {
            try
            {
                return !hero.HasBuff("summonerbarrier") || !hero.HasBuff("BlackShield")
                       || !hero.HasBuff("SivirShield") || !hero.HasBuff("BansheesVeil")
                       || !hero.HasBuff("ShroudofDarkness");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return false;
        }

        /// <summary>
        ///     Switch the Combo logic
        /// </summary>
        private static void SwitchCombo()
        {
            try
            {
                var switchTime = Utils.GameTimeTickCount - lastSwitch;
                if (Menu.Item("ElFizz.Combo.Switch").GetValue<KeyBind>().Active && switchTime >= 350)
                {
                    switch (Menu.Item("ElFizz.Combo.Type").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            Menu.Item("ElFizz.Combo.Type")
                                .SetValue(new StringList(new[] { "Q -> R -> E", "R -> Q -> E" }, 1));
                            lastSwitch = Utils.GameTimeTickCount;
                            break;
                        case 1:
                            Menu.Item("ElFizz.Combo.Type")
                                .SetValue(new StringList(new[] { "Q -> R -> E", "R -> Q -> E" }, 0));
                            lastSwitch = Utils.GameTimeTickCount;
                            break;

                        default:
                            Menu.Item("ElFizz.Combo.Type")
                                .SetValue(new StringList(new[] { "Q -> R -> E", "R -> Q -> E" }, 0));
                            lastSwitch = Utils.GameTimeTickCount;
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     Check if target is under tower
        /// </summary>
        private static bool TowerCheck(Obj_AI_Base target)
        {
            try
            {
                var tower =
                    ObjectManager.Get<Obj_AI_Turret>()
                        .FirstOrDefault(
                            x => x != null && x.Distance(target) <= 800 && x.Health > 0 && x.IsEnemy && x.IsValid);

                return tower != null;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return false;
        }

        #endregion
    }
}
 