// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="SSRumble">
//      Copyright (c) SSRumble. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using SharpDX;
using SPrediction;
using Color = SharpDX.Color;
using HitChance = SebbyLib.Prediction.HitChance;
using Orbwalking = LeagueSharp.Common.Orbwalking;
using Prediction = SPrediction.Prediction;
using PredictionInput = SebbyLib.Prediction.PredictionInput;

using EloBuddy; 
using LeagueSharp.Common; 
namespace SSRumble
{
    internal class SSRumbleInit
    {
        protected static Spell Q, W, E, R;
        protected static Orbwalking.Orbwalker Orbwalker;
        protected static Menu Config;
        protected static Bubba BubbaFat;
        private static bool SPredictionLoaded;

        public static string[] HighChamps =
        {
            "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
            "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "Leblanc",
            "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon",
            "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath",
            "Zed", "Ziggs", "Kindred", "Jhin"
        };

        public SSRumbleInit()
        {
            GameOnGameLoad(new EventArgs());
        }

        protected static AIHeroClient Player => ObjectManager.Player;

        public Items.Item GLP800 { get; private set; }
        public Items.Item Protobelt { get; private set; }

        public static void Main()
        {
            var SSRumbleInit = new SSRumbleInit();
        }

        private void GameOnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Rumble")
                return;

            #region Spells && Items

            Q = new Spell(SpellSlot.Q, 600f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 850f);
            R = new Spell(SpellSlot.R, 1700f);
            Q.SetSkillshot(0.5f, 120f, 1300f, false, SkillshotType.SkillshotCone);
            E.SetSkillshot(0.5f, 50f, 2000f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 200f, 1500f, false, SkillshotType.SkillshotLine);
            GLP800 = new Items.Item(3030, 800f);
            Protobelt = new Items.Item(3152, 850f);

            #endregion

            #region Config

            Config = new Menu("SurvivorRumble", "SurvivorRumble", true).SetFontStyle(FontStyle.Bold, Color.Chartreuse);

