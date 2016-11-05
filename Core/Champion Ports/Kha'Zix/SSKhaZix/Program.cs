using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using Color = SharpDX.Color;
using HitChance = SebbyLib.Prediction.HitChance;
using Orbwalking = SebbyLib.Orbwalking;
using Prediction = SebbyLib.Prediction.Prediction;
using PredictionInput = SebbyLib.Prediction.PredictionInput;

using EloBuddy;
using LeagueSharp.Common;
namespace SSKhaZix
{
    internal class SSKhaXiz
    {
        protected static Spell Q, W, E, R, Ignite, Smite;
        protected static Orbwalking.Orbwalker Orbwalker;
        protected static Menu Config;

        //protected static int lvl1, lvl2, lvl3, lvl4;
        protected bool BoolEvolvedQ, BoolEvolvedW, BoolEvolvedE;
        private bool IsMidAir;

        public SSKhaXiz()
        {
            GameOnGameLoad();
        }

        protected static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public Items.Item Hydra { get; private set; }
        public Items.Item Tiamat { get; private set; }
        public Items.Item Blade { get; private set; }
        public Items.Item Bilgewater { get; private set; }
        public Items.Item Youmuu { get; private set; }
        public Items.Item TitanicHydra { get; private set; }
        public SpellSlot IgniteSlot { get; private set; }
        public SpellSlot SmiteSlot { get; private set; }

        public static void Main()
        {
            var KhaXiz = new SSKhaXiz();
        }

