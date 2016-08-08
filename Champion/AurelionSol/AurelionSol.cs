namespace ElAurelion_Sol
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using EloBuddy;

    internal class AurelionSol
    {

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

        /// <summary>
        ///     Gets or sets the W1 spell
        /// </summary>
        /// <value>
        ///     The W1 spell
        /// </value>
        private static Spell W1 { get; set; }

        private static bool HasPassive()
        {
            return Player.HasBuff("AurelionSolWActive");
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
                if (Player.ChampionName != "AurelionSol")
                {
                    return;
                }

                var igniteSlot = Player.LSGetSpellSlot("summonerdot");

                if (igniteSlot != SpellSlot.Unknown)
                {   
                    IgniteSpell = new Spell(igniteSlot, 600f);
                }


                Q = new Spell(SpellSlot.Q, 650f);
                W1 = new Spell(SpellSlot.W, 350f);
                W = new Spell(SpellSlot.W, 600f);
                E = new Spell(SpellSlot.E, 400f);
                R = new Spell(SpellSlot.R, 1420f);

                Q.SetSkillshot(0.25f, 180, 850, false, SkillshotType.SkillshotLine);
                R.SetSkillshot(0.25f, 300, 4500, false, SkillshotType.SkillshotLine);

                GenerateMenu();

                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                Orbwalking.BeforeAttack += OrbwalkingBeforeAttack;
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
                Menu = new Menu("ElAurelion Sol", "AurelionSol", true);

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
                    comboMenu.AddItem(new MenuItem("Combo.Q", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("Combo.W", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("Combo.R", "Use R").SetValue(true));
                    comboMenu.AddItem(new MenuItem("Combo.R.Multiple", "R multiple targets:").SetValue(true));
                    comboMenu.AddItem(new MenuItem("Combo.R.Count", "Auto R on:").SetValue(new Slider(3, 2, 4)));
                }

                Menu.AddSubMenu(comboMenu);

                var harassMenu = new Menu("Harass", "Harass");
                {
                    harassMenu.AddItem(new MenuItem("Harass.Q", "Use Q").SetValue(true));
                }
                Menu.AddSubMenu(harassMenu);

                var laneclearMenu = new Menu("Laneclear", "Laneclear");
                {
                    laneclearMenu.AddItem(new MenuItem("laneclear.Q", "Use Q").SetValue(true));
                    laneclearMenu.AddItem(
                        new MenuItem("laneclear.minionshit", "Minimum minions hit (Q)").SetValue(
                            new Slider(2, 1, 5)));
                    laneclearMenu.AddItem(
                        new MenuItem("laneclear.Mana", "Minimum mana").SetValue(new Slider(20, 0, 100)));
                }

                Menu.AddSubMenu(laneclearMenu);

                var jungleclearMenu = new Menu("Jungleclear", "Jungleclear");
                {
                    jungleclearMenu.AddItem(new MenuItem("jungleclear.Q", "Use Q").SetValue(true));
                    jungleclearMenu.AddItem(
                        new MenuItem("aneclear.minionshit", "Minimum minions killable (Q)").SetValue(
                            new Slider(1, 1, 5)));
                    jungleclearMenu.AddItem(
                        new MenuItem("jungleclear.Mana", "Minimum mana").SetValue(new Slider(20, 0, 100)));
                }

                Menu.AddSubMenu(jungleclearMenu);

                var killstealMenu = new Menu("Killsteal", "Killsteal");
                {
                    killstealMenu.AddItem(new MenuItem("Killsteal.Active", "Activate killsteal").SetValue(true));
                    killstealMenu.AddItem(new MenuItem("Killsteal.R", "Use R").SetValue(true));
                    killstealMenu.AddItem(new MenuItem("Ignite", "Use Ignite").SetValue(true));
                }

                Menu.AddSubMenu(killstealMenu);

                var miscMenu = new Menu("Misc", "Misc");
                {
                    miscMenu.AddItem(new MenuItem("Misc.Auto.W", "Auto deactivate W").SetValue(true));
                    miscMenu.AddItem(new MenuItem("AA.Block", "Don't use AA in combo").SetValue(false));
                    miscMenu.AddItem(new MenuItem("inter", "Anti interupt").SetValue(true));
                    miscMenu.AddItem(new MenuItem("gapcloser", "Anti gapcloser").SetValue(true));
                }

                Menu.AddSubMenu(miscMenu);


                var drawingsMenu = new Menu("Drawings", "Drawings");
                {
                    drawingsMenu.AddItem(new MenuItem("Draw.Off", "Disable drawings").SetValue(false));
                    drawingsMenu.AddItem(new MenuItem("Draw.Q", "Draw Q").SetValue(new Circle()));
                    drawingsMenu.AddItem(new MenuItem("Draw.W", "Draw W").SetValue(new Circle()));
                    drawingsMenu.AddItem(new MenuItem("Draw.R", "Draw R").SetValue(new Circle()));
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
        ///     The ignite killsteal logic
        /// </summary>
        private static void HandleIgnite()
        {
            try
            {
                var kSableEnemy =
                    HeroManager.Enemies.FirstOrDefault(
                        hero =>
                        hero.LSIsValidTarget(550) && ShieldCheck(hero) && !hero.HasBuff("summonerdot") && !hero.IsZombie
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
        /// Calculates the R damage
        /// </summary>
        /// <param name="target"></param>
        /// <returns>
        /// The R damage
        /// </returns>
        private static double RDamage(Obj_AI_Base target)
        {
            return Player.CalcDamage(target, Damage.DamageType.Magical,
                (float)new double[] { 200, 400, 600 }[R.Level - 1] + 0.70f * Player.TotalMagicalDamage);
        }
        /// <summary>
        /// Calculates the Q damage
        /// </summary>
        /// <param name="target"></param>
        /// <returns>
        /// The Q damage
        /// </returns>
        private static double QDamage(Obj_AI_Base target)
        {
            return Player.CalcDamage(target, Damage.DamageType.Magical,
                (float)new double[] { 70 , 110 , 150 , 190 , 230 }[Q.Level - 1] + 0.65f * Player.TotalMagicalDamage);
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

                if (Q.LSIsReady() && IsActive("Combo.Q"))
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                }

                if (W.LSIsReady() && IsActive("Combo.W"))
                {
                    if (!HasPassive())
                    {
                        if (target.LSIsValidTarget(W1.Range))
                        {
                            return;
                        }

                        if (Player.LSDistance(target) > W1.Range && Player.LSDistance(target) < W.Range)
                        {
                            W.Cast();
                        }
                    }
                    else if(HasPassive())
                    {
                        if (Player.LSDistance(target) > W1.Range && Player.LSDistance(target) < W.Range + 100)
                        {
                            return;
                        }

                        if (Player.LSDistance(target) > W1.Range + 150)
                        {
                            W.Cast();
                        }
                    }
                }

                if (R.LSIsReady() && IsActive("Combo.R") && RDamage(target) > target.Health + target.AttackShield) 
                {
                    var prediction = R.GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.High)
                    {   
                        R.Cast(prediction.CastPosition);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        private static void OrbwalkingBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Menu.Item("AA.Block").IsActive() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                args.Process = false;
            }
            else
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    args.Process =
                        !(Q.LSIsReady() 
                          || Player.LSDistance(args.Target) >= 1000);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {

            if (!IsActive("inter"))
            {
                return;
            }

            if (args.DangerLevel != Interrupter2.DangerLevel.High
                || sender.LSDistance(Player) > Q.Range)
            {
                return;
            }

            if (sender.LSIsValidTarget(Q.Range) && args.DangerLevel == Interrupter2.DangerLevel.High
                && Q.LSIsReady())
            {
                var prediction = Q.GetPrediction(sender);
                if (prediction.Hitchance >= HitChance.High)
                {
                    Q.Cast(prediction.CastPosition);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gapcloser"></param>
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!IsActive("gapcloser"))
            {
                return;
            }

            if (Q.LSIsReady() && gapcloser.Sender.LSDistance(Player) < Q.Range)
            {
                if (gapcloser.Sender.LSIsValidTarget(Q.Range) && Q.LSIsReady())
                {
                    var prediction = Q.GetPrediction(gapcloser.Sender);
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        Q.Cast(prediction.CastPosition);
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
            try
            {
                if (IsActive("Draw.Off"))
                {
                    return;
                }

                if (Menu.Item("Draw.W").GetValue<Circle>().Active)
                {

                    if (!HasPassive())
                    {
                        if (W.Level > 0)
                        {
                            Render.Circle.DrawCircle(ObjectManager.Player.Position, W1.Range, Color.Red);
                        }
                    }
                    else
                    {
                        if (W.Level > 0)
                        {
                            Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.MediumVioletRed);
                        }
                    }
                }
                if (Menu.Item("Draw.W").GetValue<Circle>().Active)
                {
                    if (Q.Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Goldenrod);
                    }
                }

                if (Menu.Item("Draw.R").GetValue<Circle>().Active)
                {
                    if (R.Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.DeepSkyBlue);
                    }
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        /// <summary>
        ///     Harass logic
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

                if (Q.LSIsReady() && IsActive("Harass.Q"))
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
        ///     The jungleclear "logic"
        /// </summary>
        private static void OnJungleclear()
        {
            try
            {
                var minion = MinionManager.GetMinions(
                    Player.ServerPosition,
                    Q.Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (minion == null)
                {
                    return;
                }

                if (Player.ManaPercent < Menu.Item("jungleclear.Mana").GetValue<Slider>().Value)
                {
                    return;
                }

                if (IsActive("jungleclear.Q") && Q.LSIsReady())
                {
                    var prediction = Q.GetCircularFarmLocation(minion);
                    if (prediction.MinionsHit >= 1)
                    {
                        Q.Cast(prediction.Position);
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
                    var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(R.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (Q.LSIsReady() && enemy.LSIsValidTarget(Q.Range) && enemy.Health < QDamage(enemy))
                    {
                        var prediction = Q.GetPrediction(enemy);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            Q.Cast(prediction.CastPosition);
                        }
                    }

                    if(R.LSIsReady() && enemy.LSIsValidTarget(R.Range) && enemy.Health < RDamage(enemy))
                    {
                        var prediction = R.GetPrediction(enemy);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            R.Cast(prediction.CastPosition);
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
        ///     The laneclear "logic"
        /// </summary>
        private static void OnLaneclear()
        {
            try
            {
                if (Player.ManaPercent < Menu.Item("laneclear.Mana").GetValue<Slider>().Value)
                {
                    return;
                }

                var minion = MinionManager.GetMinions(Player.Position, Q.Range);
                if (minion == null)
                {
                    return;
                }

                if (IsActive("laneclear.Q") && Q.LSIsReady())
                {
                    var prediction = Q.GetCircularFarmLocation(minion);
                    if (prediction.MinionsHit >= 2)
                    {
                        Q.Cast(prediction.Position);
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

                if (IsActive("Killsteal.Active"))
                {
                    OnKillsteal();
                }

                if (IsActive("Ignite"))
                {
                    HandleIgnite();
                }

                if (IsActive("Misc.Auto.W"))
                {
                    if (HasPassive() && Player.LSGetEnemiesInRange(2000f).Count == 0)
                    {
                        W.Cast();
                    }
                }

                if (IsActive("Combo.R.Multiple"))
                {
                    float RDistance = 1420;
                    float RWidth = 120;
                    var minREnemies = Menu.Item("Combo.R.Count").GetValue<Slider>().Value;
                    foreach (var enemy in HeroManager.Enemies)
                    {
                        var startPos = enemy.ServerPosition;
                        var endPos = Player.ServerPosition.LSExtend(startPos, Player.LSDistance(enemy) + RDistance);
                        var rectangle = new Geometry.Polygon.Rectangle(startPos, endPos, RWidth);

                        if (HeroManager.Enemies.Count(x => rectangle.IsInside(x)) >= minREnemies)
                        {
                            R.Cast(enemy);
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

        #endregion
    }
}