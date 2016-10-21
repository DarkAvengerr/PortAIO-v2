namespace GFUELTalon
{
    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;
    using EloBuddy;

    internal class Talon
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
        ///     Gets Ravenous Hydra
        /// </summary>
        /// <value>
        ///     Ravenous Hydra
        /// </value>
        private static Items.Item Hydra
        {
            get
            {
                return ItemData.Ravenous_Hydra_Melee_Only.GetItem();
            }
        }

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
        ///     Gets Tiamat Item
        /// </summary>
        /// <value>
        ///     Tiamat Item
        /// </value>
        private static Items.Item Tiamat
        {
            get
            {
                return ItemData.Tiamat_Melee_Only.GetItem();
            }
        }

        /// <summary>
        ///     Gets Titanic Hydra
        /// </summary>
        /// <value>
        ///     Titanic Hydra
        /// </value>
        /// 
        private static Items.Item Titanic
        {
            get
            {
                return ItemData.Titanic_Hydra_Melee_Only.GetItem();
            }
        }

        /// <summary>
        ///     Gets or sets the W spell
        /// </summary>
        /// <value>
        ///     The W spell
        /// </value>
        private static Spell W { get; set; }

        /// <summary>
        ///     Gets Youmuus Ghostblade
        /// </summary>
        /// <value>
        ///     Youmuus Ghostblade
        /// </value>
        /// 
        private static Items.Item Youmuu
        {
            get
            {
                return ItemData.Youmuus_Ghostblade.GetItem();
            }
        }

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
                if (Player.ChampionName != "Talon")
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

                Q = new Spell(SpellSlot.Q, Orbwalking.GetRealAutoAttackRange(Player) + 100);
                W = new Spell(SpellSlot.W, 600f);
                E = new Spell(SpellSlot.E, 700f);
                R = new Spell(SpellSlot.R, 650f);

                W.SetSkillshot(0.25f, 75, 2300, false, SkillshotType.SkillshotLine);

                GenerateMenu();

                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
                Orbwalking.AfterAttack += OrbwalkingAfterAttack;
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
                if (IsActive("GFUELTalon.Misc.Antigapcloser") && E.IsReady())
                {
                    if (gapcloser.Sender.IsValidTarget(E.Range))
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
        ///     Combo logic
        /// </summary>
        private static void DoCombo()
        {
            try
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (target == null)
                {
                    return;
                }

                if (IsActive("GFUELTalon.Combo.E") && target.IsValidTarget(E.Range) && E.IsReady())
                {
                    if (IsActive("GFUELTalon.Combo.Towercheck"))
                    {
                        var underTower = target.UnderTurret();
                        if (underTower)
                        {
                            return;
                        }
                    }

                    E.Cast(target);
                }

                if (IsActive("GFUELTalon.Combo.R") && R.IsReady())
                {
                    if (IsActive("GFUELTalon.Combo.Killability"))
                    {
                        if (IsActive("GFUELTalon.Combo.Overkill.R"))
                        {
                            if (Player.GetSpellDamage(target, SpellSlot.R) > target.Health + 75
                                && (Q.IsReady() || E.IsReady() || W.IsReady()))
                            {
                                return;
                            }
                        }

                        if (GetComboDamage(target) > target.Health && target.IsValidTarget(R.Range - 50))
                        {
                            R.Cast();
                        }
                    }

                    if (IsActive("GFUELTalon.Combo.Count"))
                    {
                        foreach (
                            var x in
                                HeroManager.Enemies.Where((hero => !hero.IsDead && hero.IsValidTarget(R.Range - 50))))
                        {
                            var pred = R.GetPrediction(x);
                            if (pred.AoeTargetsHitCount >= Menu.Item("GFUELTalon.Combo.Count").GetValue<Slider>().Value)
                            {
                                R.Cast();
                            }
                        }
                    }
                }

                if (IsActive("GFUELTalon.Combo.W") && target.Distance(Player.Position) < W.Range && W.IsReady())
                {
                    var prediction = W.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        W.Cast(prediction.CastPosition);
                    }
                }

                if (IsActive("GFUELTalon.Combo.Q") && target.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    Q.Cast();
                }

                if (Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) >= target.Health)
                {
                    if (IgniteSpell.Slot != SpellSlot.Unknown)
                    {
                        Player.Spellbook.CastSpell(IgniteSpell.Slot, target);
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
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (target == null)
                {
                    return;
                }

                if (IsActive("GFUELTalon.Harass.W") && target.Distance(Player.Position) < W.Range && W.IsReady())
                {
                    var prediction = W.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        W.Cast(prediction.CastPosition);
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
                        E.Range,
                        MinionTypes.All,
                        MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth).OrderByDescending(x => x.MaxHealth).FirstOrDefault();

                if (minion == null)
                {
                    return;
                }

                if (Player.ManaPercent < Menu.Item("GFUELTalon.jungleclear.Mana").GetValue<Slider>().Value)
                {
                    return;
                }


                if (IsActive("GFUELTalon.jungleclear.E") && E.IsReady())
                {
                    E.CastOnUnit(minion);
                }

                if (IsActive("GFUELTalon.jungleclear.W") && W.IsReady() && minion.IsValidTarget(W.Range)) 
                {
                    W.Cast(minion);
                }

                if (IsActive("GFUELTalon.jungleclear.Q") && Q.IsReady() && minion.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }

                var min =
                    MinionManager.GetMinions(
                        Orbwalking.GetRealAutoAttackRange(Player),
                        MinionTypes.All,
                        MinionTeam.NotAlly).Count();

                if (min >= 1)
                {
                    if (Tiamat.IsReady())
                    {
                        Tiamat.Cast();
                    }

                    if (Hydra.IsReady())
                    {
                        Hydra.Cast();
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
                var minion = MinionManager.GetMinions(Player.Position, E.Range).MinOrDefault(x => x.Health);
                if (minion == null)
                {
                    return;
                }

                if (Player.ManaPercent < Menu.Item("GFUELTalon.laneclear.Mana").GetValue<Slider>().Value)
                {
                    return;
                }

                if (IsActive("GFUELTalon.laneclear.W") && W.IsReady())
                {
                    if (GetCenterMinion().IsValidTarget())
                    {
                        W.Cast(GetCenterMinion());
                    }
                }

                var min =
                    MinionManager.GetMinions(
                        Orbwalking.GetRealAutoAttackRange(Player),
                        MinionTypes.All,
                        MinionTeam.NotAlly).Count();

                if (min >= 3)
                {
                    if (Tiamat.IsReady())
                    {
                        Tiamat.Cast();
                    }

                    if (Hydra.IsReady())
                    {
                        Hydra.Cast();
                    }
                }

                if (IsActive("GFUELTalon.laneclear.E") && E.IsReady())
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
                Menu = new Menu("GFUEL TALON", "GFUELTALON", true);

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
                    comboMenu.AddItem(new MenuItem("GFUELTalon.Combo.Q", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("GFUELTalon.Combo.W", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("GFUELTalon.Combo.E", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("GFUELTalon.Combo.Items", "Use Items").SetValue(true));
                    comboMenu.AddItem(new MenuItem("GFUELTalon.Combo.Towercheck", "Check under tower").SetValue(false));

                    comboMenu.SubMenu("R Settings").AddItem(new MenuItem("GFUELTalon.Combo.R", "Use R").SetValue(true));
                    comboMenu.SubMenu("R Settings")
                        .AddItem(new MenuItem("GFUELTalon.Combo.Killability", "R on killability").SetValue(true));
                    comboMenu.SubMenu("R Settings")
                        .AddItem(new MenuItem("GFUELTalon.Combo.Count", "R when ult can hit"))
                        .SetValue(new Slider(1, 1, 5));
                    comboMenu.SubMenu("R Settings")
                        .AddItem(new MenuItem("GFUELTalon.Combo.Overkill.R", "Check R overkill").SetValue(false));
                }

                Menu.AddSubMenu(comboMenu);

                var harassMenu = new Menu("Harass", "Harass");
                {
                    harassMenu.AddItem(new MenuItem("GFUELTalon.Harass.W", "Use W").SetValue(true));
                }

                Menu.AddSubMenu(harassMenu);

                var laneclearMenu = new Menu("Laneclear", "Laneclear");
                {
                    laneclearMenu.AddItem(new MenuItem("GFUELTalon.laneclear.E", "Use E").SetValue(false));
                    laneclearMenu.AddItem(new MenuItem("GFUELTalon.laneclear.W", "Use W").SetValue(true));

                    laneclearMenu.AddItem(new MenuItem("GFUELTalon.laneclear.count", "Minimum minion count"))
                        .SetValue(new Slider(3, 2, 6));

                    laneclearMenu.AddItem(
                        new MenuItem("GFUELTalon.laneclear.Mana", "Minimum mana").SetValue(new Slider(20, 0, 100)));
                }

                Menu.AddSubMenu(laneclearMenu);

                var jungleclearMenu = new Menu("Jungleclear", "Jungleclear");
                {
                    jungleclearMenu.AddItem(new MenuItem("GFUELTalon.jungleclear.Q", "Use Q").SetValue(false));
                    jungleclearMenu.AddItem(new MenuItem("GFUELTalon.jungleclear.E", "Use E").SetValue(false));
                    jungleclearMenu.AddItem(new MenuItem("GFUELTalon.jungleclear.W", "Use W").SetValue(true));
                    jungleclearMenu.AddItem(
                        new MenuItem("GFUELTalon.jungleclear.Mana", "Minimum mana").SetValue(new Slider(20, 0, 100)));
                }

                Menu.AddSubMenu(jungleclearMenu);

                var killstealMenu = new Menu("Killsteal", "Killsteal");
                {
                    killstealMenu.AddItem(new MenuItem("GFUELTalon.Killsteal.W", "Killsteal W").SetValue(false));
                }

                Menu.AddSubMenu(killstealMenu);

                var miscellaneousMenu = new Menu("Miscellaneous", "Miscellaneous");
                {
                    miscellaneousMenu.AddItem(new MenuItem("GFUELTalon.Draw.Off", "Disable drawings").SetValue(false));
                    miscellaneousMenu.AddItem(new MenuItem("GFUELTalon.Draw.W", "Draw E").SetValue(true));

                    miscellaneousMenu.AddItem(
                        new MenuItem("GFUELTalon.Misc.Antigapcloser", "Use E - Antigapcloser").SetValue(true));

                    var dmgAfterE = new MenuItem("GFUELTalon.DrawComboDamage", "Draw combo damage").SetValue(true);
                    var drawFill =
                        new MenuItem("GFUELTalon.DrawColour", "Fill colour", true).SetValue(
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

        private static Obj_AI_Base GetCenterMinion()
        {
            var minions = MinionManager.GetMinions(E.Range);
            var centerlocation =
                MinionManager.GetBestCircularFarmLocation(minions.Select(x => x.Position.To2D()).ToList(), 500, E.Range);

            return centerlocation.MinionsHit >= Menu.Item("GFUELTalon.laneclear.count").GetValue<Slider>().Value
                       ? MinionManager.GetMinions(1000)
                             .OrderBy(x => x.Distance(centerlocation.Position))
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
                    damage += (float)Player.GetAutoAttackDamage(enemy, true);
                }

                if (IgniteSpell.IsReady() && IgniteSpell.Slot != SpellSlot.Unknown)
                {
                    damage += (float)Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
                }

                if (Hydra.IsReady())
                {
                    damage += (float)Player.GetItemDamage(enemy, Damage.DamageItems.Hydra);
                }

                if (Tiamat.IsReady())
                {
                    damage += (float)Player.GetItemDamage(enemy, Damage.DamageItems.Tiamat);
                }

                if (Youmuu.IsReady())
                {
                    damage += (float)Player.GetAutoAttackDamage(enemy, true) * 2;
                }

                if (Q.IsReady())
                {
                    damage += Q.GetDamage(enemy);
                }

                if (E.IsReady())
                {
                    damage += E.GetDamage(enemy);
                }

                if (W.IsReady())
                {
                    damage += W.GetDamage(enemy);
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

                if (IsActive("GFUELTalon.Draw.W") && W.IsReady())
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

                    if (target.IsValidTarget() == false)
                    {
                        return;
                    }

                    var polygon = new Geometry.Polygon.Sector(
                        ObjectManager.Player.Position,
                        target.Position,
                        54 * (float)Math.PI / 180,
                        700);
                    polygon.UpdatePolygon();
                    polygon.Draw(Color.Aqua);
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
                    var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (enemy.IsValidTarget(W.Range) && enemy.Health < W.GetDamage(enemy))
                    {
                        var prediction = W.GetPrediction(enemy);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            W.Cast(prediction.CastPosition);
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

                if (IsActive("GFUELTalon.Killsteal.W"))
                {
                    OnKillsteal();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void OrbwalkingAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            try
            {
                var enemy = target as Obj_AI_Base;
                if (!unit.IsMe || enemy == null || !(target is AIHeroClient))
                {
                    return;
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                    || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    if (target.IsValidTarget(Q.Range))
                    {
                        Q.Cast();
                    }
                }

                if (IsActive("GFUELTalon.Combo.Items"))
                {
                    UseItems(enemy);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Credits to Trees
        private static bool UseItems(Obj_AI_Base target)
        {
            if (Player.IsDashing() || Player.Spellbook.IsAutoAttacking)
            {
                return false;
            }

            var youmuus = Youmuu;
            if (Youmuu.IsReady() && youmuus.Cast())
            {
                return true;
            }

            var heroes = Player.GetEnemiesInRange(385).Count;
            var count = heroes;

            var tiamat = Tiamat;
            if (tiamat.IsReady() && count > 0 && tiamat.Cast())
            {
                return true;
            }

            var hydra = Hydra;
            if (Hydra.IsReady() && count > 0 && hydra.Cast())
            {
                return true;
            }

            var titanic = Titanic;
            return titanic.IsReady() && count > 0 && titanic.Cast();
        }

        #endregion
    }
}