        public void GameOnGameLoad()
        {
            if (Player.ChampionName != "Khazix")
                return;

            #region Spells && Items

            IgniteSlot = Player.GetSpellSlot("summonerdot");
            var smite = Player.Spellbook.Spells.FirstOrDefault(x => x.Name.ToLower().Contains("smite"));
            Q = new Spell(SpellSlot.Q, 325f);
            W = new Spell(SpellSlot.W, 1000f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 0);
            W.SetSkillshot(0.225f, 80f, 825f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 100f, 1000f, false, SkillshotType.SkillshotCircle);
            if (IgniteSlot != SpellSlot.Unknown)
                Ignite = new Spell(IgniteSlot, 550f);
            if ((smite != null) && (smite.Slot != SpellSlot.Unknown))
                Smite = new Spell(smite.Slot, 500f, TargetSelector.DamageType.True);

            Hydra = new Items.Item(3074, 225f);
            Tiamat = new Items.Item(3077, 225f);
            Blade = new Items.Item(3153, 450f);
            Bilgewater = new Items.Item(3144, 450f);
            Youmuu = new Items.Item(3142, 185f);
            TitanicHydra = new Items.Item(3748, 225f);

            #endregion

            #region Config

            Config = new Menu("SurvivorKhaZix", "SurvivorKhaZix", true).SetFontStyle(FontStyle.Bold, Color.Crimson);

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
            ComboMenu.AddItem(new MenuItem("DontAAInInvisible", "Don't AA while Invisible?").SetValue(true));
            ComboMenu.AddItem(
                new MenuItem("ComboDontEUnderTurret", "Don't E (Jump) under Turret?").SetValue(true)
                    .SetTooltip("Set it to 'true' to be Safe."));
            ComboMenu.AddItem(
                new MenuItem("ComboMinimumREnemies", "Minimum Enemies in E Range Before Casting R").SetValue(
                    new Slider(2, 1, 5)));
            ComboMenu.AddItem(
                new MenuItem("ComboUseR", "Use R").SetValue(true)
                    .SetTooltip(
                        "Will use R if there's more than 1 target, or if Low HP will go into Survive(Fight) or Die mode."));

            var LaneClearMenu = Config.AddSubMenu(new Menu(":: LaneClear", "LaneClear"));
            LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q").SetValue(true));
            LaneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W").SetValue(true));
            LaneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E").SetValue(false));
            LaneClearMenu.AddItem(new MenuItem("LaneClearItems", "Use Hydra/Tiamat to LaneClear?").SetValue(true));
            LaneClearMenu.AddItem(
                new MenuItem("LaneClearManaManager", "LaneClear Mana Manager").SetValue(new Slider(50, 0, 100)));
            LaneClearMenu.AddItem(
                new MenuItem("MinimumEMinions", "Minimum Minions To Hit Using E?").SetValue(new Slider(3, 0, 10)));

            var JungleClearMenu = Config.AddSubMenu(new Menu(":: JungleClear", "JungleClear"));
            JungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q").SetValue(true));
            JungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W").SetValue(true));
            JungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E").SetValue(false));
            JungleClearMenu.AddItem(new MenuItem("JungleClearItems", "Use Hydra/Tiamat to JungleClear?").SetValue(true));
            JungleClearMenu.AddItem(
                new MenuItem("JungleClearDontEQRange", "Don't Use E if Mobs are in Q Range?").SetValue(true));
            JungleClearMenu.AddItem(
                new MenuItem("JungleClearManaManager", "JungleClear Mana Manager").SetValue(new Slider(50, 0, 100)));
            JungleClearMenu.AddItem(
                new MenuItem("MinimumEJungleMobs", "Minimum Mobs in E before Jumping?").SetValue(new Slider(2, 1, 4)));

            var LastHitMenu = Config.AddSubMenu(new Menu(":: LastHit", "LastHit"));
            LastHitMenu.AddItem(new MenuItem("LastHitQ", "Use Q").SetValue(true));
            LastHitMenu.AddItem(new MenuItem("LastHitW", "Use W").SetValue(false));
            LastHitMenu.AddItem(
                new MenuItem("LastHitManaManager", "LastHit Mana Manager Mana Manager").SetValue(new Slider(50, 0, 100)));

            var HarassMenu = Config.AddSubMenu(new Menu(":: Harass", "Harass"));
            HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q").SetValue(true));
            HarassMenu.AddItem(new MenuItem("HarassW", "Use W").SetValue(true));
            HarassMenu.AddItem(
                new MenuItem("HarassManaManager", "Harass Mana Manager Mana Manager").SetValue(new Slider(50, 0, 100)));

            var KillStealMenu = Config.AddSubMenu(new Menu(":: Killsteal", "Killsteal"));
            KillStealMenu.AddItem(new MenuItem("EnableKS", "Enable Killsteal?").SetValue(true));
            KillStealMenu.AddItem(new MenuItem("KSQ", "KS with Q?").SetValue(true));
            KillStealMenu.AddItem(new MenuItem("KSW", "KS with W").SetValue(true));
            KillStealMenu.AddItem(new MenuItem("KSE", "KS with E").SetValue(true));
            KillStealMenu.AddItem(new MenuItem("UnavailableService",
                "KS with Ignite/Smite is currently (Temporary) Unavailable."));
            KillStealMenu.AddItem(new MenuItem("KSIgnite", "KS with Ignite").SetValue(false));
            KillStealMenu.AddItem(new MenuItem("KSSmite", "KS with Smite").SetValue(false));

            var DrawingMenu = Config.AddSubMenu(new Menu(":: Drawings", "Drawings"));
            DrawingMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawW", "Draw W Range").SetValue(false));
            DrawingMenu.AddItem(new MenuItem("DrawE", "Draw E Range").SetValue(true));
            DrawingMenu.AddItem(
                new MenuItem("DrawIsolated", "Draw Isolated?").SetValue(true).SetTooltip("Preferably set to 'true'."));
            DrawingMenu.AddItem(new MenuItem("DrawIsMidAirDebug", "Draw isMidAir (Debug)").SetValue(false));

            var MiscMenu = Config.AddSubMenu(new Menu(":: Misc", "Misc"));
            MiscMenu.AddItem(
                new MenuItem("HitChance", "Hit Chance").SetValue(new StringList(new[] { "Medium", "High", "Very High" }, 1)));

            #region DrawDamage

            var drawdamage = new Menu(":: Draw Damage", "drawdamage");
            {
                var dmgAfterShave =
                    new MenuItem("SurvivorKhaZix.DrawComboDamage", "Draw Damage on Enemy's HP Bar").SetValue(true);
                var drawFill =
                    new MenuItem("SurvivorKhaZix.DrawColour", "Fill Color", true).SetValue(
                        new Circle(true, System.Drawing.Color.Chartreuse));
                drawdamage.AddItem(drawFill);
                drawdamage.AddItem(dmgAfterShave);
                DrawDamage.DamageToUnit = CalculateDamage;
                DrawDamage.Enabled = dmgAfterShave.GetValue<bool>();
                DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
                DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;
                dmgAfterShave.ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                    };

                drawFill.ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
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
            Obj_AI_Base.OnProcessSpellCast += ObjAiHeroOnOnProcessSpellCast;
            Chat.Print("<font color='#800040'>[SurvivorSeries] Kha'Zix</font> <font color='#ff6600'>Loaded.</font>");

            #endregion
        }

