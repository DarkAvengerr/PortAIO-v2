// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="SurvivorRyze">
//      Copyright (c) SurvivorRyze. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using SharpDX;
using Color = SharpDX.Color;
using HitChance = SebbyLib.Prediction.HitChance;
using PredictionInput = SebbyLib.Prediction.PredictionInput;

using EloBuddy; 
using LeagueSharp.Common; 
namespace SurvivorRyze
{
    internal class Program
    {
        public static void Main()
        {
            Game_OnGameLoad(new EventArgs());
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != ChampionName)
                return;

            #region Spells

            _q = new Spell(SpellSlot.Q, 1000f, TargetSelector.DamageType.Magical);
            _q.SetSkillshot(0.25f, 50f, 1700, true, SkillshotType.SkillshotLine);
            _w = new Spell(SpellSlot.W, 600f, TargetSelector.DamageType.Magical);
            _e = new Spell(SpellSlot.E, 600f, TargetSelector.DamageType.Magical);
            _r = new Spell(SpellSlot.R);
            _r.SetSkillshot(2.5f, 450f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            #endregion

            _igniteSlot = Player.GetSpellSlot("summonerdot");

            #region Menu

            _menu = new Menu("SurvivorRyze", "SurvivorRyze", true).SetFontStyle(FontStyle.Bold, Color.DodgerBlue);

            var orbwalkerMenu = _menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            _orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            var targetSelectorMenu = _menu.AddSubMenu(new Menu("Target Selector", "TargetSelector"));
            TargetSelector.AddToMenu(targetSelectorMenu);

            var comboMenu = _menu.AddSubMenu(new Menu("Combo", "Combo"));
            comboMenu.AddItem(
                new MenuItem("ComboMode", "Combo Mode:").SetValue(
                        new StringList(new[] {"Burst", "Survivor Mode (Shield)", "As Fast As Possible (Spam)"}))
                    .SetTooltip("Survivor Mode - Will try to stack Shield 99% of the time.")
                    .SetFontStyle(FontStyle.Bold, Color.Chartreuse));
            comboMenu.AddItem(
                new MenuItem("UseComboPlus", "Use Special Combo?").SetValue(true)
                    .SetTooltip("Not the best if you've problems casting spells thru minions and stuff.")
                    .SetFontStyle(FontStyle.Bold, Color.Chartreuse));
            comboMenu.AddItem(new MenuItem("CUseQ", "Cast Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("CUseW", "Cast W").SetValue(true));
            comboMenu.AddItem(new MenuItem("CUseE", "Cast E").SetValue(true));
            comboMenu.AddItem(
                new MenuItem("SmartAABlock", "Smart AA Blocking").SetValue(true)
                    .SetTooltip("Turn this on and it'll AA in Combo only until you get level 6 after that it'll stop."));
            comboMenu.AddItem(
                new MenuItem("CBlockAA", "Block AA in Combo Mode").SetValue(true)
                    .SetTooltip("Turn this on, if Smart AA Blocking is OFF"));
            comboMenu.AddItem(
                new MenuItem("Combo2TimesMana", "Champion needs to have mana for atleast 2 times (Q/W/E)?").SetValue(
                        false)
                    .SetTooltip(
                        "If it's set to 'false' it'll need atleast mana for Q/W/E [1x] Post in thread if needs a change"));
            comboMenu.AddItem(new MenuItem("CUseR", "Ultimate (R) in Ultimate Menu"));
            comboMenu.AddItem(new MenuItem("CUseIgnite", "Use Ignite (Smart)").SetValue(true));

            var harassMenu = _menu.AddSubMenu(new Menu("Harass", "Harass"));
            harassMenu.AddItem(new MenuItem("HarassQ", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("HarassW", "Use W").SetValue(false));
            harassMenu.AddItem(new MenuItem("HarassE", "Use E").SetValue(false));
            harassMenu.AddItem(new MenuItem("HarassManaManager", "Mana Manager (%)").SetValue(new Slider(30, 1)));

            var laneClearMenu = _menu.AddSubMenu(new Menu("Lane Clear", "LaneClear"));
            laneClearMenu.AddItem(
                new MenuItem("EnableMouseScroll", "Enable Mouse Scroll to change Spell Farming?").SetValue(true));
            laneClearMenu.AddItem(
                    new MenuItem("EnableFarming", "Enable Farming with Spells?").SetValue(true)
                        .SetTooltip("You either change the value here by clicking or by Scrolling Down using the mouse"))
                .Permashow(true, "Farming with Spells?");
            laneClearMenu.AddItem(new MenuItem("UseQLC", "Use Q to LaneClear").SetValue(true));
            laneClearMenu.AddItem(new MenuItem("UseELC", "Use E to LaneClear").SetValue(true));
            laneClearMenu.AddItem(
                new MenuItem("LaneClearManaManager", "Mana Manager (%)").SetValue(new Slider(30, 1)));

            var jungleClearMenu = _menu.AddSubMenu(new Menu("Jungle Clear", "JungleClear"));
            jungleClearMenu.AddItem(new MenuItem("UseQJC", "Use Q to JungleClear").SetValue(true));
            jungleClearMenu.AddItem(new MenuItem("UseWJC", "Use W to JungleClear").SetValue(true));
            jungleClearMenu.AddItem(new MenuItem("UseEJC", "Use E to JungleClear").SetValue(true));
            jungleClearMenu.AddItem(
                new MenuItem("JungleClearManaManager", "Mana Manager (%)").SetValue(new Slider(30, 1)));

            var lastHitMenu = _menu.AddSubMenu(new Menu("Last Hit", "LastHit"));
            lastHitMenu.AddItem(new MenuItem("UseQLH", "Use Q to LastHit").SetValue(true));
            lastHitMenu.AddItem(new MenuItem("UseELH", "Use E to LastHit").SetValue(true));
            lastHitMenu.AddItem(new MenuItem("LaneHitManaManager", "Mana Manager (%)").SetValue(new Slider(30, 1)));

            var itemsMenu = _menu.AddSubMenu(new Menu("Items Menu", "ItemsMenu"));
            itemsMenu.AddItem(new MenuItem("UsePotions", "Use Potions").SetValue(true));
            itemsMenu.AddItem(
                new MenuItem("UseSmartPotion", "Use Smart Potion Logic").SetValue(true)
                    .SetTooltip("If Enabled, it'll check if enemy's around so it doesn't waste potions."));
            itemsMenu.AddItem(
                new MenuItem("UsePotionsAtHPPercent", "Use Potions at HP Percent 'X'").SetValue(new Slider(30)));
            itemsMenu.AddItem(new MenuItem("UseSeraph", "Use [Seraph's Embrace]?").SetValue(true));
            itemsMenu.AddItem(new MenuItem("Hextech", "Use [Hextech Gunblade]?").SetValue(true));
            itemsMenu.AddItem(new MenuItem("Protobelt", "Use [Hextech Protobelt-01]?").SetValue(true));
            itemsMenu.AddItem(new MenuItem("GLP800", "Use [Hextech GLP-800]?").SetValue(true));
            itemsMenu.AddItem(
                new MenuItem("StackTear", "Stack Tear/Manamune/Archangel in Fountain?").SetValue(true)
                    .SetTooltip("Stack it in Fountain?"));
            itemsMenu.AddItem(
                new MenuItem("StackTearNF", "Stack Tear/Manamune/Archangel if You've Blue Buff?").SetValue(false));

            _menu.Item("UseSmartPotion").ValueChanged += (sender, eventArgs) =>
            {
                if (!_menu.Item("UsePotions").GetValue<bool>() && eventArgs.GetNewValue<bool>())
                    _menu.Item("UsePotions").SetValue(true);
            };

            #region Skin Changer

            var skinChangerMenu =
                _menu.AddSubMenu(new Menu(":: Skin Changer", "SkinChanger").SetFontStyle(FontStyle.Bold,
                    Color.Chartreuse));
            var skinChanger =
                skinChangerMenu.AddItem(
                    new MenuItem("UseSkinChanger", ":: Use SkinChanger?").SetValue(true)
                        .SetFontStyle(FontStyle.Bold, Color.Chartreuse));
            var skinId =
                skinChangerMenu.AddItem(
                    new MenuItem("SkinID", ":: Skin").SetValue(new Slider(10, 0, 10))
                        .SetFontStyle(FontStyle.Bold, Color.Chartreuse));
            skinId.ValueChanged += (sender, eventArgs) =>
            {
                if (!skinChanger.GetValue<bool>())
                    return;

                //Player.SetSkin(Player.BaseSkinName, eventArgs.GetNewValue<Slider>().Value);
            };

            #endregion

            var hitChanceMenu = _menu.AddSubMenu(new Menu("HitChance Menu", "HitChance"));
            hitChanceMenu.AddItem(
                new MenuItem("HitChance", "Hit Chance").SetValue(new StringList(new[] {"Medium", "High", "Very High"}, 1)));

            var ultimateMenu = _menu.AddSubMenu(new Menu("Ultimate Menu", "UltMenu"));
            //UltimateMenu.AddItem(new MenuItem("DontREnemyCount", "Don't R If Enemy In 'X' Range").SetValue(new Slider(1000, 0, 2000)));
            //UltimateMenu.AddItem(new MenuItem("DontRIfAlly", "Don't R if Ally is Near Target 'X' Range").SetValue(new Slider(700, 0, 2000)));
            //UltimateMenu.AddItem(new MenuItem("DontRUnderTurret", "Don't use R if enemy is Under Turret").SetValue(true));
            ultimateMenu.AddItem(
                new MenuItem("UseRAndZhonyaEscape", "Use R + Zhonya to Escape (Untouched)").SetValue(false)
                    .SetFontStyle(FontStyle.Bold, Color.Chartreuse));
            ultimateMenu.AddItem(
                new MenuItem("UseR", "Use R Automatically (Beta)").SetValue(new KeyBind("T".ToCharArray()[0],
                        KeyBindType.Press))
                    .SetTooltip("It'll Use the Ultimate if there's Ally turret nearby to teleport you to it"));
            //UltimateMenu.AddItem(new MenuItem("EnemiesAroundTarget", "Dont R If 'X' Enemies are around the Target").SetValue(new Slider(3, 0, 5)));

            var miscMenu = _menu.AddSubMenu(new Menu("Misc Menu", "MiscMenu"));
            miscMenu.AddItem(new MenuItem("KSQ", "Use Q to KS").SetValue(true));
            miscMenu.AddItem(new MenuItem("KSW", "Use W to KS").SetValue(true));
            miscMenu.AddItem(new MenuItem("KSE", "Use E to KS").SetValue(true));
            miscMenu.AddItem(new MenuItem("InterruptWithW", "Use W to Interrupt Channeling Spells").SetValue(true));
            miscMenu.AddItem(new MenuItem("WGapCloser", "Use W on Enemy GapCloser (Ex. Irelia's Q)").SetValue(true));
            miscMenu.AddItem(new MenuItem("ChaseWithR", "Use R to Chase (Being Added)"));
            miscMenu.AddItem(new MenuItem("EscapeWithR", "Use R to Escape (Ultimate Menu)"));
            miscMenu.AddItem(new MenuItem("Reminders", "Enable [SS AIO Type] Reminders?").SetValue(true));

            var autoLevelerMenu = _menu.AddSubMenu(new Menu("AutoLeveler Menu", "AutoLevelerMenu"));
            autoLevelerMenu.AddItem(new MenuItem("AutoLevelUp", "AutoLevel Up Spells?").SetValue(true));
            autoLevelerMenu.AddItem(
                new MenuItem("AutoLevelUp1", "First: ").SetValue(new StringList(new[] {"Q", "W", "E", "R"}, 3)));
            autoLevelerMenu.AddItem(
                new MenuItem("AutoLevelUp2", "Second: ").SetValue(new StringList(new[] {"Q", "W", "E", "R"})));
            autoLevelerMenu.AddItem(
                new MenuItem("AutoLevelUp3", "Third: ").SetValue(new StringList(new[] {"Q", "W", "E", "R"}, 2)));
            autoLevelerMenu.AddItem(
                new MenuItem("AutoLevelUp4", "Fourth: ").SetValue(new StringList(new[] {"Q", "W", "E", "R"}, 1)));
            autoLevelerMenu.AddItem(
                new MenuItem("AutoLvlStartFrom", "AutoLeveler Start from Level: ").SetValue(new Slider(2, 6, 1)));

            var drawingMenu = _menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            drawingMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range").SetValue(true));
            drawingMenu.AddItem(new MenuItem("DrawWE", "Draw W/E Range").SetValue(true));
            drawingMenu.AddItem(new MenuItem("DrawR", "Draw R Range").SetValue(false));
            drawingMenu.AddItem(new MenuItem("DrawRMinimap", "Draw R Range | On Minimap").SetValue(true));
            drawingMenu.AddItem(new MenuItem("DrawSpellFarm", "Draw Spell Farm State? [On/Off]").SetValue(true));

            #endregion

            #region DrawHPDamage

            var dmgAfterShave =
                drawingMenu.AddItem(new MenuItem("SurvivorRyze.DrawComboDamage", "Draw Combo Damage").SetValue(true));
            var drawFill =
                drawingMenu.AddItem(new MenuItem("SurvivorRyze.DrawColour", "Fill Color", true).SetValue(
                    new Circle(true, System.Drawing.Color.Chartreuse)));
            //DrawDamage.DamageToUnit = CalculateDamage;
            //DrawDamage.Enabled = dmgAfterShave.GetValue<bool>();
            //DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
            //DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;
            dmgAfterShave.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    //DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                };

            drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                //DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                //DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };

            #endregion

            _menu.AddToMainMenu();

            #region Subscriptions

            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += OnEndScene;
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Game.OnWndProc += OnWndProc;

            #endregion

            Chat.Print("<font color='#800040'>[SurvivorSeries] Ryze</font> <font color='#ff6600'>Loaded.</font>");
        }

        private static void OnWndProc(WndEventArgs args)
        {
            if (!_menu.Item("EnableMouseScroll").GetValue<bool>())
                return;

            if (args.Msg == 0x20a)
                _menu.Item("EnableFarming").SetValue(!_menu.Item("EnableFarming").GetValue<bool>());
        }

        private static void OnEndScene(EventArgs args)
        {
            switch (_r.Level)
            {
                case 1:
                    _rangeR = 1750f;
                    break;
                case 2:
                    _rangeR = 3000f;
                    break;
            }

            if (_menu.Item("DrawRMinimap").GetValue<bool>() && (_r.Level > 0) && _r.IsReady())
            {
#pragma warning disable CS0618 // Type or member is obsolete
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, _rangeR, System.Drawing.Color.DeepPink, 2, 45, true);
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            switch (_r.Level)
            {
                case 1:
                    _rangeR = 1750f;
                    break;
                case 2:
                    _rangeR = 3000f;
                    break;
            }
            if (_menu.Item("DrawQ").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, _q.Range, System.Drawing.Color.Aqua);
            if (_menu.Item("DrawWE").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, _w.Range, System.Drawing.Color.AliceBlue);
            if (_menu.Item("DrawR").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, _rangeR, System.Drawing.Color.Orchid);

            if (!_menu.Item("DrawSpellFarm").GetValue<bool>())
                return;

            if (_menu.Item("EnableFarming").GetValue<bool>())
            {
                var drawPos = Drawing.WorldToScreen(Player.Position);
                var textSize = Drawing.GetTextEntent(("Spell Farm: ON"), 15);
                Drawing.DrawText(drawPos.X - textSize.Width - 70f, drawPos.Y, System.Drawing.Color.Chartreuse,
                    "Spell Farm: ON");
            }
            else
            {
                var drawPos = Drawing.WorldToScreen(Player.Position);
                var textSize = Drawing.GetTextEntent(("Spell Farm: OFF"), 15);
                Drawing.DrawText(drawPos.X - textSize.Width - 70f, drawPos.Y, System.Drawing.Color.DeepPink,
                    "Spell Farm: OFF");
            }
        }

        private static void AaBlock()
        {
            if (_menu.Item("SmartAABlock").GetValue<bool>())
            {
                if (Player.Level >= 6)
                    _orbwalker.SetAttack(false);
            }
            else
            {
                _orbwalker.SetAttack(!_menu.Item("CBlockAA").GetValue<bool>());
            }
            //SebbyLib.OktwCommon.blockAttack = Menu.Item("CBlockAA").GetValue<bool>();
        }

        private static void GotStronger()
        {
            // Reminder
            Chat.Print(
                "<font color='#0993F9'>[SS Ryze | Reminder]</font> <font color='#FF8800'>You got strong enough, Lower the LaneClear Mana Manager Sliders!</font>");
        }

        private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (sender.IsMe && _menu.Item("Reminders").GetValue<bool>() && (ObjectManager.Player.Level == 6))
                GotStronger();

            if (!sender.IsMe || !_menu.Item("AutoLevelUp").GetValue<bool>() ||
                (ObjectManager.Player.Level < _menu.Item("AutoLvlStartFrom").GetValue<Slider>().Value))
                return;
            if ((_lvl2 == _lvl3) || (_lvl2 == _lvl4) || (_lvl3 == _lvl4))
                return;
            var delay = 700;
            LeagueSharp.Common.Utility.DelayAction.Add(delay, () => LevelUp(_lvl1));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 50, () => LevelUp(_lvl2));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 100, () => LevelUp(_lvl3));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 150, () => LevelUp(_lvl4));
        }