            var OrbwalkerMenu = Config.AddSubMenu(new Menu(":: Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(OrbwalkerMenu);

            var TargetSelectorMenu = Config.AddSubMenu(new Menu(":: Target Selector", "TargetSelector"));

            TargetSelector.AddToMenu(TargetSelectorMenu);

            #endregion

            #region Config Items

            var ComboMenu = Config.AddSubMenu(new Menu(":: Combo", "Combo"));
            ComboMenu.AddItem(new MenuItem("ComboUseQ", "Use Q").SetValue(true));
            ComboMenu.AddItem(new MenuItem("ComboUseW", "Use W").SetValue(true));
            ComboMenu.AddItem(new MenuItem("ComboUseE", "Use E").SetValue(true));
            ComboMenu.AddItem(new MenuItem("ComboUseRSolo", "Use R on 1vs1").SetValue(true));
            ComboMenu.AddItem(new MenuItem("ComboUseItems", "Use Items?").SetValue(true));
            //ComboMenu.AddItem(
            //    new MenuItem("UseSmartCastingADC", "Use R Only if it'll land first on ADC?").SetValue(false));
            ComboMenu.AddItem(new MenuItem("ComboCastUltimate", "[Insta] Cast Ultimate Key"))
                .SetValue(new KeyBind('T', KeyBindType.Press)).Permashow(true, "[Insta Ult Active?]");
            ComboMenu.AddItem(
                new MenuItem("SemiManualR", "Semi-Manual R Casting?").SetValue(true)
                    .SetTooltip("True - Script will Auto R | False - You will R when you decide - preferably",
                        Color.Chartreuse));
            //ComboMenu.AddItem(
            //    new MenuItem("ComboMinimumRTargets", "Minimum Enemies to hit before casting Ultimate?").SetValue(
            //        new Slider(2, 1, HeroManager.Enemies.Count)));

            var LaneClearMenu = Config.AddSubMenu(new Menu(":: LaneClear", "LaneClear"));
            LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q").SetValue(true));
            LaneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E").SetValue(false));
            LaneClearMenu.AddItem(
                new MenuItem("LaneClearManaManager", "LaneClear Mana Manager").SetValue(new Slider(0, 0, 100)));
            LaneClearMenu.AddItem(
                new MenuItem("MinimumQMinions", "Minimum Minions Near You To Use Q?").SetValue(new Slider(2, 1, 10)));

            var JungleClearMenu = Config.AddSubMenu(new Menu(":: JungleClear", "JungleClear"));
            JungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q").SetValue(true));
            JungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E").SetValue(true));

            var LastHitMenu = Config.AddSubMenu(new Menu(":: LastHit", "LastHit"));
            LastHitMenu.AddItem(new MenuItem("LastHitE", "Use E").SetValue(true));
            LastHitMenu.AddItem(
                new MenuItem("LastHitManaManager", "LastHit Mana Manager").SetValue(new Slider(0, 0, 100)));

            var HarassMenu = Config.AddSubMenu(new Menu(":: Harass", "Harass"));
            HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q").SetValue(true));
            HarassMenu.AddItem(new MenuItem("HarassE", "Use E").SetValue(true));
            HarassMenu.AddItem(new MenuItem("HarassItems", "Use Items (GLP/Protobelt)").SetValue(true));
            HarassMenu.AddItem(
                new MenuItem("HarassManaManager", "Harass Mana Manager").SetValue(new Slider(0, 0, 100)));

            var KillStealMenu = Config.AddSubMenu(new Menu(":: Killsteal", "Killsteal"));
            KillStealMenu.AddItem(new MenuItem("EnableKS", "Enable Killsteal?").SetValue(true));
            KillStealMenu.AddItem(new MenuItem("KSQ", "KS with Q?").SetValue(true));
            KillStealMenu.AddItem(new MenuItem("KSE", "KS with E?").SetValue(true));
            //KillStealMenu.AddItem(new MenuItem("KSR", "KS with R?").SetValue(true)); // Later
            KillStealMenu.AddItem(new MenuItem("KSItems", "KS with Items?").SetValue(true));

            var DrawingMenu = Config.AddSubMenu(new Menu(":: Drawings", "Drawings"));
            DrawingMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawE", "Draw E Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawR", "Draw R Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawRCast", "Draw R Cast Location").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("drawKickPos", "Ultimate Cast Position"))
                .SetValue(new Circle(true, System.Drawing.Color.DeepPink));
            DrawingMenu.AddItem(new MenuItem("drawKickLine", "Ultimate Line Direction"))
                .SetValue(new Circle(true, System.Drawing.Color.Chartreuse));
            DrawingMenu.AddItem(new MenuItem("drawRTarget", "Desired Target"))
                .SetValue(new Circle(true, System.Drawing.Color.LimeGreen));

            #region Skin Changer

            /*var SkinChangerMenu =
                Config.AddSubMenu(new Menu(":: Skin Changer", "SkinChanger").SetFontStyle(FontStyle.Bold,
                    Color.Chartreuse));
            var SkinChanger =
                SkinChangerMenu.AddItem(
                    new MenuItem("UseSkinChanger", ":: Use SkinChanger?").SetValue(true)
                        .SetFontStyle(FontStyle.Bold, Color.Crimson));
            var SkinID =
                SkinChangerMenu.AddItem(
                    new MenuItem("SkinID", ":: Skin").SetValue(new StringList(new[] {"Classic", "Candy King Rumble"}, 0))
                        .SetFontStyle(FontStyle.Bold, Color.Crimson));
            SkinID.ValueChanged += (sender, eventArgs) =>
            {
                if (!SkinChanger.GetValue<bool>())
                    return;

                //Player.SetSkin(Player.BaseSkinName, eventArgs.GetNewValue<StringList>().SelectedIndex);
            };*/

            #endregion

            var MiscMenu = Config.AddSubMenu(new Menu(":: Settings", "Settings"));
            MiscMenu.AddItem(
                new MenuItem("HitChance", "Hit Chance").SetValue(new StringList(new[] {"Medium", "High", "Very High"}, 1)));
            var PredictionVar = MiscMenu.AddItem(
                new MenuItem("Prediction", "Prediction:").SetValue(new StringList(
                    new[] {"Common", "OKTW", "SPrediction"}, 1)));
            if (PredictionVar.GetValue<StringList>().SelectedIndex == 2)
                if (!SPredictionLoaded)
                {
                    Prediction.Initialize(MiscMenu, "SPrediction Settings");
                    var SPreditctionLoaded =
                        MiscMenu.AddItem(new MenuItem("SPredictionLoaded", "SPrediction Loaded!"));
                    SPredictionLoaded = true;
                }
            PredictionVar.ValueChanged += (sender, eventArgs) =>
            {
                if (eventArgs.GetNewValue<StringList>().SelectedIndex == 2)
                    if (!SPredictionLoaded)
                    {
                        Prediction.Initialize(MiscMenu, "SPrediction Settings");
                        var SPreditctionLoaded =
                            MiscMenu.AddItem(new MenuItem("SPredictionLoaded", "SPrediction Loaded!"));
                        Chat.Print(
                            "<font color='#0993F9'>[SS Rumble Warning]</font> <font color='#FF8800'>Please exit the menu and click back on it again, to see the settings or Reload (F5)</font>");

                        SPredictionLoaded = true;
                    }
            };
            MiscMenu.AddItem(new MenuItem("UseWNearbyEnemy", "[Auto] Use (W) Nearby Enemies").SetValue(false));
            MiscMenu.AddItem(new MenuItem("EnableMouseScroll", "Enable Mouse Scroll to Store Heat?").SetValue(true));
            MiscMenu.AddItem(
                    new MenuItem("EnableStoreHeat", "Enable Storing Heat?").SetValue(false)
                        .SetTooltip("You either change the value here by clicking or by Scrolling Down using the mouse"))
                .Permashow(true, "Storing Heat?");

            #region DrawDamage

            var drawdamage = DrawingMenu.AddSubMenu(new Menu(":: Draw Damage", "drawdamage"));
            {
                var dmgAfterShave =
                    drawdamage.AddItem(
                        new MenuItem("SurvivorRumble.DrawComboDamage", "Draw Damage on Enemy's HP Bar").SetValue(true));
                var drawFill =
                    drawdamage.AddItem(new MenuItem("SurvivorRumble.DrawColour", "Fill Color", true).SetValue(
                        new Circle(true, System.Drawing.Color.Chartreuse)));
                DrawDamage.DamageToUnit = CalculateDamage;
                DrawDamage.Enabled = dmgAfterShave.GetValue<bool>();
                DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
                DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;
                dmgAfterShave.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                    };

                drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                    DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };
            }

