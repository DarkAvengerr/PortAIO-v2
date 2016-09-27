using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ThreshFlayMaker
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    /// <summary>
    /// The program class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Gets or sets the q.
        /// </summary>
        /// <value>
        /// The q.
        /// </value>
        private static Spell Q { get; set; }

        /// <summary>
        /// Gets or sets the w.
        /// </summary>
        /// <value>
        /// The w.
        /// </value>
        private static Spell W { get; set; }

        /// <summary>
        /// Gets or sets the e.
        /// </summary>
        /// <value>
        /// The e.
        /// </value>
        private static Spell E { get; set; }

        /// <summary>
        /// Gets or sets the r.
        /// </summary>
        /// <value>
        /// The r.
        /// </value>
        private static Spell R { get; set; }

        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        /// <value>
        /// The menu.
        /// </value>
        private static Menu Menu { get; set; }

        /// <summary>
        /// Gets the player.
        /// </summary>
        /// <value>
        /// The player.
        /// </value>
        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        /// <summary>
        /// Gets or sets the orbwalker.
        /// </summary>
        /// <value>
        /// The orbwalker.
        /// </value>
        private static Orbwalking.Orbwalker Orbwalker { get; set; }

        /// <summary>
        /// Gets the hooked target.
        /// </summary>
        /// <value>
        /// The hooked target.
        /// </value>
        private static Obj_AI_Base HookedTarget
        {
            get
            {
                return ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.IsEnemy && x.HasBuff("ThreshQ"));
            }
        }



        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Thresh")
            {
                return;
            }
            
            Q = new Spell(SpellSlot.Q, 1075);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 500);
            R = new Spell(SpellSlot.R, 350);

            Q.SetSkillshot(0.35f, 60, 1200, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 300, 1750, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(1, 110, 2000, false, SkillshotType.SkillshotLine);

            CreateMenu();

            Chat.Print("<font color=\"#7CFC00\"><b>Thresh the Flay Maker:</b></font> by ChewyMoon & Shiver loaded");

            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnNewPath += Obj_AI_Base_OnNewPath;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        /// <summary>
        /// Called when the game is drawn.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            var drawQ = Menu.Item("DrawQ").IsActive();
            var drawW = Menu.Item("DrawW").IsActive();
            var drawE = Menu.Item("DrawE").IsActive();
            var drawR = Menu.Item("DrawR").IsActive();

            if (drawQ)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawW)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, W.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawE)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawR)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.Aqua : Color.Red);
            }
        }

        /// <summary>
        /// Fired when there is a enemy gapcloser.
        /// </summary>
        /// <param name="gapcloser">The gapcloser.</param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!gapcloser.Sender.IsValidTarget() || !Menu.Item("EGapcloser").IsActive())
            {
                return;
            }

            E.Cast(gapcloser.Sender);

        }

        /// <summary>
        /// Fired when there is an interruptable target.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Interrupter2.InterruptableTargetEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!sender.IsValidTarget(Q.Range) || args.DangerLevel != Interrupter2.DangerLevel.High)
            {
                return;
            }

            if (E.IsReady() && E.IsInRange(sender) && Menu.Item("EInterrupt").IsActive())
            {
                E.Cast(sender);
            }
            else if (Q.IsReady() && Menu.Item("QInterrupt").IsActive())
            {
                Q.Cast(sender);
            }
        }

        /// <summary>
        /// Called when an object has a new path.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectNewPathEventArgs"/> instance containing the event data.</param>
        private static void Obj_AI_Base_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (!sender.IsValid<AIHeroClient>() || !args.IsDash || !sender.IsValidTarget(Q.Range))
            {
                return;
            }

            //Chat.Print("DASH");

            if (E.IsReady() && E.IsInRange(sender) && Menu.Item("EDash").IsActive())
            {
                var endPosition = args.Path.Last();
                var isFleeing = endPosition.Distance(Player.ServerPosition) > Player.Distance(sender);

                var prediction = E.GetPrediction(sender);

                if (prediction.Hitchance != HitChance.VeryHigh)
                {
                    return;
                }

                var x = Player.ServerPosition.X - endPosition.X;
                var y = Player.ServerPosition.Y - endPosition.Y;

                var vector = new Vector3(
                    Player.ServerPosition.X + x,
                    Player.ServerPosition.Y + y,
                    Player.ServerPosition.Z);

                E.Cast(
                    !isFleeing
                        ? prediction.CastPosition
                        : vector);
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    DoCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    DoHarass();
                    break;
            }

            AutoW();
            AutoQ();
        }

        /// <summary>
        /// Automatics the q.
        /// </summary>
        private static void AutoQ()
        {
            if (!Menu.Item("QImmobile").IsActive())
            {
                return;
            }

            var autoQTarget =
                HeroManager.Enemies.FirstOrDefault(
                    x =>
                    x.HasBuffOfType(BuffType.Charm) || x.HasBuffOfType(BuffType.Knockup)
                    || x.HasBuffOfType(BuffType.Stun) || x.HasBuffOfType(BuffType.Suppression)
                    || x.HasBuffOfType(BuffType.Snare));

            if (autoQTarget != null && HookedTarget == null)
            {
                Q.Cast(autoQTarget);
            }
        }

        /// <summary>
        /// Automatics the w.
        /// </summary>
        private static void AutoW()
        {
            var lanternLowAllies = Menu.Item("WLowAllies").IsActive();
            var lanternHealthPercent = Menu.Item("WAllyPercent").GetValue<Slider>().Value;

            if (lanternLowAllies)
            {
                var ally =
                    HeroManager.Allies.Where(
                        x => x.IsValidTarget(W.Range, false) && x.HealthPercent < lanternHealthPercent)
                        .OrderByDescending(x => Menu.Item("W" + x.ChampionName).GetValue<Slider>().Value)
                        .FirstOrDefault();

                if (ally != null && ally.CountEnemiesInRange(700) >= 1)
                {
                    W.Cast(W.GetPrediction(ally).CastPosition);
                }
            }


        }

        /// <summary>
        /// Does the harass.
        /// </summary>
        private static void DoHarass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (!target.IsValidTarget())
            {
                return;
            }

            var useQ1 = Menu.Item("UseQ1Harass").IsActive();
            var useQ2 = Menu.Item("UseQ2Harass").IsActive();
            var useE = Menu.Item("UseEHarass").IsActive();

            if (Q.IsReady())
            {
                if (useQ1 && HookedTarget == null)
                {
                    Q.Cast(target);
                }

                if (useQ2 && HookedTarget.IsValid<AIHeroClient>())
                {
                    Q.Cast();
                }
            }

            if (useE && E.IsReady() && HookedTarget == null)
            {
                var isFleeing = Player.Distance(target) < target.Distance(Game.CursorPos);
                var prediction = E.GetPrediction(target);

                if (prediction.Hitchance != HitChance.VeryHigh)
                {
                    return;
                }

                var x = Player.ServerPosition.X - target.ServerPosition.X;
                var y = Player.ServerPosition.Y - target.ServerPosition.Y;

                var vector = new Vector3(
                    Player.ServerPosition.X + x,
                    Player.ServerPosition.Y + y,
                    Player.ServerPosition.Z);

                E.Cast(
                    isFleeing
                        ? prediction.CastPosition
                        : vector);
            }
        }

        /// <summary>
        /// Does the combo.
        /// </summary>
        private static void DoCombo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (!target.IsValidTarget())
            {
                return;
            }

            var useQ = Menu.Item("UseQCombo").IsActive();
            var useW = Menu.Item("UseWCombo").IsActive();
            var useE = Menu.Item("UseECombo").IsActive();
            var useR = Menu.Item("UseRCombo").IsActive();
            var ultEnemies = Menu.Item("UseRComboEnemies").GetValue<Slider>().Value;

            if (useQ && Q.IsReady())
            {
                if (HookedTarget.IsValid<AIHeroClient>())
                {
                    Q.Cast();
                }
                else if (HookedTarget == null)
                {
                    Q.Cast(target);
                }
            }

            if (useW && W.IsReady() && HookedTarget.IsValid<AIHeroClient>())
            {
                var ally =
                    HeroManager.Allies.Where(x => !x.IsMe && x.IsValidTarget(W.Range, false) && x.Distance(Player) > 300)
                        .OrderByDescending(x => Menu.Item("W" + x.ChampionName).GetValue<Slider>().Value)
                        .FirstOrDefault();

                if (ally != null)
                {
                    W.Cast(W.GetPrediction(ally).CastPosition);
                }
                else if (Player.HealthPercent < 50)
                {
                    W.Cast(W.GetPrediction(Player).CastPosition);
                }
            }

            if (useE && E.IsReady() && HookedTarget == null)
            {
                var isFleeing = Player.Distance(target) < target.Distance(Game.CursorPos);
                var prediction = E.GetPrediction(target);

                if (prediction.Hitchance == HitChance.VeryHigh)
                {
                    var x = Player.ServerPosition.X - target.ServerPosition.X;
                    var y = Player.ServerPosition.Y - target.ServerPosition.Y;

                    var vector = new Vector3(
                        Player.ServerPosition.X + x,
                        Player.ServerPosition.Y + y,
                        Player.ServerPosition.Z);

                    E.Cast(
                        isFleeing
                            ? prediction.CastPosition
                            : vector);
                }
            }

            if (useR && R.IsReady() && Player.CountEnemiesInRange(R.Range) >= ultEnemies)
            {
                R.Cast();
            }

        }

        /// <summary>
        /// Creates the menu.
        /// </summary>
        private static void CreateMenu()
        {
            Menu = new Menu("Thresh the Flay Maker", "cmshiverThresh", true);

            var tsMenu = new Menu("Target Selector", "TS");
            TargetSelector.AddToMenu(tsMenu);
            Menu.AddSubMenu(tsMenu);

            var orbwalkMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);
            Menu.AddSubMenu(orbwalkMenu);

            var comboMenu = new Menu("Combo Settings", "Combo");
            comboMenu.AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseRCombo", "Use R").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseRComboEnemies", "R Min Enemies >=").SetValue(new Slider(2, 1, 5)));
            Menu.AddSubMenu(comboMenu);

            var harassMenu = new Menu("Harass Settings", "Harass");
            harassMenu.AddItem(new MenuItem("UseQ1Harass", "Use Q1 (Hook)").SetValue(true));
            harassMenu.AddItem(new MenuItem("UseQ2Harass", "Use Q2 (Fly)").SetValue(false));
            harassMenu.AddItem(new MenuItem("UseEHarass", "Use E").SetValue(true));
            Menu.AddSubMenu(harassMenu);

            var lanternMenu = new Menu("Lantern Settings", "WSettings");
            var priorityMenu = new Menu("Priorities", "Prioriy");
            foreach (var ally in HeroManager.Allies.Select(x => x.ChampionName))
            {
                priorityMenu.AddItem(
                    new MenuItem(string.Format("W{0}", ally), ally).SetValue(new Slider(5, 0, HeroManager.Allies.Count)));
            }

            lanternMenu.AddSubMenu(priorityMenu);
            lanternMenu.AddItem(new MenuItem("WLowAllies", "W Low Allies").SetValue(true));
            lanternMenu.AddItem(new MenuItem("WAllyPercent", "Ally Health Percent").SetValue(new Slider(30)));
            Menu.AddSubMenu(lanternMenu);

            // Dash -> You : Away | Dash <- You : Towards
            var flayMenu = new Menu("Flay Settings", "Flay");
            flayMenu.AddItem(new MenuItem("EDash", "E on Dash (Smart)").SetValue(true));
            flayMenu.AddItem(new MenuItem("EInterrupt", "E to Interrupt").SetValue(true));
            flayMenu.AddItem(new MenuItem("EGapcloser", "E on Incoming Gapcloser").SetValue(true));
            Menu.AddSubMenu(flayMenu);

            var hookMenu = new Menu("Hook Settings", "Hook");
            hookMenu.AddItem(new MenuItem("QInterrupt", "Q to Interrupt").SetValue(true));
            hookMenu.AddItem(new MenuItem("QImmobile", "Q on Immobile").SetValue(true));
            // TODO: Predict flash with advanced algorithms?
            Menu.AddSubMenu(hookMenu);

            var drawMenu = new Menu("Drawing Settings", "Draw");
            drawMenu.AddItem(new MenuItem("DrawQ", "Draw Q").SetValue(true));
            drawMenu.AddItem(new MenuItem("DrawW", "Draw W").SetValue(false));
            drawMenu.AddItem(new MenuItem("DrawE", "Draw E").SetValue(false));
            drawMenu.AddItem(new MenuItem("DrawR", "Draw R").SetValue(false));
            Menu.AddSubMenu(drawMenu);

            Menu.AddToMainMenu();
        }
    }
}