        private static void LevelUp(int indx)
        {
            if (ObjectManager.Player.Level < 4)
            {
                if ((indx == 0) && (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0))
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if ((indx == 1) && (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0))
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if ((indx == 2) && (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0))
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
            }
            else
            {
                if (indx == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (indx == 1)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (indx == 2)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (indx == 3)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
            }
        }

        private static void PotionsCheck()
        {
            if (_menu.Item("UseSmartPotion").GetValue<bool>())
                if (Player.CountEnemiesInRange(800) == 0)
                    return;

            if (Player.HasBuff("RegenerationPotion") || Player.HasBuff("ItemMiniRegenPotion") ||
                Player.HasBuff("ItemCrystalFlaskJungle") || Player.HasBuff("ItemDarkCrystalFlask") ||
                Player.HasBuff("ItemCrystalFlask"))
                return;

            if (Player.HealthPercent <= _menu.Item("UsePotionsAtHPPercent").GetValue<Slider>().Value)
                if (HpPotion.IsReady())
                    HpPotion.Cast();
                else if (Biscuit.IsReady())
                    Biscuit.Cast();
                else if (FlaskHunterJg.IsReady())
                    FlaskHunterJg.Cast();
                else if (FlaskCorruptJg.IsReady())
                    FlaskCorruptJg.Cast();
                else if (FlaskRef.IsReady())
                    FlaskRef.Cast();
        }

        private static void SeraphUsage()
        {
            var incomingdmg = OktwCommon.GetIncomingDamage(Player, 1);
            if (Seraph.IsReady() && _menu.Item("UseSeraph").GetValue<bool>())
            {
                var shieldint = Player.Mana*0.2 + 150;
                if ((incomingdmg > Player.Health) && (incomingdmg < Player.Health + shieldint))
                    Seraph.Cast();
            }
        }

        private static void ItemsChecks()
        {
            if (Glp800.IsReady())
            {
                var t = TargetSelector.GetTarget(Glp800.Range, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget())
                    if (_menu.Item("GLP800").GetValue<bool>() &&
                        (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo))
                        Glp800.Cast(Prediction.GetPrediction(t, 0.5f).CastPosition);
            }

            if (Protobelt.IsReady())
            {
                var t = TargetSelector.GetTarget(Protobelt.Range, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget())
                    if (_menu.Item("Protobelt").GetValue<bool>() &&
                        (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo))
                        Protobelt.Cast(Prediction.GetPrediction(t, 0.5f).CastPosition);
            }

            if (Hextech.IsReady())
            {
                var t = TargetSelector.GetTarget(Hextech.Range, TargetSelector.DamageType.Magical);
                if (t.IsValidTarget())
                    if (_menu.Item("Hextech").GetValue<bool>() &&
                        (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo))
                        Hextech.Cast(t);
            }
        }

        private static void StackItems()
        {
            if (Player.InFountain() ||
                (Player.HasBuff("CrestoftheAncientGolem") && (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None) &&
                 _menu.Item("StackTearNF").GetValue<bool>())) // Add if Player has Blue Buff
                if (Items.HasItem(3004, Player) || Items.HasItem(3003, Player) || Items.HasItem(3070, Player) ||
                    Items.HasItem(3072, Player) || Items.HasItem(3073, Player) || Items.HasItem(3008, Player))
                    _q.Cast(Player.ServerPosition);
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
                return;

            if (_menu.Item("StackTear").GetValue<bool>())
                StackItems();
            SeraphUsage();
            ItemsChecks();
            KsCheck();
            PotionsCheck();
            switch (_orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    AaBlock();
                    Combo();
                    if (_menu.Item("UseComboPlus").GetValue<bool>())
                        ComboPlusCheck(); // Beta - Will be improved
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    JungleClear();
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    _orbwalker.SetMovement(true);
                    _orbwalker.SetAttack(true);
                    RZhonya();
                    break;
            }
            if (_menu.Item("UseR").GetValue<KeyBind>().Active)
            {
                _orbwalker.SetMovement(false);
                _orbwalker.SetAttack(false);
                REscape();
            }
            //AutoLeveler
            if (_menu.Item("AutoLevelUp").GetValue<bool>())
            {
                _lvl1 = _menu.Item("AutoLevelUp1").GetValue<StringList>().SelectedIndex;
                _lvl2 = _menu.Item("AutoLevelUp2").GetValue<StringList>().SelectedIndex;
                _lvl3 = _menu.Item("AutoLevelUp3").GetValue<StringList>().SelectedIndex;
                _lvl4 = _menu.Item("AutoLevelUp4").GetValue<StringList>().SelectedIndex;
            }
        }

        private static void JungleClear()
        {
            if (Player.ManaPercent < _menu.Item("JungleClearManaManager").GetValue<Slider>().Value)
                return;

            var jgcq = _menu.Item("UseQJC").GetValue<bool>();
            var jgcw = _menu.Item("UseWJC").GetValue<bool>();
            var jgce = _menu.Item("UseEJC").GetValue<bool>();

            var mob =
                MinionManager.GetMinions(Player.ServerPosition, _q.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (mob == null)
                return;
            if (jgcq && jgce && _q.IsReady() && _e.IsReady())
            {
                _q.Cast(mob.Position);
                _e.CastOnUnit(mob);
                _q.Cast(mob.Position);
                if (jgcw && _w.IsReady() && !_q.IsReady())
                {
                    _w.CastOnUnit(mob);
                    _q.Cast(mob.Position);
                }
            }
            else if (jgcq && jgce && !_q.IsReady() && _e.IsReady())
            {
                _e.CastOnUnit(mob);
                _q.Cast(mob.Position);
                if (jgcw && _w.IsReady() && !_q.IsReady())
                {
                    _w.CastOnUnit(mob);
                    _q.Cast(mob.Position);
                }
            }
            else if (jgcq && jgce && jgcw && !_q.IsReady() && !_e.IsReady() && _w.IsReady())
            {
                _w.CastOnUnit(mob);
                _q.Cast(mob.Position);
                if (_e.IsReady())
                {
                    _e.CastOnUnit(mob);
                    _q.Cast(mob.Position);
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!_menu.Item("WGapCloser").GetValue<bool>() ||
                (Player.Mana < _w.Instance.SData.Mana + _q.Instance.SData.Mana))
                return;

            var t = gapcloser.Sender;

            if (gapcloser.End.Distance(Player.ServerPosition) < _w.Range)
                _w.Cast(t);
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient t,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            var wCast = _menu.Item("InterruptWithW").GetValue<bool>();
            if (!wCast || !t.IsValidTarget(_w.Range) || !_w.IsReady()) return;
            _w.Cast(t);
        }

        private static void KsCheck()
        {
            var target = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Magical);

            // If Target's not in Q Range or there's no target or target's invulnerable don't fuck with him
            if ((target == null) || !target.IsValidTarget(_q.Range) || target.IsInvulnerable)
                return;

            var ksQ = _menu.Item("KSQ").GetValue<bool>();
            var ksW = _menu.Item("KSW").GetValue<bool>();
            var ksE = _menu.Item("KSE").GetValue<bool>();

            // KS
            if (ksQ && (OktwCommon.GetKsDamage(target, _q) > target.Health) && target.IsValidTarget(_q.Range))
                SebbySpell(_q, target);
            if (ksW && (OktwCommon.GetKsDamage(target, _w) > target.Health) && target.IsValidTarget(_w.Range))
                _w.CastOnUnit(target);
            if (ksE && (OktwCommon.GetKsDamage(target, _e) > target.Health) && target.IsValidTarget(_e.Range))
                _e.CastOnUnit(target);
        }

        public static bool RyzeCharge0()
        {
            return Player.HasBuff("ryzeqiconnocharge");
        }

        public static bool RyzeCharge1()
        {
            return Player.HasBuff("ryzeqiconhalfcharge");
        }

        public static bool RyzeCharge2()
        {
            return Player.HasBuff("ryzeqiconfullcharge");
        }

        private static void SebbySpell(Spell qr, Obj_AI_Base target)
        {
            var coreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
            var aoe2 = false;

            if (qr.Type == SkillshotType.SkillshotCircle)
            {
                coreType2 = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
                aoe2 = true;
            }

            if ((qr.Width > 80) && !qr.Collision)
                aoe2 = true;

            var predInput2 = new PredictionInput
            {
                Aoe = aoe2,
                Collision = qr.Collision,
                Speed = qr.Speed,
                Delay = qr.Delay,
                Range = qr.Range,
                From = Player.ServerPosition,
                Radius = qr.Width,
                Unit = target,
                Type = coreType2
            };
            var poutput2 = SebbyLib.Prediction.Prediction.GetPrediction(predInput2);

            if (OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                return;

            if (_menu.Item("HitChance").GetValue<StringList>().SelectedIndex == 0)
            {
                if (poutput2.Hitchance >= HitChance.Medium)
                    qr.Cast(poutput2.CastPosition);
            }
            else if (_menu.Item("HitChance").GetValue<StringList>().SelectedIndex == 1)
            {
                if (poutput2.Hitchance >= HitChance.High)
                    qr.Cast(poutput2.CastPosition);
            }
            else if (_menu.Item("HitChance").GetValue<StringList>().SelectedIndex == 2)
            {
                if (poutput2.Hitchance >= HitChance.VeryHigh)
                    qr.Cast(poutput2.CastPosition);
            }
        }

        private static float QGetRealDamage(Obj_AI_Base target)
        {
            if (!target.HasBuff("RyzeE"))
                return QDefaultDamage(target);
            if (((_e.IsReady() && !_q.IsReady()) || (_e.IsReady() && _q.IsReady()) || (!_e.IsReady() && _q.IsReady())) &&
                target.HasBuff("RyzeE"))
            {
                switch (_e.Level)
                {
                    case 1:
                        _qRealDamage = QDefaultDamage(target)/40*100;
                        break;
                    case 2:
                        _qRealDamage = QDefaultDamage(target)/55*100;
                        break;
                    case 3:
                        _qRealDamage = QDefaultDamage(target)/70*100;
                        break;
                    case 4:
                        _qRealDamage = QDefaultDamage(target)/85*100;
                        break;
                    case 5:
                        _qRealDamage = QDefaultDamage(target)/100*100;
                        break;
                }
                //Chat.Print("Inside V2 qRealDamage:" + QRealDamage);
                return _qRealDamage;
            }
            //Chat.Print("Inside else at end:" + QDefaultDamage(target));
            return QDefaultDamage(target);
        }

        private static void ComboPlusCheck()
        {
            // Combo
            var cUseQ = _menu.Item("CUseQ").GetValue<bool>();
            //var CUseW = Menu.Item("CUseW").GetValue<bool>();
            var cUseE = _menu.Item("CUseE").GetValue<bool>();
            // Checks
            var target = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Magical);

            // If Target's not in Q Range or there's no target or target's invulnerable don't fuck with him
            if ((target == null) || !target.IsValidTarget(_q.Range) || target.IsInvulnerable)
                return;

            var ryzeebuffed =
                MinionManager.GetMinions(Player.Position, _q.Range)
                    .Find(x => x.HasBuff("RyzeE") && x.IsValidTarget(_q.Range));
            var noebuffed =
                MinionManager.GetMinions(Player.Position, _q.Range)
                    .Find(x => x.IsValidTarget(_q.Range) && (x.Distance(target) < 200));

            if (cUseQ && cUseE && target.IsValidTarget(_q.Range))
                if ((ryzeebuffed != null) && ryzeebuffed.IsValidTarget(_q.Range))
                {
                    if (ryzeebuffed.Health < QGetRealDamage(ryzeebuffed))
                    {
                        //Chat.Print("<font color='#9400D3'>DEBUG: Spread</font>");
                        if (!_q.IsReady() && _e.IsReady())
                        {
                            _e.CastOnUnit(ryzeebuffed);
                            _q.Cast(ryzeebuffed);
                            //Chat.Print("<font color='#9400D3'>DEBUG: Spreading [Reset with E]</font>");
                        }
                        _q.Cast(ryzeebuffed);
                    }
                    if (target.HasBuff("RyzeE") && (target.Distance(ryzeebuffed) < 200) &&
                        ryzeebuffed.IsValidTarget(_q.Range))
                    {
                        //Chat.Print("<font color='#9400D3'>DEBUG: Got to Part 1</font>");
                        _q.Cast(ryzeebuffed);
                    }
                    else if (!target.HasBuff("RyzeE"))
                    {
                        _e.CastOnUnit(target);
                        if (target.Distance(ryzeebuffed) < 200)
                            _q.Cast(ryzeebuffed);
                    }
                }
                else if ((ryzeebuffed == null) || !ryzeebuffed.IsValidTarget())
                {
                    if ((noebuffed != null) && noebuffed.IsValidTarget(_e.Range) &&
                        (noebuffed.Health < QGetRealDamage(noebuffed)))
                        if (_e.IsReady())
                        {
                            _e.CastOnUnit(noebuffed);
                            if (_q.IsReady())
                                _q.Cast(noebuffed);
                            //Chat.Print("<font color='#9400D3'>DEBUG: Spreading [Reset with E]</font>");
                        }
                }
        }

        private static float QDefaultDamage(Obj_AI_Base target)
        {
            var damage = Player.CalcDamage(target, Damage.DamageType.Magical,
                new[] {60, 85, 110, 135, 160, 185}[Player.GetSpell(SpellSlot.Q).Level - 1] +
                Player.TotalMagicalDamage/45*100 + Player.Mana/3*100);
            return (float) damage;
        }

        private static void Combo()
        {
            // Combo
            var cUseQ = _menu.Item("CUseQ").GetValue<bool>();
            var cUseW = _menu.Item("CUseW").GetValue<bool>();
            var cUseE = _menu.Item("CUseE").GetValue<bool>();
            // Checks
            var target = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Magical);

            // If Target's not in Q Range or there's no target or target's invulnerable don't fuck with him
            if ((target == null) || !target.IsValidTarget(_q.Range) || target.IsInvulnerable)
                return;
            switch (_menu.Item("ComboMode").GetValue<StringList>().SelectedIndex)
            {
                case 0:

                    #region Burst Mode

                    // Execute the Lad
                    if (_menu.Item("CUseIgnite").GetValue<bool>() &&
                        (target.Health <
                         OktwCommon.GetIncomingDamage(target) +
                         Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)))
                        ObjectManager.Player.Spellbook.CastSpell(_igniteSlot, target);
                    if (Player.Mana >= _q.Instance.SData.Mana + _w.Instance.SData.Mana + _e.Instance.SData.Mana)
                    {
                        if (cUseW && target.IsValidTarget(_w.Range) && _w.IsReady())
                            _w.CastOnUnit(target);
                        if (cUseQ && target.IsValidTarget(_q.Range) && _q.IsReady())
                            SebbySpell(_q, target);
                        if (cUseE && target.IsValidTarget(_e.Range) && _e.IsReady())
                            _e.CastOnUnit(target);
                    }
                    else
                    {
                        if (cUseW && target.IsValidTarget(_w.Range) && _w.IsReady())
                            _w.CastOnUnit(target);
                        if (cUseQ && target.IsValidTarget(_q.Range) && _q.IsReady())
                            SebbySpell(_q, target);
                        if (cUseE && target.IsValidTarget(_e.Range) && _e.IsReady())
                            _e.CastOnUnit(target);
                    }

                    #endregion

                    break;
                case 1:

                    #region SurvivorMode

                    if ((_q.Level >= 1) && (_w.Level >= 1) && (_e.Level >= 1))
                    {
                        if (!target.IsValidTarget(_w.Range - 15f) && _q.IsReady())
                            SebbySpell(_q, target);
                        // Try having Full Charge if either W or E spells are ready...
                        if (RyzeCharge1() && _q.IsReady() && (_w.IsReady() || _e.IsReady()))
                        {
                            if (_e.IsReady() && cUseE)
                                _e.Cast(target);
                            if (_w.IsReady() && cUseW)
                                _w.Cast(target);
                        }
                        // Rest in Piece XDDD
                        if (RyzeCharge1() && !_e.IsReady() && !_w.IsReady() && cUseQ)
                            SebbySpell(_q, target);

                        if (RyzeCharge0() && !_e.IsReady() && !_w.IsReady() && cUseQ)
                            SebbySpell(_q, target);

                        if (!RyzeCharge2())
                        {
                            _e.Cast(target);
                            _w.Cast(target);
                        }
                        else
                        {
                            SebbySpell(_q, target);
                        }
                    }
                    else
                    {
                        if (target.IsValidTarget(_q.Range) && _q.IsReady() && cUseQ)
                            SebbySpell(_q, target);

                        if (target.IsValidTarget(_w.Range) && _w.IsReady() && cUseW)
                            _w.Cast(target);

                        if (target.IsValidTarget(_e.Range) && _e.IsReady() && cUseE)
                            _e.Cast(target);
                    }

                    #endregion

                    break;
                case 2:
                {
                    #region As Fast As Possible (SPAM)

                    if (_menu.Item("CUseIgnite").GetValue<bool>() &&
                        (target.Health <
                         OktwCommon.GetIncomingDamage(target) +
                         Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)))
                        ObjectManager.Player.Spellbook.CastSpell(_igniteSlot, target);
                    if (target.IsValidTarget(_q.Range) && _q.IsReady() && cUseQ)
                        SebbySpell(_q, target);
                    if (target.IsValidTarget(_w.Range) && _w.IsReady() && cUseW)
                        _w.CastOnUnit(target);
                    if (target.IsValidTarget(_e.Range) && _e.IsReady() && cUseE)
                        _e.CastOnUnit(target);

                    #endregion
                }
                    break;
            }
        }

        private static void Harass()
        {
            // Harass
            var harassUseQ = _menu.Item("HarassQ").GetValue<bool>();
            var harassUseW = _menu.Item("HarassW").GetValue<bool>();
            var harassUseE = _menu.Item("HarassE").GetValue<bool>();
            // Checks
            var target = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Magical);
            var ryzeebuffed =
                MinionManager.GetMinions(Player.Position, _e.Range)
                    .Find(x => x.HasBuff("RyzeE") && x.IsValidTarget(_e.Range));
            // If Target's not in Q Range or there's no target or target's invulnerable don't fuck with him
            if ((target == null) || !target.IsValidTarget(_q.Range) || target.IsInvulnerable)
                return;

            // Execute the Lad
            if (Player.ManaPercent > _menu.Item("HarassManaManager").GetValue<Slider>().Value)
            {
                if (harassUseW && target.IsValidTarget(_w.Range))
                    _w.CastOnUnit(target);
                if (harassUseQ && target.IsValidTarget(_q.Range))
                    SebbySpell(_q, target);
                if (harassUseE && ryzeebuffed.IsValidTarget() && (target.Distance(ryzeebuffed) < 200))
                    _e.CastOnUnit(ryzeebuffed);
                else if (harassUseE && (!ryzeebuffed.IsValidTarget() || (ryzeebuffed == null)) &&
                         target.IsValidTarget(_w.Range))
                    _e.CastOnUnit(target);
            }
        }

        private static void LastHit()
        {
            var useQ = _menu.Item("UseQLH").GetValue<bool>();
            var useE = _menu.Item("UseELH").GetValue<bool>();
            // To be Done
            if (Player.ManaPercent > _menu.Item("LaneHitManaManager").GetValue<Slider>().Value)
            {
                var specialQ =
                    Cache.GetMinions(Player.Position, _q.Range)
                        .OrderBy(x => x.Distance(Player.Position));
                if (_q.IsReady() && useQ)
                    foreach (var omfgabriel in specialQ)
                        if (omfgabriel.Health < QGetRealDamage(omfgabriel))
                            _q.Cast(omfgabriel);
                var allMinionsQ = Cache.GetMinions(Player.Position, _q.Range);
                var allMinionsE = Cache.GetMinions(Player.Position, _e.Range);
                if (_q.IsReady() && useQ)
                {
                    if (allMinionsQ.Count > 0)
                        foreach (var minion in allMinionsQ)
                        {
                            if (!minion.IsValidTarget() || (minion == null))
                                return;
                            if (minion.Health < QGetRealDamage(minion))
                                _q.Cast(minion);
                        }
                }
                else if (_e.IsReady() && useE)
                {
                    if (allMinionsE.Count > 0)
                        foreach (var minion in allMinionsE)
                        {
                            if (!minion.IsValidTarget() || (minion == null))
                                return;
                            if (minion.Health < _e.GetDamage(minion))
                                _e.CastOnUnit(minion);
                        }
                }
            }
        }

        private static void LaneClear()
        {
            if (!_menu.Item("EnableFarming").GetValue<bool>())
                return;

            // LaneClear | Notes: Rework on early levels not using that much abilities since Spell Damage is lower, higher Lvl is fine
            if (!_menu.Item("UseQLC").GetValue<bool>() && !_menu.Item("UseELC").GetValue<bool>()) return;
            if (!(Player.ManaPercent > _menu.Item("LaneClearManaManager").GetValue<Slider>().Value)) return;
            var ryzeebuffed =
                Cache.GetMinions(Player.Position, _q.Range)
                    .Find(x => x.HasBuff("RyzeE") && x.IsValidTarget(_q.Range));
            var ryzenotebuffed =
                Cache.GetMinions(Player.Position, _q.Range)
                    .Find(x => !x.HasBuff("RyzeE") && x.IsValidTarget(_q.Range));
            var allMinionsQ = Cache.GetMinions(Player.Position, _q.Range);
            var allMinions = Cache.GetMinions(Player.Position, _e.Range);
            if (_q.IsReady() && !_e.IsReady())
                if (allMinionsQ.Count > 0)
                    foreach (var minion in allMinionsQ)
                    {
                        if (!minion.IsValidTarget() || (minion == null))
                            return;
                        if (minion.Health < QGetRealDamage(minion))
                            _q.Cast(minion);
                    }
            if (!_q.IsReady() && (_q.Level > 0) && _e.IsReady())
                if (ryzeebuffed != null)
                {
                    if ((ryzeebuffed.Health + 15 < _e.GetDamage(ryzeebuffed) + QGetRealDamage(ryzeebuffed)) &&
                        ryzeebuffed.IsValidTarget(_e.Range))
                    {
                        _e.CastOnUnit(ryzeebuffed);
                        if (_q.IsReady())
                            _q.Cast(ryzeebuffed);

                        _orbwalker.ForceTarget(ryzeebuffed);
                    }
                }
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                else if (ryzeebuffed == null)
                {
                    foreach (var minion in allMinions)
                        if (minion.IsValidTarget(_e.Range) &&
                            (minion.Health + 15 < _e.GetDamage(minion) + QGetRealDamage(minion)))
                        {
                            _e.CastOnUnit(minion);
                            if (_q.IsReady())
                                // ReSharper disable once ExpressionIsAlwaysNull
                                _q.Cast(ryzeebuffed);
                        }
                }
            if (!_q.IsReady() || !_e.IsReady()) return;
            if (ryzeebuffed != null)
            {
                if (!(ryzeebuffed.Health <
                      QDefaultDamage(ryzeebuffed) + _e.GetDamage(ryzeebuffed) + QDefaultDamage(ryzeebuffed)) ||
                    !ryzeebuffed.IsValidTarget(_e.Range)) return;
                _q.Cast(ryzeebuffed);
                if (ryzeebuffed.IsValidTarget(_e.Range))
                    _e.CastOnUnit(ryzeebuffed);
                if (!_e.IsReady() && _q.IsReady())
                    _q.Cast(ryzeebuffed);
            }
            else
            {
                if (!(ryzenotebuffed?.Health <
                      QDefaultDamage(ryzenotebuffed) + _e.GetDamage(ryzenotebuffed) +
                      QDefaultDamage(ryzenotebuffed)) || !ryzenotebuffed.IsValidTarget(_e.Range)) return;
                _q.Cast(ryzenotebuffed);
                if (ryzenotebuffed.IsValidTarget(_e.Range))
                {
                    _orbwalker.ForceTarget(ryzenotebuffed);
                    _e.CastOnUnit(ryzenotebuffed);
                }
                if (!_e.IsReady() && _q.IsReady())
                    _q.Cast(ryzenotebuffed);
            }
        } // LaneClear End

        private static void RZhonya()
        {
            if ((Player.HealthPercent < 50) && _menu.Item("UseRAndZhonyaEscape").GetValue<bool>() &&
                _r.Instance.IsReady() &&
                Zhonya.IsOwned(Player))
                if (Zhonya.IsReady())
                {
                    switch (_r.Level)
                    {
                        case 1:
                            _rangeR = 1750f;
                            break;
                        case 2:
                            _rangeR = 3000f;
                            break;
                    }
                    if (GrabDefensePosition() == Player.ServerPosition)
                        return;
                    _r.Cast(GrabDefensePosition());
                    LeagueSharp.Common.Utility.DelayAction.Add(145, delegate
                    {
                        if (Player.HasBuff("RyzeRChannel")) Zhonya.Cast();
                    });
                }
        }

        private static Vector3 GrabDefensePosition()
        {
            switch (_r.Level)
            {
                case 1:
                    _rangeR = 1750f;
                    break;
                case 2:
                    _rangeR = 3000f;
                    break;
            }
            var nearTurret = ObjectManager.Get<Obj_AI_Turret>()
                .FirstOrDefault(turret => turret.IsAlly && (turret.Distance(Player.Position) <= _rangeR));

            var nearAlly =
                HeroManager.Allies.FirstOrDefault(
                    ally => (ally.CountEnemiesInRange(1500) <= 0) && (ally.Distance(Player) <= _rangeR));
            if (nearTurret != null)
                return nearTurret.ServerPosition;
            if (nearAlly != null)
                return nearAlly.ServerPosition;
            return Player.ServerPosition;
        }

        private static void REscape()
        {
            switch (_r.Level)
            {
                case 1:
                    _rangeR = 1750f;
                    break;
                case 2:
                    _rangeR = 3000f;
                    break;
            }
            var nearByTurrets =
                ObjectManager.Get<Obj_AI_Turret>().Find(turret => (turret.Distance(Player) < _rangeR) && turret.IsAlly);
            if (nearByTurrets != null)
                _r.Cast(nearByTurrets.Position);
        }

        //RUsage

        private static float CalculateDamage(Obj_AI_Base enemy)
        {
            float damage = 0;
            if (_q.IsReady() || (Player.Mana <= _q.Instance.SData.Mana + _e.Instance.SData.Mana))
                damage += QGetRealDamage(enemy);
            else if (_q.IsReady() || (Player.Mana <= _q.Instance.SData.Mana))
                damage += QDefaultDamage(enemy);

            if (_w.IsReady() || (Player.Mana <= _w.Instance.SData.Mana + _w.Instance.SData.Mana))
                damage += _w.GetDamage(enemy) + _w.GetDamage(enemy);
            else if (_w.IsReady() || (Player.Mana <= _w.Instance.SData.Mana))
                damage += _w.GetDamage(enemy);

            if (_e.IsReady() || (Player.Mana <= _e.Instance.SData.Mana + _e.Instance.SData.Mana))
                damage += _e.GetDamage(enemy) + _e.GetDamage(enemy);
            else if (_e.IsReady() || (Player.Mana <= _e.Instance.SData.Mana))
                damage += _e.GetDamage(enemy);

            if (_menu.Item("CUseIgnite").GetValue<bool>())
                damage += (float) Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);

            return damage;
        }

        #region Declaration

        private static Spell _q, _w, _e, _r;
        private static SpellSlot _igniteSlot;

        #region Items

        public static Items.Item
            HpPotion = new Items.Item(2003),
            Flask = new Items.Item(2041),
            Biscuit = new Items.Item(2010),
            FlaskRef = new Items.Item(2031),
            FlaskHunterJg = new Items.Item(2032),
            FlaskCorruptJg = new Items.Item(2033),
            Protobelt = new Items.Item(3152, 850f),
            Glp800 = new Items.Item(3030, 800f),
            Hextech = new Items.Item(3146, 700f),
            Seraph = new Items.Item(3040),
            Zhonya = new Items.Item(3157);

        #endregion

        private static Orbwalking.Orbwalker _orbwalker;
        private static Menu _menu;

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private const string ChampionName = "Ryze";
        private static int _lvl1, _lvl2, _lvl3, _lvl4;
        private static float _rangeR;
        private static float _qRealDamage;

        #endregion
    }
}