            #endregion

            // Add everything to the main config/menu/root.
            Config.AddToMainMenu();

            #endregion

            #region Subscriptions

            Game.OnUpdate += GameOnUpdate;
            Drawing.OnDraw += DrawingOnOnDraw;
            Game.OnWndProc += OnWndProc;
            //Obj_AI_Base.OnProcessSpellCast += ObjAiHeroOnOnProcessSpellCast;
            Chat.Print("<font color='#800040'>[SurvivorSeries] Rumble</font> <font color='#ff6600'>Loaded.</font>");

            #endregion
        }

        private void OnWndProc(WndEventArgs args)
        {
            if (!Config.Item("EnableMouseScroll").GetValue<bool>())
                return;

            if (args.Msg == 0x20a)
                Config.Item("EnableStoreHeat").SetValue(!Config.Item("EnableStoreHeat").GetValue<bool>());
        }

        private void ObjAiHeroOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.Target is Obj_AI_Minion || !(sender is AIHeroClient))
                return;

            #region Spell Data Grab

            /*Chat.Print("Spell: " + args.SData.Name + " | Width: " + args.SData.LineWidth + " | Speed: " +
                           args.SData.MissileSpeed + " | Delay: " + args.SData.DelayCastOffsetPercent);*/

            #endregion
        }

        private void SebbySpell(Spell ER, Obj_AI_Base target)
        {
            if (Config.Item("Prediction").GetValue<StringList>().SelectedIndex == 1)
            {
                var CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
                var aoe2 = false;

                if (ER.Type == SkillshotType.SkillshotCircle)
                {
                    CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
                    aoe2 = true;
                }

                if ((ER.Width > 80) && !ER.Collision)
                    aoe2 = true;

                var predInput2 = new PredictionInput
                {
                    Aoe = aoe2,
                    Collision = ER.Collision,
                    Speed = ER.Speed,
                    Delay = ER.Delay,
                    Range = ER.Range,
                    From = Player.ServerPosition,
                    Radius = ER.Width,
                    Unit = target,
                    Type = CoreType2
                };
                var poutput2 = SebbyLib.Prediction.Prediction.GetPrediction(predInput2);

                if (OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                    return;

                if ((ER.Speed != float.MaxValue) &&
                    OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                    return;

                if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 0)
                {
                    if (poutput2.Hitchance >= HitChance.Medium)
                        ER.Cast(poutput2.CastPosition);
                    else if (predInput2.Aoe && (poutput2.AoeTargetsHitCount > 1) && (poutput2.Hitchance >= HitChance.Medium))
                        ER.Cast(poutput2.CastPosition);
                }
                else if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 1)
                {
                    if (poutput2.Hitchance >= HitChance.High)
                        ER.Cast(poutput2.CastPosition);
                }
                else if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 2)
                {
                    if (poutput2.Hitchance >= HitChance.VeryHigh)
                        ER.Cast(poutput2.CastPosition);
                }
            }
            else if (Config.Item("Prediction").GetValue<StringList>().SelectedIndex == 0)
            {
                if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 0)
                    ER.CastIfHitchanceEquals(target, LeagueSharp.Common.HitChance.Medium);
                else if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 1)
                    ER.CastIfHitchanceEquals(target, LeagueSharp.Common.HitChance.High);
                else if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 2)
                    ER.CastIfHitchanceEquals(target, LeagueSharp.Common.HitChance.VeryHigh);
            }
            else if (Config.Item("Prediction").GetValue<StringList>().SelectedIndex == 2)
            {
                var hero = target as AIHeroClient;
                if ((hero != null) && hero.IsValid)
                {
                    var t = hero;
                    if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 0)
                        ER.SPredictionCast(t, LeagueSharp.Common.HitChance.Medium);
                    else if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 1)
                        ER.SPredictionCast(t, LeagueSharp.Common.HitChance.High);
                    else if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 2)
                        ER.SPredictionCast(t, LeagueSharp.Common.HitChance.VeryHigh);
                }
                else
                {
                    ER.CastIfHitchanceEquals(target, LeagueSharp.Common.HitChance.High);
                }
            }
        }

        private void DrawingOnOnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            if (Config.Item("DrawQ").GetValue<bool>() && Q.IsReady())
                Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Chartreuse);
            if (Config.Item("DrawE").GetValue<bool>() && E.IsReady())
                Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.GreenYellow);
            if (Config.Item("DrawR").GetValue<bool>() && R.IsReady())
                Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.DarkOrange);
            if ((BubbaFat != null) && Config.Item("ComboCastUltimate").GetValue<KeyBind>().Active)
            {
                var kickLineMenu = Config.Item("drawKickLine").GetValue<Circle>();
                if (kickLineMenu.Active)
                {
                    var start = Drawing.WorldToScreen(BubbaFat.KickPos.To3D());
                    var end = Drawing.WorldToScreen(BubbaFat.TargetHero.Position);

                    Drawing.DrawLine(start, end, 6, kickLineMenu.Color);
                }

                var kickPosMenu = Config.Item("drawKickPos").GetValue<Circle>();
                if (kickPosMenu.Active)
                    Render.Circle.DrawCircle(BubbaFat.KickPos.To3D(), 55f, kickPosMenu.Color, 6);


                var kickTargetMenu = Config.Item("drawRTarget").GetValue<Circle>();
                if (kickTargetMenu.Active)
                    Render.Circle.DrawCircle(BubbaFat.TargetHero.Position, 55f, kickTargetMenu.Color, 3);
            }
        }

        /// <summary>
        ///     Full Credits to Kurisu MVP
        /// </summary>
        /// <param name="target"></param>
        private void BubbKushGo(AIHeroClient target)
        {
            var posChecked = 0;
            var maxPosToCheck = 50;
            var posRadius = 50;
            var radiusIndex = 0;

            var bubba = new Bubba();
            var bubbaList = new List<Bubba>();

            while (posChecked < maxPosToCheck)
            {
                radiusIndex++;
                var curRadius = radiusIndex*2*posRadius;
                var curCurcleChecks = (int) Math.Ceiling(2*Math.PI*curRadius/(2*(double) posRadius));

                for (var i = 1; i < curCurcleChecks; i++)
                {
                    posChecked++;

                    var cRadians = 0x2*Math.PI/(curCurcleChecks - 1)*i;
                    var startPos = new Vector2((float) Math.Floor(target.Position.X + curRadius*
                                                                  Math.Cos(cRadians)),
                        (float) Math.Floor(target.Position.Y + curRadius*Math.Sin(cRadians)));

                    var endPos = startPos.Extend(target.Position.To2D(), 1000f);
                    var targetProj = target.Position.To2D().ProjectOn(startPos, endPos);

                    foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget()))
                        if ((hero.NetworkId != target.NetworkId) && (hero.Distance(targetProj.SegmentPoint) <= 1000))
                        {
                            var mPos =
                                LeagueSharp.Common.Prediction.GetPrediction(hero, 320 + Game.Ping/2).UnitPosition.To2D();
                            // R.GetPrediction().CastPosition
                            var mProj = mPos.ProjectOn(startPos, endPos);
                            //if (NavMesh.GetCollisionFlags(endPos.To3D()) != CollisionFlags.Wall)
                            if (startPos.IsWall())
                                continue;
                            if (mProj.IsOnSegment &&
                                (mProj.SegmentPoint.Distance(hero.Position) <= hero.BoundingRadius + 100))
                                if (bubba.HeroesOnSegment.Contains(hero) == false)
                                {
                                    bubba.HeroToKick = hero;
                                    bubba.TargetHero = target;
                                    bubba.KickPos = hero.Position.To2D().Extend(startPos, -(hero.BoundingRadius + 35));
                                    bubba.HeroesOnSegment.Add(hero);
                                }
                        }

                    bubbaList.Add(bubba);

                    BubbaFat =
                        bubbaList.Where(x => x.HeroesOnSegment.Count > 0)
                            .OrderByDescending(x => x.HeroesOnSegment.Count)
                            .ThenByDescending(x => x.HeroToKick.MaxHealth).FirstOrDefault();

                    if (BubbaFat != null)
                    {
                        /*if (Config.Item("UseSmartCastingADC").GetValue<bool>())
                        {
                            var importantTarget =
                                HeroManager.Enemies.FirstOrDefault(
                                    x =>
                                        HighChamps.Contains(BubbaFat.TargetHero.ChampionName) &&
                                        x.IsValidTarget(R.Range));
                            if (importantTarget != null)
                                if (R.Instance.IsReady() &&
                                    Config.Item("SemiManualR").GetValue<bool>())
                                    R.Cast(importantTarget.Position, BubbaFat.HeroToKick.Position);
                        }*/
                        if (R.Instance.IsReady() && Config.Item("SemiManualR").GetValue<bool>())
                            R.Cast(BubbaFat.TargetHero.Position, BubbaFat.HeroToKick.Position);
                    }
                }
            }
        }

        private void GameOnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
                return;

            KillStealCheck();
            TakingFatalDamageCheck();
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    JungleClear();
                    LaneClear();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    HarassMode();
                    break;

                case Orbwalking.OrbwalkingMode.None:
                    StoreHeat();
                    break;
            }
            if (Config.Item("ComboCastUltimate").GetValue<KeyBind>().Active)
            {
                var t = TargetSelector.GetTarget(R.Range + 1000, TargetSelector.DamageType.Physical);
                if (t != null)
                    BubbKushGo(t);
            }
        }

        private void JungleClear()
        {
            var junglemobs =
                Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Neutral)
                    .OrderByDescending(x => x.MaxHealth)
                    .FirstOrDefault();
            if (junglemobs == null)
                return;

            if (junglemobs.IsValidTarget(Q.Range) && Q.IsReady() && Config.Item("JungleClearQ").GetValue<bool>() &&
                Player.IsFacing(junglemobs))
                Q.Cast();

            if (junglemobs.IsValidTarget(E.Range) && E.IsReady() && Config.Item("JungleClearE").GetValue<bool>())
                E.Cast(junglemobs.ServerPosition);
        }

        private void KillStealCheck()
        {
            if (Config.Item("EnableKS").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                if ((target == null) || !target.IsValidTarget())
                    return;

                if (Config.Item("KSQ").GetValue<bool>() && Player.IsFacing(target) && target.IsValidTarget(Q.Range) &&
                    Q.Instance.IsReady() &&
                    (target.Health < OktwCommon.GetKsDamage(target, Q)))
                    Q.Cast();
                if (Config.Item("KSE").GetValue<bool>() && target.IsValidTarget(E.Range) && E.Instance.IsReady() &&
                    (target.Health < OktwCommon.GetKsDamage(target, E)))
                    SebbySpell(E, target);
                if (Config.Item("KSItems").GetValue<bool>())
                {
                    if (GLP800.IsReady() && target.IsValidTarget(GLP800.Range) &&
                        (target.Health < OktwCommon.GetIncomingDamage(target) + (100 + Player.TotalMagicalDamage)*100))
                        GLP800.Cast(target.ServerPosition);
                    if (Protobelt.IsReady() && target.IsValidTarget(Protobelt.Range) &&
                        (target.Health < OktwCommon.GetIncomingDamage(target) + (75 + Player.TotalMagicalDamage)*100))
                        Protobelt.Cast(target.ServerPosition);
                }
            }
        }

        private void Combo()
        {
            var UseQ = Config.Item("ComboUseQ").GetValue<bool>();
            var UseW = Config.Item("ComboUseW").GetValue<bool>();
            var UseE = Config.Item("ComboUseE").GetValue<bool>();
            var UseItems = Config.Item("ComboUseItems").GetValue<bool>();

            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if ((target == null) || !target.IsValidTarget())
                return;
            if (UseQ && target.IsValidTarget(Q.Range) && Player.IsFacing(target) && (Player.Mana < 80))
                Q.Cast();
            if (UseE && target.IsValidTarget(E.Range) && (Player.Mana < 80))
                SebbySpell(E, target);
            if (UseW && (Player.CountEnemiesInRange(E.Range) > 0) && (Player.HealthPercent < 80))
                W.Cast();
            if ((target.CountEnemiesInRange(1000) <= 0) && target.IsValidTarget(700) &&
                Config.Item("ComboUseRSolo").GetValue<bool>())
                switch (target.IsFacing(Player))
                {
                    case true:
                        R.Cast(target.ServerPosition, Player.ServerPosition);
                        break;
                    case false:
                        R.Cast(Player.ServerPosition, target.ServerPosition);
                        break;
                }

            if (UseItems)
            {
                if (GLP800.IsReady() && target.IsValidTarget(GLP800.Range))
                    GLP800.Cast(target.ServerPosition);
                if (Protobelt.IsReady() && target.IsValidTarget(Protobelt.Range))
                    Protobelt.Cast(target.ServerPosition);
            }
        }

        private void StoreHeat()
        {
            if (Config.Item("EnableStoreHeat").GetValue<bool>() && (Player.Mana < 50))
            {
                if (Q.Instance.IsReady())
                    Q.Cast();
                if (W.Instance.IsReady())
                    W.Cast();
            }
        }

        private void TakingFatalDamageCheck()
        {
            if (Config.Item("UseWNearbyEnemy").GetValue<bool>() && (Player.CountEnemiesInRange(E.Range) > 0) &&
                (Player.HealthPercent < 20) && W.IsReady())
                W.Cast();
        }

        private void HarassMode()
        {
            var UseQ = Config.Item("HarassQ").GetValue<bool>();
            var UseE = Config.Item("HarassE").GetValue<bool>();
            var UseItems = Config.Item("HarassItems").GetValue<bool>();
            var HarassManaManager = Config.Item("HarassManaManager").GetValue<Slider>().Value;

            if (Player.ManaPercent < HarassManaManager)
                return;

            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if ((target == null) || !target.IsValidTarget())
                return;

            if (target.IsValidTarget(E.Range) && UseE && E.Instance.IsReady() && (Player.Mana < 90))
                SebbySpell(E, target);

            if (target.IsValidTarget(Q.Range) && UseQ && Q.Instance.IsReady() && Player.IsFacing(target) &&
                (Player.Mana < 80))
                Q.Cast();

            if (UseItems)
            {
                if (GLP800.IsReady() && target.IsValidTarget(GLP800.Range))
                    GLP800.Cast(target.ServerPosition);
                if (Protobelt.IsReady() && target.IsValidTarget(Protobelt.Range))
                    Protobelt.Cast(target.ServerPosition);
            }
        }

        private void LastHit()
        {
            var UseE = Config.Item("LastHitE").GetValue<bool>();
            var LastHitManaManager = Config.Item("LastHitManaManager").GetValue<Slider>().Value;

            if (Player.ManaPercent < LastHitManaManager)
                return;

            var minion = Cache.GetMinions(Player.Position, E.Range, MinionTeam.Enemy).FirstOrDefault();
            if ((minion == null) || !minion.IsValidTarget())
                return;

            if (UseE && E.Instance.IsReady() && (minion.Health + 10 < E.GetDamage(minion)))
                E.Cast(minion.ServerPosition);
        }

        private void LaneClear()
        {
            var UseQ = Config.Item("LaneClearQ").GetValue<bool>();
            var UseE = Config.Item("LaneClearE").GetValue<bool>();
            var LaneClearManaManager = Config.Item("LaneClearManaManager").GetValue<Slider>().Value;

            if (Player.ManaPercent < LaneClearManaManager)
                return;

            /* NEW LOGICS */
            var minioncache = Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Enemy);
            if ((minioncache != null) && (minioncache.Count >= Config.Item("MinimumQMinions").GetValue<Slider>().Value))
            {
                var selectedminion = minioncache.FirstOrDefault();
                if ((selectedminion == null) || !selectedminion.IsValidTarget())
                    return;
                if (Player.IsFacing(selectedminion) && selectedminion.IsValidTarget(Q.Range) && Q.Instance.IsReady() &&
                    UseQ)
                    Q.Cast();
            }
            if ((minioncache != null) && (minioncache.Count > 0))
            {
                var selectedminion = minioncache.FirstOrDefault();
                if ((selectedminion == null) || !selectedminion.IsValidTarget())
                    return;
                if (UseE && E.Instance.IsReady() && ObjectManager.Player.Spellbook.IsAutoAttacking && selectedminion.IsValidTarget(E.Range) &&
                    (E.GetDamage(selectedminion) > selectedminion.Health + 10))
                    E.Cast(selectedminion.ServerPosition);
            }
        }

        private float CalculateDamage(Obj_AI_Base enemy)
        {
            double damage = 0;

            if (Q.Instance.IsReady())
                damage += QDamage(enemy, 2);
            if (E.Instance.IsReady())
                damage += E.GetDamage(enemy);
            if (R.Instance.IsReady())
                damage += R.GetDamage(enemy);

            damage += Player.GetAutoAttackDamage(enemy);

            return (float) damage;
        }

        private double QDamage(Obj_AI_Base target, float time)
        {
            if (Player.Mana < 50)
            {
                var damage = Player.CalcDamage(target, Damage.DamageType.Magical,
                    (float) new[] {6.25, 11.25, 16.25, 21.25, 26.25}[Player.GetSpell(SpellSlot.Q).Level - 1] +
                    Player.TotalMagicalDamage/8.33*100);
                return damage*time/0.25;
            }
            else
            {
                var damage = Player.CalcDamage(target, Damage.DamageType.Magical,
                    (float) new[] {9.4, 16.9, 24.4, 31.9, 39.4}[Player.GetSpell(SpellSlot.Q).Level - 1] +
                    Player.TotalMagicalDamage/12.5*100);
                return damage*time/0.25;
            }
        }

        protected class Bubba
        {
            public List<AIHeroClient> HeroesOnSegment = new List<AIHeroClient>();
            public AIHeroClient HeroToKick;
            public Vector2 KickPos;
            public AIHeroClient TargetHero;
        }
    }
}