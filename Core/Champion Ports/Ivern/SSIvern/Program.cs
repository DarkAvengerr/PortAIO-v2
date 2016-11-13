using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using SPrediction;
using Color = SharpDX.Color;
using HitChance = SebbyLib.Prediction.HitChance;
using Orbwalking = SebbyLib.Orbwalking;
using Prediction = SPrediction.Prediction;
using PredictionInput = SebbyLib.Prediction.PredictionInput;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SSIvern
{
    internal class SSIvernInit
    {
        protected static Spell Q, W, E, R;
        protected static Orbwalking.Orbwalker Orbwalker;
        protected static Obj_AI_Base Daisy;
        protected static bool DaisyStatus = false;
        protected static Menu Config;
        private static bool SPredictionLoaded;
        public static string[] HighChamps =
            {
                "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
                "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "Leblanc",
                "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon",
                "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath",
                "Zed", "Ziggs","Kindred","Jhin"
            };
        //protected static int lvl1, lvl2, lvl3, lvl4;

        public SSIvernInit()
        {
            GameOnGameLoad();
        }

        protected static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public Items.Item GLP800 { get; private set; }
        public Items.Item Protobelt { get; private set; }
        public SpellSlot IgniteSlot { get; private set; }
        public SpellSlot SmiteSlot { get; private set; }

        public static void Main()
        {
            var SSIvernInit = new SSIvernInit();
        }

        public void GameOnGameLoad()
        {
            if (Player.ChampionName != "Ivern")
                return;

            #region Spells && Items

            //IgniteSlot = Player.GetSpellSlot("summonerdot");
            //var smite = Player.Spellbook.Spells.FirstOrDefault(x => x.Name.ToLower().Contains("smite"));
            Q = new Spell(SpellSlot.Q, 1100f);
            W = new Spell(SpellSlot.W, 1600f);
            E = new Spell(SpellSlot.E, 750f);
            R = new Spell(SpellSlot.R, 0);
            Q.SetSkillshot(0.5f, 65f, 1300f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.5f, 50f, 1600f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.5f, 100f, 1200f, false, SkillshotType.SkillshotCircle);
            /*if ((Ignite != null) && (IgniteSlot != SpellSlot.Unknown))
                Ignite = new Spell(IgniteSlot, 550f);
            if ((smite != null) && (smite.Slot != SpellSlot.Unknown))
                Smite = new Spell(smite.Slot, 500f, TargetSelector.DamageType.True);*/
            GLP800 = new Items.Item(3030, 800f);
            Protobelt = new Items.Item(3152, 850f);

            #endregion

            #region SpellDatabase Initializing

            #endregion

            #region Config

            Config = new Menu("SurvivorIvern", "SurvivorIvern", true).SetFontStyle(FontStyle.Bold, Color.Chartreuse);

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
            ComboMenu.AddItem(new MenuItem("ComboUseItems", "Use Items?").SetValue(true));
            ComboMenu.AddItem(
                new MenuItem("SemiManualR", "Semi-Manual R Casting?").SetValue(false)
                    .SetTooltip("True - Script will Auto R | False - You will R when you decide - preferably",
                        Color.Chartreuse));
            ComboMenu.AddItem(
                new MenuItem("ComboMinimumREnemies", "Minimum Enemies in E Range Before Casting R").SetValue(
                    new Slider(2, 1, 5)));
            ComboMenu.AddItem(
                new MenuItem("ComboUseR", "Use R").SetValue(true)
                    .SetTooltip(
                        "Will use R if there's more than 1 target"));

            var DaisyMenu = Config.AddSubMenu(new Menu(":: Daisy", "Daisy"));
            DaisyMenu.AddItem(new MenuItem("AutoDaisy", "Auto-Play Daisy?").SetValue(true));
            DaisyMenu.AddItem(new MenuItem("DaisyFooter", ":: Soon More Options! <3"));

            var EWhitelistMenu = Config.AddSubMenu(new Menu(":: (E) Protector", "Protector").SetFontStyle(FontStyle.Bold, Color.HotPink));
            {
                //EWhitelistMenu.AddItem(new MenuItem("EProtectionAlly", "Use (E) to Protect?").SetValue(true));
                foreach (var ally in HeroManager.Allies.Where(x => x.IsValid))
                {
                    EWhitelistMenu.AddItem(new MenuItem("EProtectionAlly." + ally.ChampionName, "Protect with (E): " + ally.ChampionName).SetValue(HighChamps.Contains(ally.ChampionName)));
                }
            }

            var LaneClearMenu = Config.AddSubMenu(new Menu(":: LaneClear", "LaneClear"));
            LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q").SetValue(true));
            LaneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E").SetValue(false));
            LaneClearMenu.AddItem(new MenuItem("LaneClearEAlly", "Use E on Ally to Farm").SetValue(false));
            LaneClearMenu.AddItem(
                new MenuItem("LaneClearManaManager", "LaneClear Mana Manager").SetValue(new Slider(50, 0, 100)));
            LaneClearMenu.AddItem(
                new MenuItem("MinimumEMinions", "Minimum Minions Near You To Use E?").SetValue(new Slider(3, 1, 10)));

            var LastHitMenu = Config.AddSubMenu(new Menu(":: LastHit", "LastHit"));
            LastHitMenu.AddItem(new MenuItem("LastHitQ", "Use Q").SetValue(true));
            LastHitMenu.AddItem(
                new MenuItem("LastHitManaManager", "LastHit Mana Manager Mana Manager").SetValue(new Slider(50, 0, 100)));

            var HarassMenu = Config.AddSubMenu(new Menu(":: Harass", "Harass"));
            HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q").SetValue(true));
            HarassMenu.AddItem(new MenuItem("HarassW", "Use W").SetValue(true));
            HarassMenu.AddItem(new MenuItem("HarassItems", "Use Items (GLP/Protobelt)").SetValue(true));
            HarassMenu.AddItem(
                new MenuItem("HarassManaManager", "Harass Mana Manager Mana Manager").SetValue(new Slider(50, 0, 100)));

            var KillStealMenu = Config.AddSubMenu(new Menu(":: Killsteal", "Killsteal"));
            KillStealMenu.AddItem(new MenuItem("EnableKS", "Enable Killsteal?").SetValue(true));
            KillStealMenu.AddItem(new MenuItem("KSQ", "KS with Q?").SetValue(true));
            KillStealMenu.AddItem(new MenuItem("KSItems", "KS with Items?").SetValue(true));
            KillStealMenu.AddItem(new MenuItem("UnavailableService",
                "KS with Ignite/Smite is currently (Temporary) Unavailable."));
            KillStealMenu.AddItem(new MenuItem("KSIgnite", "KS with Ignite").SetValue(false));
            KillStealMenu.AddItem(new MenuItem("KSSmite", "KS with Smite").SetValue(false));

            var DrawingMenu = Config.AddSubMenu(new Menu(":: Drawings", "Drawings"));
            DrawingMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawW", "Draw W Range").SetValue(false));
            DrawingMenu.AddItem(new MenuItem("DrawE", "Draw E Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawR", "Draw R Range").SetValue(false));
            DrawingMenu.AddItem(new MenuItem("DrawDaisy", "Draw Daisy Attack Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawDaisyStatus", "Draw Daisy Status/Mode!").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawJungleMobPassive", "Draw Jungle Mob Ready!").SetValue(true));

            var GapCloserInterrupter =
                Config.AddSubMenu(
                    new Menu(":: Anti-GapCloser/Interrupter", "GapCloserInterrupter").SetFontStyle(FontStyle.Bold,
                        Color.Chartreuse));
            GapCloserInterrupter.AddItem(new MenuItem("QToInterrupt", "Q to Interrupt?").SetValue(true));
            GapCloserInterrupter.AddItem(new MenuItem("GapCloserAutoE", "E on Gapclose enemy near you?").SetValue(false));
            GapCloserInterrupter.AddItem(new MenuItem("GapCloserInfo",
                "                   .:Gap-Closer Q on Enemy (Whitelist):."));
            foreach (var enemy in HeroManager.Enemies)
                GapCloserInterrupter
                    .AddItem(
                        new MenuItem("GapCloserEnemies" + enemy.ChampionName, enemy.ChampionName).SetValue(true)
                            .SetTooltip("Use Q on GapClosing Champion (" + enemy.ChampionName + ")"));

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
                            "<font color='#0993F9'>[SS Ivern Warning]</font> <font color='#FF8800'>Please exit the menu and click back on it again, to see the settings or Reload (F5)</font>");

                        SPredictionLoaded = true;
                    }
            };
            MiscMenu.AddItem(new MenuItem("UseENearbyEnemy", "[Auto] Use (E) Nearby Enemies").SetValue(false));
            MiscMenu.AddItem(
                new MenuItem("MinimumEnemiesNearEDistance", "Distance between You and Enemies before E-ing?").SetValue(
                    new Slider(700, 1, 2000)));
            MiscMenu.AddItem(
                new MenuItem("MinimumEnemiesNearE", "Minimum Enemies Near You to E?").SetValue(new Slider(2, 1, 5)));

            #region DrawDamage

            var drawdamage = new Menu(":: Draw Damage", "drawdamage");
            {
                var dmgAfterShave =
                    new MenuItem("SurvivorIvern.DrawComboDamage", "Draw Damage on Enemy's HP Bar").SetValue(true);
                var drawFill =
                    new MenuItem("SurvivorIvern.DrawColour", "Fill Color", true).SetValue(
                        new Circle(true, System.Drawing.Color.Chartreuse));
                drawdamage.AddItem(drawFill);
                drawdamage.AddItem(dmgAfterShave);
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
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            GameObject.OnDelete += GameObjectOnOnDelete;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += ObjAiHeroOnOnProcessSpellCast;
            Chat.Print("<font color='#800040'>[SurvivorSeries] Ivern</font> <font color='#ff6600'>Loaded.</font>");

            #endregion
        }

        private void GameObjectOnOnDelete(GameObject sender, EventArgs args)
        {
            if (sender.IsValid && sender.IsAlly && sender is Obj_AI_Minion && (sender.Name.ToLower() == "ivernminion"))
                Daisy = null;
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Q.Instance.IsReady())
            {
                var GapCloser = gapcloser.Sender;
                if (Config.Item("GapCloserEnemies" + GapCloser.ChampionName).GetValue<bool>() &&
                    GapCloser.IsValidTarget(400))
                    Q.Cast(GapCloser.ServerPosition);
                if (Config.Item("GapCloserAutoE").GetValue<bool>() && GapCloser.IsValidTarget(200))
                    E.CastOnUnit(Player);
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Config.Item("QToInterrupt").GetValue<bool>() && sender.IsValidTarget(Q.Range) && Q.IsReady())
                Q.Cast(sender);
        }

        private void ObjAiHeroOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.Target is Obj_AI_Minion || !(sender is AIHeroClient))
                return;
            /*Chat.Print("Spell: " + args.SData.Name + " | Width: " + args.SData.LineWidth + " | Speed: " +
                           args.SData.MissileSpeed + " | Delay: " + args.SData.DelayCastOffsetPercent);*/
            if (E.Instance.IsReady() && sender.IsEnemy && args.Target != null && ((Obj_AI_Base)args.Target).IsValidTarget(E.Range))
            {
                //Chat.Print(sender.Target.Name + " | " + args.Target.Type + " | " + args.SData.Name + " | " + sender.Name + " | " + (AIHeroClient)args.Target);
                if (args.Target is AIHeroClient && Config.Item("EProtectionAlly."+((AIHeroClient)args.Target).ChampionName).GetValue<bool>()
                    && sender.GetSpellDamage(((Obj_AI_Base)args.Target), args.SData.Name) + 150 > ((Obj_AI_Base)args.Target).Health)
                {
                    E.CastOnUnit((Obj_AI_Base)args.Target);
                    Notifications.AddNotification(
                        new Notification("Successfully protected " + ((AIHeroClient)args.Target).ChampionName, 3000).SetTextColor(
                            System.Drawing.Color.Chartreuse));
                }
            }
        }

        private void SebbySpell(Spell QW, Obj_AI_Base target)
        {
            if (Config.Item("Prediction").GetValue<StringList>().SelectedIndex == 1)
            {
                var CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
                var aoe2 = false;

                if (QW.Type == SkillshotType.SkillshotCircle)
                {
                    CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
                    aoe2 = true;
                }

                if ((QW.Width > 80) && !QW.Collision)
                    aoe2 = true;

                var predInput2 = new PredictionInput
                {
                    Aoe = aoe2,
                    Collision = QW.Collision,
                    Speed = QW.Speed,
                    Delay = QW.Delay,
                    Range = QW.Range,
                    From = Player.ServerPosition,
                    Radius = QW.Width,
                    Unit = target,
                    Type = CoreType2
                };
                var poutput2 = SebbyLib.Prediction.Prediction.GetPrediction(predInput2);

                if (OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                    return;

                if ((QW.Speed != float.MaxValue) &&
                    OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                    return;

                if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 0)
                {
                    if (poutput2.Hitchance >= HitChance.Medium)
                        QW.Cast(poutput2.CastPosition);
                    else if (predInput2.Aoe && (poutput2.AoeTargetsHitCount > 1) && (poutput2.Hitchance >= HitChance.Medium))
                        QW.Cast(poutput2.CastPosition);
                }
                else if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 1)
                {
                    if (poutput2.Hitchance >= HitChance.High)
                        QW.Cast(poutput2.CastPosition);
                }
                else if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 2)
                {
                    if (poutput2.Hitchance >= HitChance.VeryHigh)
                        QW.Cast(poutput2.CastPosition);
                }
            }
            else if (Config.Item("Prediction").GetValue<StringList>().SelectedIndex == 0)
            {
                if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 0)
                    QW.CastIfHitchanceEquals(target, LeagueSharp.Common.HitChance.Medium);
                else if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 1)
                    QW.CastIfHitchanceEquals(target, LeagueSharp.Common.HitChance.High);
                else if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 2)
                    QW.CastIfHitchanceEquals(target, LeagueSharp.Common.HitChance.VeryHigh);
            }
            else if (Config.Item("Prediction").GetValue<StringList>().SelectedIndex == 2)
            {
                if (target is AIHeroClient && target.IsValid)
                {
                    var t = target as AIHeroClient;
                    if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 0)
                        QW.SPredictionCast(t, LeagueSharp.Common.HitChance.Medium);
                    else if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 1)
                        QW.SPredictionCast(t, LeagueSharp.Common.HitChance.High);
                    else if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 2)
                        QW.SPredictionCast(t, LeagueSharp.Common.HitChance.VeryHigh);
                }
                else
                {
                    QW.CastIfHitchanceEquals(target, LeagueSharp.Common.HitChance.High);
                }
            }
        }

        private void Obj_AI_Base_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.Name == "Ivern_Base_P_cas.troy")
            {
                Notifications.AddNotification(
                    new Notification("Jungle Camp claiming.", 4000).SetTextColor(
                        System.Drawing.Color.Chartreuse));
            }
            if (obj.Name == "Ivern_Base_P_FinishedGround.troy")
            {
                Notifications.AddNotification(
                    new Notification("Jungle Camp is now ready.", 4000).SetTextColor(
                        System.Drawing.Color.Chartreuse));
            }
            if (obj.IsValid && obj.IsAlly && obj is Obj_AI_Minion && (obj.Name.ToLower() == "ivernminion"))
                Daisy = obj as Obj_AI_Base;
        }

        private void DrawingOnOnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            if (Config.Item("DrawQ").GetValue<bool>() && Q.IsReady())
                Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Chartreuse);
            if (Config.Item("DrawW").GetValue<bool>() && W.IsReady())
                Render.Circle.DrawCircle(Player.Position, W.Range, System.Drawing.Color.DeepPink);
            if (Config.Item("DrawE").GetValue<bool>() && E.IsReady())
                Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.GreenYellow);
            if (Config.Item("DrawR").GetValue<bool>() && R.IsReady())
                Render.Circle.DrawCircle(Player.Position, R.Range, System.Drawing.Color.DarkOrange);
            if (Config.Item("DrawDaisy").GetValue<bool>() && R.IsReady())
                if ((Daisy != null) && Daisy.IsValid)
                {
                    Render.Circle.DrawCircle(Daisy.Position, 200f, System.Drawing.Color.Chartreuse);
                    if (Config.Item("DrawDaisyStatus").GetValue<bool>())
                    {
                        switch (DaisyStatus)
                        {
                            case false:
                            {
                                var drawPos = Drawing.WorldToScreen(Player.Position);
                                var textSize = Drawing.GetTextEntent(("Daisy: Following Player"), 15);
                                Drawing.DrawText(drawPos.X - textSize.Width - 70f, drawPos.Y,
                                    System.Drawing.Color.Chartreuse,
                                    "Daisy: Following Player");
                            }
                                break;
                            case true:
                            {
                                var drawPos = Drawing.WorldToScreen(Player.Position);
                                var textSize = Drawing.GetTextEntent(("Daisy: Engaging!"), 15);
                                Drawing.DrawText(drawPos.X - textSize.Width - 70f, drawPos.Y,
                                    System.Drawing.Color.DeepPink,
                                    "Daisy: Engaging!");
                            }
                                break;
                        }
                    }
                }
            if (Config.Item("DrawJungleMobPassive").GetValue<bool>())
                foreach (var junglemob in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.HasBuff("ivernpmanager")))
                    if (!junglemob.HasBuff("ivernpmonsterready"))
                        Render.Circle.DrawCircle(junglemob.Position, 200, System.Drawing.Color.DarkOrange, 15);
                    else if (junglemob.HasBuff("ivernpmonsterready"))
                        Render.Circle.DrawCircle(junglemob.Position, 200, System.Drawing.Color.Chartreuse, 15);
        }

        private void GameOnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
                return;

            KillStealCheck();
            TakingFatalDamageCheck();
            DaisyProgram();
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    HarassMode();
                    break;
            }
        }

        private void KillStealCheck()
        {
            if (Config.Item("EnableKS").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                if ((target == null) || !target.IsValidTarget())
                    return;

                if (Config.Item("KSQ").GetValue<bool>() && Q.Instance.IsReady() &&
                    (target.Health < OktwCommon.GetKsDamage(target, Q)))
                    SebbySpell(Q, target);
                if (Config.Item("KSItems").GetValue<bool>())
                {
                    if (GLP800.IsReady() && target.IsValidTarget(GLP800.Range) &&
                        (target.Health < OktwCommon.GetIncomingDamage(target) + (100 + Player.TotalMagicalDamage)*100))
                        GLP800.Cast(target.ServerPosition);
                    if (Protobelt.IsReady() && target.IsValidTarget(Protobelt.Range) &&
                        (target.Health < OktwCommon.GetIncomingDamage(target) + (75 + Player.TotalMagicalDamage)*100))
                        Protobelt.Cast(target.ServerPosition);
                }
                /*if (Config.Item("KSIgnite").GetValue<bool>() && Ignite != null && Ignite.Slot != SpellSlot.Unknown && Player.Spellbook.GetSpell(Ignite.Slot).State == SpellState.Ready &&
                    target.Health < OktwCommon.GetIncomingDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite))
                    Player.Spellbook.CastSpell(Ignite.Slot, target);
                if (Config.Item("KSSmite").GetValue<bool>() && Smite != null && Smite.Slot != SpellSlot.Unknown &&
                    target.Health < OktwCommon.GetIncomingDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Smite))
                    Player.Spellbook.CastSpell(Smite.Slot, target);*/
            }
        }

        private void Combo()
        {
            var UseQ = Config.Item("ComboUseQ").GetValue<bool>();
            var UseW = Config.Item("ComboUseW").GetValue<bool>();
            var UseE = Config.Item("ComboUseE").GetValue<bool>();
            var UseR = Config.Item("ComboUseR").GetValue<bool>();
            var UseItems = Config.Item("ComboUseItems").GetValue<bool>();
            var ComboMinimumREnemies = Config.Item("ComboMinimumREnemies").GetValue<Slider>().Value;

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if ((target == null) || !target.IsValidTarget())
                return;
            if (Player.HasBuff("ivernqallyjump") && (target.CountEnemiesInRange(1000) <= 1))
                Q.Cast();
            if (UseE && E.Instance.IsReady() && target.IsValidTarget(E.Range))
                E.CastOnUnit(Player);
            if (UseQ && Q.Instance.IsReady())
                SebbySpell(Q, target);
            if (!Config.Item("SemiManualR").GetValue<bool>())
                if (UseR && (Player.CountEnemiesInRange(E.Range) >= ComboMinimumREnemies) && R.Instance.IsReady())
                    R.Cast(target.ServerPosition);
            if (UseW && W.Instance.IsReady() && (target.Distance(Player) > Player.AttackRange) &&
                (target.Distance(Player) < 450) && !Player.HasBuff("IvernW"))
                W.Cast(Player.ServerPosition);

            if (UseItems)
            {
                if (GLP800.IsReady() && target.IsValidTarget(GLP800.Range))
                    GLP800.Cast(target.ServerPosition);
                if (Protobelt.IsReady() && target.IsValidTarget(Protobelt.Range))
                    Protobelt.Cast(target.ServerPosition);
            }
        }

        private void TakingFatalDamageCheck()
        {
            /*foreach (var allyprotector in HeroManager.Allies.Where(x => x.IsValidTarget(E.Range)))
                if (E.Instance.IsReady() && allyprotector.Health < OktwCommon.GetIncomingDamage(allyprotector, 6))
                {
                    E.CastOnUnit(allyprotector);
                    Notifications.AddNotification(
                        new Notification("Successfully protected " + allyprotector.ChampionName, 3000).SetTextColor(
                            System.Drawing.Color.Chartreuse));
                }
            if (Player.Health < OktwCommon.GetIncomingDamage(Player, 6) && E.IsReady())
            {
                E.CastOnUnit(Player);
                Notifications.AddNotification(
                        new Notification("Successfully Protected Yourself", 3000).SetTextColor(
                            System.Drawing.Color.Chartreuse));
            }*/
            if (Config.Item("UseENearbyEnemy").GetValue<bool>() && Player.CountEnemiesInRange(Config.Item("MinimumEnemiesNearEDistance").GetValue<Slider>().Value) >=
                Config.Item("MinimumEnemiesNearE").GetValue<Slider>().Value && E.IsReady())
                E.CastOnUnit(Player);
        }

        private void HarassMode()
        {
            var UseQ = Config.Item("HarassQ").GetValue<bool>();
            var UseW = Config.Item("HarassW").GetValue<bool>();
            var UseItems = Config.Item("HarassItems").GetValue<bool>();
            var HarassManaManager = Config.Item("HarassManaManager").GetValue<Slider>().Value;

            if (Player.ManaPercent < HarassManaManager)
                return;

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if ((target == null) || !target.IsValidTarget())
                return;

            if (UseQ && target.IsValidTarget(Q.Range) && Q.Instance.IsReady())
                SebbySpell(Q, target);

            if (UseW && (target.Distance(Player) > Player.AttackRange) && (target.Distance(Player) < 450) &&
                !Player.HasBuff("IvernW") && W.Instance.IsReady())
                W.Cast(Player.ServerPosition);

            if (UseItems)
            {
                if (GLP800.IsReady() && target.IsValidTarget(GLP800.Range))
                    GLP800.Cast(target.ServerPosition);
                if (Protobelt.IsReady() && target.IsValidTarget(Protobelt.Range))
                    Protobelt.Cast(target.ServerPosition);
            }
        }

        private void DaisyProgram()
        {
            if (R.Instance.IsReady())
                if (Config.Item("AutoDaisy").GetValue<bool>() && (Daisy != null) && Daisy.IsValid)
                {
                    var enemy =
                        HeroManager.Enemies.Where(
                                x => x.IsValidTarget() && (Daisy.Distance(x.Position) < 1000) && !x.UnderTurret(true))
                            .OrderBy(x => x.Distance(Daisy))
                            .FirstOrDefault();
                    if (enemy != null)
                    {
                        if (Daisy.Distance(enemy.Position) > 200)
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MovePet, enemy);
                            DaisyStatus = true;
                        }
                        else
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttackPet, enemy);
                            DaisyStatus = true;
                            E.CastOnUnit(Daisy);
                        }
                    }
                    else
                    {
                        var iverntarget = Orbwalker.GetTarget() as Obj_AI_Base;
                        if (iverntarget != null)
                            if (Daisy.Distance(iverntarget.Position) > 200)
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MovePet, iverntarget);
                                DaisyStatus = true;
                            }
                            else
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttackPet, iverntarget);
                                DaisyStatus = true;
                                E.CastOnUnit(Daisy);
                            }
                        else if (Daisy.UnderTurret(true))
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MovePet, Player);
                            DaisyStatus = false;
                        }
                    }
                }
                else
                {
                    Daisy = null;
                }
        }

        private void LastHit()
        {
            var UseQ = Config.Item("LastHitQ").GetValue<bool>();
            var LastHitManaManager = Config.Item("LastHitManaManager").GetValue<Slider>().Value;

            if (Player.ManaPercent < LastHitManaManager)
                return;

            var minion = Cache.GetMinions(Player.Position, Q.Range, MinionTeam.Enemy).FirstOrDefault();
            if ((minion == null) || !minion.IsValidTarget())
                return;

            if (UseQ && Q.Instance.IsReady() && (minion.Health < Q.GetDamage(minion)))
                Q.Cast(minion.ServerPosition);
        }

        private void LaneClear()
        {
            var UseQ = Config.Item("LaneClearQ").GetValue<bool>();
            var UseE = Config.Item("LaneClearE").GetValue<bool>();
            var LaneClearManaManager = Config.Item("LaneClearManaManager").GetValue<Slider>().Value;
            var MinimumEMinions = Config.Item("MinimumEMinions").GetValue<Slider>().Value;

            var minionsq =
                Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Enemy)
                    .OrderByDescending(x => x.Distance(Player.Position))
                    .FirstOrDefault();

            if (Player.ManaPercent < LaneClearManaManager)
                return;

            var minionselist = Cache.GetMinions(Player.ServerPosition, 120, MinionTeam.Enemy);
            var minionallylist = Cache.GetMinions(Player.Position, 2*E.Range, MinionTeam.Enemy).FirstOrDefault();
            if ((minionallylist != null) || minionallylist.IsValidTarget())
            {
                var ally = HeroManager.Allies.Where(x => x.Distance(minionallylist) <= 120);
                if (ally.Any())
                    E.CastOnUnit(ally.FirstOrDefault());
            }
            if (UseQ && Q.Instance.IsReady() && minionsq.IsValidTarget() && (minionsq != null) &&
                (minionsq.Health < Q.GetDamage(minionsq)))
                Q.CastOnUnit(minionsq);

            if (UseE && E.Instance.IsReady() && (minionselist.Count > MinimumEMinions))
                E.Cast();
        }

        private float CalculateDamage(Obj_AI_Base enemy)
        {
            double damage = 0;

            if (Q.Instance.IsReady())
                damage += Q.GetDamage(enemy);
            if (E.Instance.IsReady())
                damage += E.GetDamage(enemy);

            damage += Player.GetAutoAttackDamage(enemy);

            if (R.Instance.IsReady())
                damage += R.GetDamage(enemy);

            return (float) damage;
        }
    }
}