        private void ObjAiHeroOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;

            if ((sender.IsMe && (args.SData.Name == "KhazixE")) || (args.SData.DisplayName == "KhazixE") ||
                (args.SData.Name == "KhazixELong") || (args.SData.DisplayName == "KhazixELong"))
            {
                IsMidAir = true;
                if (BoolEvolvedE)
                    LeagueSharp.Common.Utility.DelayAction.Add(1500, () => IsMidAir = false);
                else
                    LeagueSharp.Common.Utility.DelayAction.Add(1200, () => IsMidAir = false);
            }
        }

        private void SebbySpell(Spell W, Obj_AI_Base target)
        {
            var CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
            var aoe2 = false;

            if (W.Type == SkillshotType.SkillshotCircle)
            {
                CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
                aoe2 = true;
            }

            if ((W.Width > 80) && !W.Collision)
                aoe2 = true;

            var predInput2 = new PredictionInput
            {
                Aoe = aoe2,
                Collision = W.Collision,
                Speed = W.Speed,
                Delay = W.Delay,
                Range = W.Range,
                From = Player.ServerPosition,
                Radius = W.Width,
                Unit = target,
                Type = CoreType2
            };
            var poutput2 = Prediction.GetPrediction(predInput2);

            if (OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                return;

            if ((W.Speed != float.MaxValue) && OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                return;

            if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 0)
            {
                if (poutput2.Hitchance >= HitChance.Medium)
                    W.Cast(poutput2.CastPosition);
            }
            else if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 1)
            {
                if (poutput2.Hitchance >= HitChance.High)
                    W.Cast(poutput2.CastPosition);
            }
            else if (Config.Item("HitChance").GetValue<StringList>().SelectedIndex == 2)
            {
                if (poutput2.Hitchance >= HitChance.VeryHigh)
                    W.Cast(poutput2.CastPosition);
            }
        }

