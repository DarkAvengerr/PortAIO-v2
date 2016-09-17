using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NasusTheLumberJack
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    /// This program is created by KarmaPanda
    /// </summary>
    internal class Program
    {
        #region Initilization

        /// <summary>
        /// Champion Name
        /// </summary>
        public const string ChampionName = "Nasus";

        /// <summary>
        /// The Spell Q
        /// </summary>
        private static Spell spellQ;

        /// <summary>
        /// The Spell W
        /// </summary>
        private static Spell spellW;

        /// <summary>
        /// The Spell E
        /// </summary>
        private static Spell spellE;

        /// <summary>
        /// The Spell R
        /// </summary>
        private static Spell spellR;

        /// <summary>
        /// The Orbwalker
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        private static Orbwalking.Orbwalker Orbwalker;

        /// <summary>
        /// The Menu
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        private static Menu Config;

        /// <summary>
        /// Player
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1630:DocumentationTextMustContainWhitespace", Justification = "Reviewed. Suppression is OK here.")]
        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        /// <summary>
        /// Called when program starts
        /// </summary>
        private static void Main()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        /// <summary>
        /// Called when game is loaded
        /// </summary>
        /// <param name="args">
        /// The Args
        /// </param>
        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.CharData.BaseSkinName != "Nasus")
            {
                return;
            }

            spellQ = new Spell(SpellSlot.Q, Player.AttackRange + 50);
            spellW = new Spell(SpellSlot.W, 600);
            spellE = new Spell(SpellSlot.E, 650);
            spellR = new Spell(SpellSlot.R, 20);

            Config = new Menu("Nasus the Lumber Jack", "kpNasus", true);
            Config.AddToMainMenu();

            // Target Selector
            var targetSelector = new Menu("Target Selector", "tsMenu");
            TargetSelector.AddToMenu(targetSelector);
            Config.AddSubMenu(targetSelector);

            // Orbwalker
            var orbwalkMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);
            Config.AddSubMenu(orbwalkMenu);

            // Combo
            var comboMenu = new Menu("Combo Config", "combo");
            comboMenu.AddItem(new MenuItem("useQCombo", "Use Q")).SetValue(true);
            comboMenu.AddItem(new MenuItem("useWCombo", "Use W")).SetValue(true);
            comboMenu.AddItem(new MenuItem("useECombo", "Use E")).SetValue(true);
            comboMenu.AddItem(new MenuItem("useRCombo", "Use R")).SetValue(true);
            comboMenu.AddItem(new MenuItem("useRHP", "HP before using R").SetValue(new Slider(35)));
            Config.AddSubMenu(comboMenu);

            // LastHit
            var lastHitMenu = new Menu("LastHit Config", "lasthit");
            lastHitMenu.AddItem(new MenuItem("useQLastHit", "Use Q To LastHit")).SetValue(true);
            lastHitMenu.AddItem(new MenuItem("manamanagerQ", "Mana Percent before using Q").SetValue(new Slider(50)));
            Config.AddSubMenu(lastHitMenu);

            // Harass
            var harassMenu = new Menu("Harass Config", "harass");
            harassMenu.AddItem(new MenuItem("useQHarass", "Use Q To Harass")).SetValue(false);
            harassMenu.AddItem(new MenuItem("useQHarass2", "Use Q To LastHit")).SetValue(true);
            harassMenu.AddItem(new MenuItem("manamanagerQ", "Mana Percent before using spellQ").SetValue(new Slider(50)));
            harassMenu.AddItem(new MenuItem("useWHarass", "Use W")).SetValue(false);
            harassMenu.AddItem(new MenuItem("useEHarass", "Use E to Harass")).SetValue(false);
            Config.AddSubMenu(harassMenu);

            // LaneClear
            var laneClearMenu = new Menu("LaneClear Config", "laneclear");
            laneClearMenu.AddItem(new MenuItem("laneclearQ", "Use W only when killable")).SetValue(true);
            laneClearMenu.AddItem(new MenuItem("laneclearE", "Use E")).SetValue(true);
            laneClearMenu.AddItem(new MenuItem("eKillOnly", "Use E only if killable")).SetValue(false);
            Config.AddSubMenu(laneClearMenu);
            
            // Misc
            var miscMenu = new Menu("Misc Config", "misc");
            miscMenu.AddItem(new MenuItem("aaDisable", "Disable AA if Q isn't active during LastHit and Mixed")).SetValue(false);
            Config.AddSubMenu(miscMenu);

            // Drawing
            var drawMenu = new Menu("Drawing", "Drawing");
            drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range")).SetValue(true);
            Config.AddSubMenu(drawMenu);
            
            // PrintChat
            Chat.Print("Nasus The Lumber Jack Loaded");

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.BeforeAttack += OrbwalkingBeforeAttack;
        }

        #endregion

        /// <summary>
        /// Combo Mode
        /// </summary>
        private static void Combo()
        {
            var wtarget = TargetSelector.GetTarget(spellW.Range, TargetSelector.DamageType.Physical);
            var etarget = TargetSelector.GetTarget(spellE.Range + spellE.Width, TargetSelector.DamageType.Magical);

            var useWCombo = Config.SubMenu("combo").Item("useWCombo").GetValue<bool>();
            var useECombo = Config.SubMenu("combo").Item("useECombo").GetValue<bool>();
            var useRCombo = Config.SubMenu("combo").Item("useRCombo").GetValue<bool>();
            var useRhp = Config.SubMenu("combo").Item("useRHP").GetValue<Slider>().Value;

            // spellW
            if (wtarget != null)
            {
                if (useWCombo && spellW.IsReady() && spellW.IsInRange(wtarget))
                {
                    spellW.Cast(wtarget);
                }
            }

            if (etarget != null)
            {
                // spellE
                if (useECombo && spellE.IsReady() && etarget.IsValidTarget() && spellE.IsInRange(etarget))
                {
                    var prediction = spellE.GetPrediction(etarget).Hitchance;
                    if (prediction >= HitChance.VeryHigh)
                    {
                        spellE.Cast(etarget);
                    }
                    else
                    {
                        return;
                    }
                } 
            }

            // spellR
            if (useRCombo && spellR.IsReady() && Player.CountEnemiesInRange(spellE.Range) >= 1 && Player.HealthPercent <= useRhp)
            {
                spellR.CastOnUnit(Player);
            }
        }

        /// <summary>
        /// LaneClear Mode
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private static void LaneClear()
        {
            var qMinion = MinionManager.GetMinions(spellQ.Range);
            var laneclearQ = Config.SubMenu("laneclear").Item("laneclearQ").GetValue<bool>();
            var laneclearE = Config.SubMenu("laneclear").Item("laneclearE").GetValue<bool>();
            var eKillOnly = Config.SubMenu("laneclear").Item("eKillOnly").GetValue<bool>();
            var eMinion = MinionManager.GetMinions(spellE.Range + spellE.Width);
            var eLocation = spellE.GetCircularFarmLocation(eMinion, spellE.Range);

            if (laneclearQ)
            {
                foreach (var minion in qMinion)
                {
                    if (minion.Health <= spellQ.GetDamage(minion))
                    {
                        spellQ.Cast();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                    }
                }
            }

            if (laneclearE && eKillOnly)
            {
                foreach (var minion in eMinion)
                {
                    if (minion.Health <= spellE.GetDamage(minion) && spellE.IsInRange(minion))
                    {
                        spellE.Cast(eLocation.Position);
                    }
                }
            }
            else if (laneclearE)
            {
                foreach (var minion in eMinion)
                {
                    if (spellE.IsInRange(minion))
                    {
                        spellE.Cast(eLocation.Position);
                    }
                }
            }
        }

        /// <summary>
        /// Harass Mode
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private static void Harass()
        {
            var useQHarass = Config.SubMenu("harass").Item("useQHarass").GetValue<bool>();
            var useWHarass = Config.SubMenu("harass").Item("useWHarass").GetValue<bool>();
            var useEHarass = Config.SubMenu("harass").Item("useEHarass").GetValue<bool>();

            if (useQHarass)
            {
                var target = TargetSelector.GetTarget(spellQ.Range, TargetSelector.DamageType.Physical);

                if (target.IsValidTarget() && spellQ.IsInRange(target) && spellQ.IsReady())
                {
                    spellQ.Cast();
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }
            }

            if (useWHarass)
            {
                var wTarget = TargetSelector.GetTarget(spellW.Range, TargetSelector.DamageType.Physical);

                if (wTarget.IsValidTarget(spellW.Range, true) && spellW.IsInRange(wTarget) && spellW.IsReady())
                {
                    spellW.Cast(wTarget);
                }
            }

            if (useEHarass)
            {
                var eTarget = TargetSelector.GetTarget(spellE.Range, TargetSelector.DamageType.Magical);

                if (eTarget.IsValidTarget(spellE.Range, true) && spellE.IsInRange(eTarget) && spellE.IsReady())
                {
                    spellE.CastIfHitchanceEquals(eTarget, HitChance.VeryHigh);
                }
            }
        }

        /// <summary>
        /// Called when game update
        /// </summary>
        /// <param name="args">
        /// The Args
        /// </param>
        private static void Game_OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }
        }

        /// <summary>
        /// Action Before Attacking
        /// </summary>
        /// <param name="args">
        /// The Args
        /// </param>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private static void OrbwalkingBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    var target = TargetSelector.GetTarget(spellQ.Range, TargetSelector.DamageType.Physical);
                    var useQCombo = Config.SubMenu("combo").Item("useQCombo").GetValue<bool>();

                    if (target.IsValidTarget() && spellQ.IsInRange(target) && spellQ.IsReady() && useQCombo)
                    {
                        spellQ.Cast();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }

                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    var useQLastHit = Config.SubMenu("lasthit").Item("useQLastHit").GetValue<bool>();
                    var aaDisable = Config.SubMenu("misc").Item("aaDisable").GetValue<bool>();
                    var manamanagerQ = Config.SubMenu("lasthit").Item("manamanagerQ").GetValue<Slider>().Value;
                    var minionQ = MinionManager.GetMinions(spellQ.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);

                    if (useQLastHit && aaDisable)
                    {
                        args.Process = false;
                        foreach (var minion in minionQ)
                        {
                            if (manamanagerQ <= Player.ManaPercent && minion.Health <= spellQ.GetDamage(minion) + Player.GetAutoAttackDamage(minion) && spellQ.IsReady())
                            {
                                spellQ.Cast();
                                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                                args.Process = true;
                            }
                        }
                    }
                    else if (useQLastHit)
                    {
                        foreach (var minion in minionQ)
                        {
                            if (manamanagerQ <= Player.ManaPercent && minion.Health <= spellQ.GetDamage(minion) + Player.GetAutoAttackDamage(minion) && spellQ.IsReady())
                            {
                                spellQ.Cast();
                                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                            }
                        }
                    }

                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    var useQHarass2 = Config.SubMenu("harass").Item("useQHarass2").GetValue<bool>();
                    aaDisable = Config.SubMenu("misc").Item("aaDisable").GetValue<bool>();

                    if (useQHarass2 && aaDisable)
                    {
                        args.Process = false;
                        minionQ = MinionManager.GetMinions(spellQ.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
                        manamanagerQ = Config.SubMenu("harass").Item("manamanagerQ").GetValue<Slider>().Value;

                        foreach (var minion in minionQ)
                        {
                            if (manamanagerQ <= Player.ManaPercent && minion.Health <= spellQ.GetDamage(minion) + Player.GetAutoAttackDamage(minion) && spellQ.IsReady())
                            {
                                spellQ.Cast();
                                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                                args.Process = true;
                            }
                        }
                    }
                    else if (useQHarass2)
                    {
                        minionQ = MinionManager.GetMinions(spellQ.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
                        manamanagerQ = Config.SubMenu("harass").Item("manamanagerQ").GetValue<Slider>().Value;

                        foreach (var minion in minionQ)
                        {
                            if (manamanagerQ <= Player.ManaPercent && minion.Health <= spellQ.GetDamage(minion) + Player.GetAutoAttackDamage(minion) && spellQ.IsReady())
                            {
                                spellQ.Cast();
                                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                            }
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Calls when game draws
        /// </summary>
        /// <param name="args">
        /// The Args
        /// </param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            var drawE = Config.SubMenu("Drawing").Item("DrawE").GetValue<bool>();

            if (drawE)
            {
                Render.Circle.DrawCircle(Player.Position, spellE.Range, spellE.IsReady() ? Color.YellowGreen : Color.Red);
            }
        }
    }
}