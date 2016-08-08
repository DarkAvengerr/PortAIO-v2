using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GFUELQuinn
{
    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class Quinn
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
                if (Player.ChampionName != "Quinn")
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

                Q = new Spell(SpellSlot.Q, 1025f);
                W = new Spell(SpellSlot.W, 2100f);
                E = new Spell(SpellSlot.E, 675f);
                R = new Spell(SpellSlot.R, 0);

                Q.SetSkillshot(313f, 60f, 1550, true, SkillshotType.SkillshotLine);

                GenerateMenu();

                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
                Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
                Orbwalking.BeforeAttack += OrbwalkingBeforeAttack;
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
            try
            {
                if (IsActive("GFUELQuinn.Misc.Antigapcloser") && E.LSIsReady())
                {
                    if (gapcloser.Sender.LSIsValidTarget(E.Range))
                    {
                        E.CastOnUnit(gapcloser.Sender);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     Combo logic test
        /// </summary>
        private static void DoCombo()
        {
            try
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                if (target == null)
                {
                    return;
                }

                var passiveTarget = HeroManager.Enemies.Find(x => x.HasBuff("quinnw") && x.LSIsValidTarget(Q.Range));
                if (passiveTarget != null)
                {
                    Orbwalker.ForceTarget(passiveTarget);
                }
                else
                {
                    Orbwalker.ForceTarget(null);
                }

                if (IsActive("GFUELQuinn.Combo.Ghostblade"))
                {
                    var ghostBlade = ItemData.Youmuus_Ghostblade.GetItem();
                    if (ghostBlade.IsReady() && ghostBlade.IsOwned(Player)
                        && target.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(Player) + 100))
                    {
                        ghostBlade.Cast();
                    }
                }

                if (IsActive("GFUELQuinn.Combo.E") && target.LSDistance(Player.Position) < E.Range && E.LSIsReady())
                {
                    E.CastOnUnit(target);
                }

                if (IsActive("GFUELQuinn.Combo.Q") && target.LSDistance(Player.Position) < Q.Range && Q.LSIsReady())
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        Q.Cast(prediction.CastPosition);
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
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                if (target == null)
                {
                    return;
                }

                var passiveTarget = HeroManager.Enemies.Find(x => x.HasBuff("quinnw") && x.LSIsValidTarget(Q.Range));
                if (passiveTarget != null)
                {
                    Orbwalker.ForceTarget(passiveTarget);
                }
                else
                {
                    Orbwalker.ForceTarget(null);
                }

                if (IsActive("GFUELQuinn.Harass.Q") && target.LSDistance(Player.Position) < Q.Range && Q.LSIsReady())
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        Q.Cast(prediction.CastPosition);
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
                        Q.Range,
                        MinionTypes.All,
                        MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth).OrderByDescending(x => x.MaxHealth).FirstOrDefault();

                if (minion == null)
                {
                    return;
                }

                if (Player.ManaPercent < Menu.Item("GFUELQuinn.jungleclear.Mana").GetValue<Slider>().Value)
                {
                    return;
                }

                var passiveTarget =
                    MinionManager.GetMinions(Player.Position, Q.Range + Q.Width)
                        .Find(x => x.HasBuff("quinnw") && x.LSIsValidTarget(Q.Range));
                if (passiveTarget != null)
                {
                    Orbwalker.ForceTarget(passiveTarget);
                }
                else
                {
                    Orbwalker.ForceTarget(null);
                }

                if (IsActive("GFUELQuinn.jungleclear.Q"))
                {
                    Q.Cast(minion);
                }

                if (IsActive("GFUELQuinn.jungleclear.Q"))
                {
                    E.CastOnUnit(minion);
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
                var minion = MinionManager.GetMinions(Player.Position, Q.Range + Q.Width).FirstOrDefault();
                if (minion == null)
                {
                    return;
                }

                if (Player.ManaPercent < Menu.Item("GFUELQuinn.laneclear.Mana").GetValue<Slider>().Value)
                {
                    return;
                }

                var passiveTarget =
                    MinionManager.GetMinions(Player.Position, Q.Range + Q.Width)
                        .Find(x => x.HasBuff("quinnw") && x.LSIsValidTarget(Q.Range));
                if (passiveTarget != null)
                {
                    Orbwalker.ForceTarget(passiveTarget);
                }
                else
                {
                    Orbwalker.ForceTarget(null);
                }

                if (IsActive("GFUELQuinn.laneclear.Q"))
                {
                    if (GetCenterMinion().LSIsValidTarget())
                    {
                        Q.Cast(GetCenterMinion());
                    }
                }

                if (IsActive("GFUELQuinn.laneclear.E"))
                {
                    if (E.GetDamage(minion) > minion.Health)
                    {
                        E.CastOnUnit(minion);
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
                Menu = new Menu("GFUEL QUINN", "GFUELQUINN", true);

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
                    comboMenu.AddItem(new MenuItem("GFUELQuinn.Combo.Q", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("GFUELQuinn.Combo.E", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("GFUELQuinn.Combo.Ghostblade", "Use Ghostblade").SetValue(true));
                }

                Menu.AddSubMenu(comboMenu);

                var harassMenu = new Menu("Harass", "Harass");
                {
                    harassMenu.AddItem(new MenuItem("GFUELQuinn.Harass.Q", "Use Q").SetValue(true));
                }

                Menu.AddSubMenu(harassMenu);

                var laneclearMenu = new Menu("Laneclear", "Laneclear");
                {
                    laneclearMenu.AddItem(new MenuItem("GFUELQuinn.laneclear.Q", "Use Q").SetValue(true));
                    laneclearMenu.AddItem(new MenuItem("GFUELQuinn.laneclear.E", "Use E").SetValue(false));
                    laneclearMenu.AddItem(new MenuItem("GFUELQuinn.laneclear.count", "Minimum minion count"))
                        .SetValue(new Slider(3, 2, 6));

                    laneclearMenu.AddItem(
                        new MenuItem("GFUELQuinn.laneclear.Mana", "Minimum mana").SetValue(new Slider(20, 0, 100)));
                }

                Menu.AddSubMenu(laneclearMenu);

                var jungleclearMenu = new Menu("Jungleclear", "Jungleclear");
                {
                    jungleclearMenu.AddItem(new MenuItem("GFUELQuinn.jungleclear.Q", "Use Q").SetValue(true));
                    jungleclearMenu.AddItem(new MenuItem("GFUELQuinn.jungleclear.E", "Use E").SetValue(true));
                    jungleclearMenu.AddItem(
                        new MenuItem("GFUELQuinn.jungleclear.Mana", "Minimum mana").SetValue(new Slider(20, 0, 100)));
                }

                Menu.AddSubMenu(jungleclearMenu);

                var killstealMenu = new Menu("Killsteal", "Killsteal");
                {
                    killstealMenu.AddItem(new MenuItem("GFUELElise.Killsteal.Q", "Killsteal Q").SetValue(true));
                }

                Menu.AddSubMenu(killstealMenu);

                var miscellaneousMenu = new Menu("Miscellaneous", "Miscellaneous");
                {
                    miscellaneousMenu.AddItem(new MenuItem("GFUELQuinn.Draw.Off", "Disable drawings").SetValue(false));
                    miscellaneousMenu.AddItem(new MenuItem("GFUELQuinn.Draw.Q", "Draw Q").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("GFUELQuinn.Draw.E", "Draw E").SetValue(new Circle()));

                    miscellaneousMenu.AddItem(
                        new MenuItem("GFUELQuinn.Misc.Antigapcloser", "Use E - Antigapcloser").SetValue(true));
                    miscellaneousMenu.AddItem(
                        new MenuItem("GFUELQuinn.Misc.Interrupter", "Use E - interrupter").SetValue(true));

                    var dmgAfterE = new MenuItem("GFUELQuinn.DrawComboDamage", "Draw combo damage").SetValue(true);
                    var drawFill =
                        new MenuItem("ElDiana.DrawColour", "Fill colour", true).SetValue(
                            new Circle(true, Color.Goldenrod));
                    miscellaneousMenu.AddItem(drawFill);
                    miscellaneousMenu.AddItem(dmgAfterE);

                    DamageIndicator.DamageToUnit = GetComboDamage;
                    DamageIndicator.Enabled = dmgAfterE.IsActive();
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

        //Credits to Legacy :-]
        private static Obj_AI_Base GetCenterMinion()
        {
            var minions = MinionManager.GetMinions(Q.Range + 500);
            var centerlocation =
                MinionManager.GetBestCircularFarmLocation(
                    minions.Select(x => x.Position.LSTo2D()).ToList(),
                    500,
                    Q.Range);

            return centerlocation.MinionsHit >= Menu.Item("GFUELQuinn.laneclear.count").GetValue<Slider>().Value
                       ? MinionManager.GetMinions(1000)
                             .OrderBy(x => x.LSDistance(centerlocation.Position))
                             .FirstOrDefault()
                       : null;
        }

        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            try
            {
                float damage = 0;

                if (!Player.Spellbook.IsAutoAttacking)
                {
                    damage += (float)ObjectManager.Player.LSGetAutoAttackDamage(enemy, true);
                }

                if (Q.LSIsReady())
                {
                    damage += Q.GetDamage(enemy);
                }

                if (E.LSIsReady())
                {
                    damage += E.GetDamage(enemy);
                }

                return damage;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return 0;
        }

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            try
            {
                if (IsActive("GFUELQuinn.Misc.Interrupter") && E.LSIsReady())
                {
                    if (sender.LSIsValidTarget(E.Range))
                    {
                        E.CastOnUnit(sender);
                    }
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
        ///     Called when the game draws itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnDraw(EventArgs args)
        {
            try
            {
                if (IsActive("GFUELQuinn.Draw.Off"))
                {
                    return;
                }

                if (Menu.Item("GFUELQuinn.Draw.Q").GetValue<Circle>().Active)
                {
                    if (Q.Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.White);
                    }
                }

                if (Menu.Item("GFUELQuinn.Draw.E").GetValue<Circle>().Active)
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
        ///     Killsteal logic
        /// </summary>
        private static void OnKillsteal()
        {
            try
            {
                foreach (
                    var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (enemy.LSIsValidTarget(Q.Range) && enemy.Health < Q.GetDamage(enemy))
                    {
                        var prediction = Q.GetPrediction(enemy);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            Q.Cast(prediction.CastPosition);
                        }
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

                //Console.WriteLine(Menu.Item("GFUELQuinn.laneclear.count").GetValue<Slider>().Value);

                //Console.WriteLine("Buffs: {0}", string.Join(" | ", Player.Buffs.Select(b => b.DisplayName)));

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

                if (IsActive("GFUELElise.Killsteal.Q"))
                {
                    OnKillsteal();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void OrbwalkingBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            try
            {
                if (!args.Unit.IsMe)
                {
                    return;
                }
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                    || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    if (!(args.Target is AIHeroClient))
                    {
                        return;
                    }

                    var target = HeroManager.Enemies.Find(x => x.HasBuff("quinnw") && x.LSIsValidTarget(Q.Range));
                    if (target == null)
                    {
                        return;
                    }
                    if (Orbwalking.InAutoAttackRange(target))
                    {
                        Orbwalker.ForceTarget(target);
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit
                    || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    var minion = args.Target as Obj_AI_Minion;
                    if (minion != null && minion.LSHasBuff("quinnw"))
                    {
                        Orbwalker.ForceTarget(minion);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
    }
}