        private void DrawingOnOnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Config.Item("DrawQ").GetValue<bool>() && Q.IsReady())
                Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Crimson);
            if (Config.Item("DrawW").GetValue<bool>() && W.IsReady())
                Render.Circle.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Aqua);
            if (Config.Item("DrawE").GetValue<bool>() && E.IsReady())
                Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Chartreuse);

            if (Config.Item("DrawIsMidAirDebug").GetValue<bool>())
                switch (IsMidAir)
                {
                    case true:
                        {
                            var drawPos = Drawing.WorldToScreen(Player.Position);
                            var textSize = Drawing.GetTextEntent(("IsMidAir: True"), 15);
                            Drawing.DrawText(drawPos.X - textSize.Width - 70f, drawPos.Y, System.Drawing.Color.Chartreuse,
                                "IsMidAir: True");
                        }
                        break;
                    case false:
                        {
                            var drawPos = Drawing.WorldToScreen(Player.Position);
                            var textSize = Drawing.GetTextEntent(("IsMidAir: False"), 15);
                            Drawing.DrawText(drawPos.X - textSize.Width - 70f, drawPos.Y, System.Drawing.Color.DeepPink,
                                "IsMidAir: False");
                        }
                        break;
                }

            if (!Config.Item("DrawIsolated").GetValue<bool>())
                return;

            foreach (
                var enemy in
                HeroManager.Enemies.Where(
                    x => IsIsolated(x) && x.IsValidTarget() && (x.Distance(Player.ServerPosition) < 3000)))
            {
                var drawPos = Drawing.WorldToScreen(enemy.Position);
                var textSize = Drawing.GetTextEntent(("Isolated!"), 15);
                Drawing.DrawText(drawPos.X - textSize.Width / 2f, drawPos.Y, System.Drawing.Color.Chartreuse, "Isolated!");
            }
        }

        private void EvolvedSpells()
        {
            if (!BoolEvolvedQ && Player.HasBuff("khazixqevo"))
            {
                Q.Range = 375;
                BoolEvolvedQ = true;
            }
            if (!BoolEvolvedW && Player.HasBuff("khazixwevo"))
            {
                W.Width = 103f;
                BoolEvolvedW = true;
            }

            if (!BoolEvolvedE && Player.HasBuff("khazixeevo"))
            {
                E.Range = 900;
                BoolEvolvedE = true;
            }
        }

        private bool IsInvisible()
        {
            return Player.HasBuff("khazixrstealth");
        }

        private void GameOnUpdate(EventArgs args)
        {
            EvolvedSpells();
            if (Player.IsDead || Player.IsRecalling())
                return;

            Orbwalker.SetAttack(!IsInvisible());
            KillStealCheck();
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
            }
        }

        private void KillStealCheck()
        {
            if (Config.Item("EnableKS").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if ((target == null) || !target.IsValidTarget())
                    return;

                if (Config.Item("KSQ").GetValue<bool>() && Q.Instance.IsReady() &&
                    (target.Health < GetRealQDamage(target) + OktwCommon.GetIncomingDamage(target)))
                    Q.CastOnUnit(target);
                if (Config.Item("KSW").GetValue<bool>() && W.Instance.IsReady() &&
                    (target.Health < OktwCommon.GetKsDamage(target, W)))
                    SebbySpell(W, target);
                if (Config.Item("KSE").GetValue<bool>() && E.Instance.IsReady() &&
                    (target.Health < OktwCommon.GetKsDamage(target, E)))
                    E.Cast(target.ServerPosition);
                /*if (Config.Item("KSIgnite").GetValue<bool>() && Ignite.Slot != SpellSlot.Unknown && Player.Spellbook.GetSpell(Ignite.Slot).State == SpellState.Ready &&
                    target.Health < OktwCommon.GetKsDamage(target, Ignite))
                    Player.Spellbook.CastSpell(Ignite.Slot, target);
                if (Config.Item("KSSmite").GetValue<bool>() && Smite.Slot != SpellSlot.Unknown &&
                    target.Health < OktwCommon.GetKsDamage(target, Smite))
                    Player.Spellbook.CastSpell(Smite.Slot, target);*/
            }
        }

        private void Combo()
        {
            var UseQ = Config.Item("ComboUseQ").GetValue<bool>();
            var UseW = Config.Item("ComboUseW").GetValue<bool>();
            var UseE = Config.Item("ComboUseE").GetValue<bool>();
            var UseR = Config.Item("ComboUseR").GetValue<bool>();
            var DontEUnderTurret = Config.Item("ComboDontEUnderTurret").GetValue<bool>();
            var ComboMinimumREnemies = Config.Item("ComboMinimumREnemies").GetValue<Slider>().Value;

            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if ((target == null) || !target.IsValidTarget())
                return;

            if (UseR && (Player.CountEnemiesInRange(E.Range) >= ComboMinimumREnemies) && R.Instance.IsReady())
                R.Cast();

            if (UseE && E.Instance.IsReady() && !Q.IsInRange(target))
            {
                if (target.UnderTurret() && DontEUnderTurret)
                    return;
                E.Cast(target.Position);
            }
            if (UseQ && Q.Instance.IsReady())
                Q.CastOnUnit(target);
            if ((IsMidAir && target.IsValidTarget(Hydra.Range)) || target.IsValidTarget(Tiamat.Range) ||
                target.IsValidTarget(TitanicHydra.Range))
            {
                if (Hydra.IsReady())
                    Hydra.Cast();
                if (TitanicHydra.IsReady())
                    TitanicHydra.Cast();
                if (Tiamat.IsReady())
                    Tiamat.Cast();
            }
            if (UseW && W.Instance.IsReady())
                SebbySpell(W, target);

            if (Youmuu.IsReady() && target.IsValidTarget(Player.AttackRange + 400))
                Youmuu.Cast();
            if (Hydra.IsReady() && target.IsValidTarget(Hydra.Range))
                Hydra.Cast();
            if (TitanicHydra.IsReady() && target.IsValidTarget(TitanicHydra.Range))
                TitanicHydra.Cast();
            if (Tiamat.IsReady() && target.IsValidTarget(Tiamat.Range))
                Tiamat.Cast();
        }

        private void HarassMode()
        {
            var UseQ = Config.Item("HarassQ").GetValue<bool>();
            var UseW = Config.Item("HarassW").GetValue<bool>();
            var HarassManaManager = Config.Item("HarassManaManager").GetValue<Slider>().Value;

            if (Player.ManaPercent < HarassManaManager)
                return;

            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

            if ((target == null) || !target.IsValidTarget())
                return;

            if (UseQ && Q.Instance.IsReady() && target.IsValidTarget(Q.Range))
                Q.CastOnUnit(target);

            if (UseW && W.Instance.IsReady() && target.IsValidTarget(W.Range))
                SebbySpell(W, target);
        }

        private void LastHit()
        {
            var UseQ = Config.Item("LastHitQ").GetValue<bool>();
            var UseW = Config.Item("LastHitW").GetValue<bool>();
            var LastHitManaManager = Config.Item("LastHitManaManager").GetValue<Slider>().Value;

            if (Player.ManaPercent < LastHitManaManager)
                return;

            var minion = Cache.GetMinions(Player.Position, W.Range, MinionTeam.Enemy).FirstOrDefault();
            if ((minion == null) || !minion.IsValidTarget())
                return;

            if (UseQ && Q.Instance.IsReady() && (minion.Health < Q.GetDamage(minion)))
                Q.CastOnUnit(minion);

            if (UseW && W.Instance.IsReady() && (minion.Health < W.GetDamage(minion)))
                W.Cast(minion.Position);
        }

        private void LaneClear()
        {
            var UseQ = Config.Item("LaneClearQ").GetValue<bool>();
            var UseW = Config.Item("LaneClearW").GetValue<bool>();
            var UseE = Config.Item("LaneClearE").GetValue<bool>();
            var UseItems = Config.Item("LaneClearItems").GetValue<bool>();
            var LaneClearManaManager = Config.Item("LaneClearManaManager").GetValue<Slider>().Value;
            var MinimumEMinions = Config.Item("MinimumEMinions").GetValue<Slider>().Value;

            var minionsq =
                Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Enemy)
                    .OrderByDescending(x => x.Distance(Player.Position))
                    .FirstOrDefault();

            if (UseItems)
            {
                if (Hydra.IsReady() && minionsq.IsValidTarget(Hydra.Range))
                    Hydra.Cast();

                if (Tiamat.IsReady() && minionsq.IsValidTarget(Tiamat.Range))
                    Tiamat.Cast();
            }

            if (Player.ManaPercent < LaneClearManaManager)
                return;

            var minionselist = Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Enemy);
            var minionsw = minionselist.OrderByDescending(x => x.Distance(Player.Position)).FirstOrDefault();
            var minionse = E.GetCircularFarmLocation(minionselist);

            if (UseQ && Q.Instance.IsReady() && minionsq.IsValidTarget() && (minionsq != null) &&
                (minionsq.Health < GetRealQDamage(minionsq)))
                Q.CastOnUnit(minionsq);

            if (UseW && W.Instance.IsReady() && minionsw.IsValidTarget() && (minionsw != null) &&
                (minionsw.Health < W.GetDamage(minionsw)))
                W.Cast(minionsw.ServerPosition);

            if ((minionse.MinionsHit >= MinimumEMinions) && UseE &&
                E.Instance.IsReady())
                E.Cast(minionse.Position);
        }

        private void JungleClear()
        {
            var UseQ = Config.Item("JungleClearQ").GetValue<bool>();
            var UseW = Config.Item("JungleClearW").GetValue<bool>();
            var UseE = Config.Item("JungleClearE").GetValue<bool>();
            var UseItems = Config.Item("JungleClearItems").GetValue<bool>();
            var JungleClearManaManager = Config.Item("JungleClearManaManager").GetValue<Slider>().Value;
            var MinimumEJungleMobs = Config.Item("MinimumEJungleMobs").GetValue<Slider>().Value;

            var junglemobsq =
                Cache.GetMinions(Player.Position, Q.Range, MinionTeam.Neutral)
                    .OrderByDescending(x => x.MaxHealth)
                    .FirstOrDefault();

            if (UseItems)
            {
                if (Hydra.IsReady() && junglemobsq.IsValidTarget(Hydra.Range))
                    Hydra.Cast();

                if (Tiamat.IsReady() && junglemobsq.IsValidTarget(Tiamat.Range))
                    Tiamat.Cast();
            }

            if (Player.ManaPercent < JungleClearManaManager)
                return;

            var minionselist = Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Neutral);
            var minionsw = minionselist.OrderByDescending(x => x.MaxHealth).FirstOrDefault();
            var minionse = E.GetCircularFarmLocation(minionselist);
            if (minionsw == null)
                return;

            if (Config.Item("JungleClearDontEQRange").GetValue<bool>() && (Player.Distance(minionse.Position) > Q.Range))
            {
                if ((minionse.MinionsHit >= MinimumEJungleMobs) && UseE && E.Instance.IsReady())
                    E.Cast(minionse.Position);
            }
            else if (!Config.Item("JungleClearDontEQRange").GetValue<bool>())
            {
                if ((minionse.MinionsHit >= MinimumEJungleMobs) && UseE && E.Instance.IsReady())
                    E.Cast(minionse.Position);
            }

            if (junglemobsq.IsValidTarget() && UseQ && Q.Instance.IsReady())
                Q.CastOnUnit(junglemobsq);

            if (minionsw.IsValidTarget() && UseW && W.Instance.IsReady())
                W.Cast(minionsw.Position);
        }

        private bool IsIsolated(Obj_AI_Base enemy)
        {
            return
                !ObjectManager.Get<Obj_AI_Base>()
                    .Any(
                        x =>
                            (x.NetworkId != enemy.NetworkId) && (x.Team == enemy.Team) && (x.Distance(enemy) <= 500) &&
                            ((x.Type == GameObjectType.AIHeroClient) || (x.Type == GameObjectType.obj_AI_Minion) ||
                             (x.Type == GameObjectType.obj_AI_Turret)));
        }

        private double GetRealQDamage(Obj_AI_Base enemy)
        {
            if (Q.Range < 326)
                return 0.984 * Player.GetSpellDamage(enemy, SpellSlot.Q, IsIsolated(enemy) ? 1 : 0);
            if (Q.Range > 325)
            {
                var isolated = IsIsolated(enemy);
                if (isolated)
                    return 0.984 * Player.GetSpellDamage(enemy, SpellSlot.Q, 3);
                return Player.GetSpellDamage(enemy, SpellSlot.Q, 0);
            }
            return 0;
        }

        private float CalculateDamage(Obj_AI_Base enemy)
        {
            double damage = 0;

            if (Q.Instance.IsReady())
                damage += GetRealQDamage(enemy);

            if (E.Instance.IsReady())
                damage += E.GetDamage(enemy);

            if (W.Instance.IsReady())
                damage += W.GetDamage(enemy);

            if (Tiamat.IsReady())
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Tiamat);

            else if (Hydra.IsReady())
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Hydra);

            return (float)damage;
        }
    }
}