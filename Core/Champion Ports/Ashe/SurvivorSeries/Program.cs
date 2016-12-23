// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs.cs" company="SurvivorAshe">
//      Copyright (c) SurvivorAshe. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using Color = SharpDX.Color;
using HitChance = SebbyLib.Prediction.HitChance;

using Prediction = SebbyLib.Prediction.Prediction;
using PredictionInput = SebbyLib.Prediction.PredictionInput;

using EloBuddy; 
using LeagueSharp.Common; 
namespace SurvivorAshe
{
    internal class Program
    {
        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != ChampionName)
                return;

            #region Spells

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1240);
            E = new Spell(SpellSlot.E, 2500);
            R = new Spell(SpellSlot.R, float.MaxValue);

            W.SetSkillshot(0.5f, 20f, 2000f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.5f, 300f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 130f, 1600f, false, SkillshotType.SkillshotLine);

            #endregion

            #region Menu

            Menu = new Menu("SurvivorAshe", "SurvivorAshe", true);

            var OrbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(OrbwalkerMenu);

            var TargetSelectorMenu = Menu.AddSubMenu(new Menu("Target Selector", "TargetSelector"));
            TargetSelector.AddToMenu(TargetSelectorMenu);

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            ComboMenu.AddItem(new MenuItem("ComboUseQ", "Use Q").SetValue(true));
            ComboMenu.AddItem(new MenuItem("ComboUseW", "Use W").SetValue(true));
            ComboMenu.AddItem(
                new MenuItem("ComboUseR", "Use R").SetValue(true)
                    .SetTooltip("Will use R if there's target that's killable in (W + R + 3 AA) Damage"));

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            HarassMenu.AddItem(new MenuItem("HarassUseQ", "Use Q").SetValue(true));
            HarassMenu.AddItem(new MenuItem("HarassUseW", "Use W").SetValue(true));
            HarassMenu.AddItem(new MenuItem("HarassManaManager", "Mana Manager (%)").SetValue(new Slider(40, 1, 100)));

            var LaneClearMenu = Menu.AddSubMenu(new Menu("Lane Clear", "LaneClear"));
            LaneClearMenu.AddItem(new MenuItem("LaneClearUseQ", "Use Q").SetValue(true));
            LaneClearMenu.AddItem(new MenuItem("LaneClearUseW", "Use W").SetValue(true));
            LaneClearMenu.AddItem(
                new MenuItem("LaneClearManaManager", "Mana Manager (%)").SetValue(new Slider(40, 1, 100)));

            var JungleClearMenu = Menu.AddSubMenu(new Menu("Jungle Clear", "JungleClear"));
            JungleClearMenu.AddItem(new MenuItem("UseQJC", "Use Q").SetValue(true));
            JungleClearMenu.AddItem(new MenuItem("UseWJC", "Use W").SetValue(true));
            JungleClearMenu.AddItem(
                new MenuItem("JungleClearManaManager", "Mana Manager (%)").SetValue(new Slider(40, 1, 100)));

            var UltimateMenu = Menu.AddSubMenu(new Menu("Ultimate Menu", "UltimateMenu"));
            UltimateMenu.AddItem(
                    new MenuItem("InstaRSelectedTarget", "Instantly Ult [Selected Target] or Nearby").SetValue(
                            new KeyBind("T".ToCharArray()[0], KeyBindType.Press))
                        .SetTooltip("It'll Use the Ultimate if there's Enemy selected or enemy nearby."))
                .Permashow(true, "[Ashe]Insta Ult");
            UltimateMenu.AddItem(
                new MenuItem("DontUltEnemyIfAllyNearby", "Don't Ult if Ally is Near Almost Dead Enemy").SetValue(false));


            var MiscMenu = Menu.AddSubMenu(new Menu("Misc Menu", "MiscMenu"));
            MiscMenu.AddItem(new MenuItem("KillSteal", "Activate KillSteal?").SetValue(true));
            MiscMenu.AddItem(new MenuItem("BuyBlueTrinket", "Buy Blue Trinket on Lvl 6?").SetValue(true));
            MiscMenu.AddItem(new MenuItem("EOnFlash", "Cast E if Enemy Player Flashes?").SetValue(true));
            MiscMenu.AddItem(new MenuItem("RToInterrupt", "Auto R to Interrupt Enemies").SetValue(true));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                MiscMenu.SubMenu("[GapClosers] R Config")
                    .AddItem(
                        new MenuItem("GapCloserEnemies" + enemy.ChampionName, enemy.ChampionName).SetValue(false)
                            .SetTooltip("Use R on GapClosing Champions"));

            var HitChanceMenu = Menu.AddSubMenu(new Menu("HitChance Menu", "HitChance"));
            HitChanceMenu.AddItem(
                new MenuItem("HitChance", "Hit Chance").SetValue(new StringList(new[] {"Medium", "High", "Very High"}, 1)));

            var AutoLevelerMenu = Menu.AddSubMenu(new Menu("AutoLeveler Menu", "AutoLevelerMenu"));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp", "AutoLevel Up Spells?").SetValue(true));
            AutoLevelerMenu.AddItem(
                new MenuItem("AutoLevelUp1", "First: ").SetValue(new StringList(new[] {"Q", "W", "E", "R"}, 3)));
            AutoLevelerMenu.AddItem(
                new MenuItem("AutoLevelUp2", "Second: ").SetValue(new StringList(new[] {"Q", "W", "E", "R"}, 0)));
            AutoLevelerMenu.AddItem(
                new MenuItem("AutoLevelUp3", "Third: ").SetValue(new StringList(new[] {"Q", "W", "E", "R"}, 1)));
            AutoLevelerMenu.AddItem(
                new MenuItem("AutoLevelUp4", "Fourth: ").SetValue(new StringList(new[] {"Q", "W", "E", "R"}, 2)));
            AutoLevelerMenu.AddItem(
                new MenuItem("AutoLvlStartFrom", "AutoLeveler Start from Level: ").SetValue(new Slider(2, 6, 1)));

            #region Skin Changer

            var SkinChangerMenu =
                Menu.AddSubMenu(new Menu(":: Skin Changer", "SkinChanger").SetFontStyle(FontStyle.Bold, Color.Chartreuse));
            var SkinChanger =
                SkinChangerMenu.AddItem(
                    new MenuItem("UseSkinChanger", ":: Use SkinChanger?").SetValue(true)
                        .SetFontStyle(FontStyle.Bold, Color.Crimson));
            var SkinID =
                SkinChangerMenu.AddItem(
                    new MenuItem("SkinID", ":: Skin").SetValue(new Slider(8, 0, 8))
                        .SetFontStyle(FontStyle.Bold, Color.Crimson));
            SkinID.ValueChanged += (sender, eventArgs) =>
            {
                if (!SkinChanger.GetValue<bool>())
                    return;

                //Player.SetSkin(Player.BaseSkinName, eventArgs.GetNewValue<Slider>().Value);
            };

            #endregion

            var DrawingMenu = Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            DrawingMenu.AddItem(new MenuItem("DrawAA", "Draw AA Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawW", "Draw W Range").SetValue(true));

            #endregion

            #region DrawHPDamage

            var dmgAfterShave =
                DrawingMenu.AddItem(
                    new MenuItem("SurvivorAshe.DrawComboDamage", "Draw Damage on Enemy's HP Bar").SetValue(true));
            var drawFill =
                DrawingMenu.AddItem(new MenuItem("SurvivorAshe.DrawColour", "Fill Color", true).SetValue(
                    new Circle(true, System.Drawing.Color.SeaGreen)));
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

            #endregion

            Menu.AddToMainMenu();

            #region Subscriptions

            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
            Game.OnUpdate += Game_OnUpdate;
            //Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpell;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;

            #endregion

            Chat.Print("<font color='#800040'>[SurvivorSeries] Ashe</font> <font color='#ff6600'>Loaded.</font>");
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            // Accurate Draw AutoAttack
            if (Menu.Item("DrawAA").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, Orbwalking.GetRealAutoAttackRange(null),
                    System.Drawing.Color.Gold);
            if (Menu.Item("DrawW").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, W.Range, System.Drawing.Color.SkyBlue);
        }

        private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!sender.IsMe || !Menu.Item("AutoLevelUp").GetValue<bool>() ||
                (ObjectManager.Player.Level < Menu.Item("AutoLvlStartFrom").GetValue<Slider>().Value))
                return;
            if ((lvl2 == lvl3) || (lvl2 == lvl4) || (lvl3 == lvl4))
                return;
            var delay = 700;
            LeagueSharp.Common.Utility.DelayAction.Add(delay, () => LevelUp(lvl1));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 50, () => LevelUp(lvl2));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 100, () => LevelUp(lvl3));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 150, () => LevelUp(lvl4));
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

        public static void Obj_AI_Base_OnProcessSpell(Obj_AI_Base enemy, GameObjectProcessSpellCastEventArgs Spell)
        {
            if (!Menu.Item("EOnFlash").GetValue<bool>() || (enemy.Team == Player.Team))
                return;

            if ((Spell.SData.Name.ToLower() == "summonerflash") && (enemy.Distance(Player.Position) < 2200))
                E.Cast(Spell.End);
        }

        private static void SebbySpell(Spell WR, Obj_AI_Base target)
        {
            var CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
            var aoe2 = false;

            if (WR.Type == SkillshotType.SkillshotCircle)
            {
                CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
                aoe2 = true;
            }

            if ((WR.Width > 80) && !WR.Collision)
                aoe2 = true;

            var predInput2 = new PredictionInput
            {
                Aoe = aoe2,
                Collision = WR.Collision,
                Speed = WR.Speed,
                Delay = WR.Delay,
                Range = WR.Range,
                From = Player.ServerPosition,
                Radius = WR.Width,
                Unit = target,
                Type = CoreType2
            };
            var poutput2 = Prediction.GetPrediction(predInput2);

            if (OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                return;

            if ((WR.Speed != float.MaxValue) && OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                return;

            if (Menu.Item("HitChance").GetValue<StringList>().SelectedIndex == 0)
            {
                if (poutput2.Hitchance >= HitChance.Medium)
                    WR.Cast(poutput2.CastPosition);
            }
            else if (Menu.Item("HitChance").GetValue<StringList>().SelectedIndex == 1)
            {
                if (poutput2.Hitchance >= HitChance.High)
                    WR.Cast(poutput2.CastPosition);
            }
            else if (Menu.Item("HitChance").GetValue<StringList>().SelectedIndex == 2)
            {
                if (poutput2.Hitchance >= HitChance.VeryHigh)
                    WR.Cast(poutput2.CastPosition);
            }
        }

        /*private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe)
            {
                if (Orbwalking.InAutoAttackRange(target))
                {
                    switch (Orbwalker.ActiveMode)
                    {
                        case Orbwalking.OrbwalkingMode.LaneClear:
                            {
                                if (Q.IsReady() && Menu.Item("LaneClearUseQ").GetValue<bool>())
                                {
                                    var Minions = MinionManager.GetMinions(Orbwalking.GetRealAutoAttackRange(Player), MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
                                    {

                                    }
                                }
                                break;
                            }
                    }
                }
            }
        }*/

        private static void JungleClear()
        {
            if (Player.ManaPercent < Menu.Item("JungleClearManaManager").GetValue<Slider>().Value)
                return;
            var jgcq = Menu.Item("UseQJC").GetValue<bool>();
            var jgcw = Menu.Item("UseWJC").GetValue<bool>();

            var mob =
                Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Neutral).OrderBy(x => x.MaxHealth).FirstOrDefault();
            if (mob == null)
                return;

            if (jgcq && Q.IsReady() && mob.IsValidTarget())
                Q.Cast();
            if (jgcw && W.IsReady() && mob.IsValidTarget())
                W.CastOnUnit(mob);
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
                return;

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (Player.ManaPercent > Menu.Item("HarassManaManager").GetValue<Slider>().Value)
                        Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                {
                    JungleClear();
                    if (Player.ManaPercent > Menu.Item("LaneClearManaManager").GetValue<Slider>().Value)
                        LaneClear();
                }
                    break;
            }
            KSAshe();

            if (Menu.Item("InstaRSelectedTarget").GetValue<KeyBind>().Active)
                InstantlyR();

            if (Menu.Item("BuyBlueTrinket").GetValue<bool>() && (Player.Level >= 6) && Player.InShop() &&
                !(Items.HasItem(3342) || Items.HasItem(3363)))
                Shop.BuyItem(ItemId.Farsight_Alteration);

            //AutoLeveler
            if (Menu.Item("AutoLevelUp").GetValue<bool>())
            {
                lvl1 = Menu.Item("AutoLevelUp1").GetValue<StringList>().SelectedIndex;
                lvl2 = Menu.Item("AutoLevelUp2").GetValue<StringList>().SelectedIndex;
                lvl3 = Menu.Item("AutoLevelUp3").GetValue<StringList>().SelectedIndex;
                lvl4 = Menu.Item("AutoLevelUp4").GetValue<StringList>().SelectedIndex;
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.IsReady())
            {
                var GapCloser = gapcloser.Sender;
                if (Menu.Item("GapCloserEnemies" + GapCloser.ChampionName).GetValue<bool>() &&
                    GapCloser.IsValidTarget(750))
                    R.Cast(GapCloser.ServerPosition);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient t,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Menu.Item("RToInterrupt").GetValue<bool>() && t.IsValidTarget(2400) && R.IsReady())
                R.Cast(t);
        }

        private static void InstantlyR()
        {
            var target = TargetSelector.GetSelectedTarget();
            if ((target == null) || !target.IsValidTarget())
                target = TargetSelector.GetTarget(1400, TargetSelector.DamageType.Physical);

            if (target.IsValidTarget() && (target != null) && !target.HasBuff("rebirth"))
                if (Player.Mana > R.Instance.SData.Mana)
                    SebbySpell(R, target);
        }

        private static void DontREnemyNearAlly()
        {
            if (Menu.Item("DontUltEnemyIfAllyNearby").GetValue<bool>())
            {
                var enemy =
                    HeroManager.Enemies.Where(
                        x => x.IsValidTarget() && (x.HealthPercent < 20) && (x.CountAlliesInRange(500) > 0));

                if ((enemy == null) || (enemy != null))
                    return;
            }
        }

        private static void KSAshe()
        {
            if (Menu.Item("KillSteal").GetValue<bool>())
            {
                var target = Orbwalker.GetTarget() as AIHeroClient;

                if (target.IsValidTarget(W.Range) &&
                    (target.Health < W.GetDamage(target) + R.GetDamage(target) + OktwCommon.GetIncomingDamage(target)))
                {
                    DontREnemyNearAlly();
                    if (!target.CanMove || target.IsStunned)
                        W.Cast(target.Position);
                    if (!OktwCommon.IsSpellHeroCollision(target, R))
                        SebbySpell(R, target);
                }
                if (target.IsValidTarget(W.Range) &&
                    (target.Health < W.GetDamage(target) + OktwCommon.GetIncomingDamage(target)))
                    if (target.CanMove)
                        SebbySpell(W, target);
                    else
                        W.Cast(target.Position);
                if (target.IsValidTarget(1600) &&
                    (target.Health < R.GetDamage(target) + OktwCommon.GetIncomingDamage(target)))
                {
                    DontREnemyNearAlly();
                    if (!OktwCommon.IsSpellHeroCollision(target, R))
                        SebbySpell(R, target);
                }
                // More to be Added
            }
        }

        private static void Combo()
        {
            // If target's champion target him, if it's not target surrounding enemy [creep, etc]
            var target = Orbwalker.GetTarget() as AIHeroClient;
            if (target == null)
                target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

            if ((target != null) && target.IsValidTarget())
            {
                if ((Menu.Item("ComboUseQ").GetValue<bool>() &&
                     target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null)) && Q.IsReady() &&
                     (Player.Mana > Q.Instance.SData.Mana + R.Instance.SData.Mana)) ||
                    (target.Health < 5*Player.GetAutoAttackDamage(Player)))
                    Q.Cast();
                if (Menu.Item("ComboUseW").GetValue<bool>() && W.IsReady() &&
                    (Player.Mana > W.Instance.SData.Mana + R.Instance.SData.Mana) && target.IsValidTarget(W.Range))
                    SebbySpell(W, target);
                DontREnemyNearAlly();
                foreach (
                    var ulttarget in
                    HeroManager.Enemies.Where(ulttarget => target.IsValidTarget(2000) && OktwCommon.ValidUlt(ulttarget))
                )
                    if (Menu.Item("ComboUseR").GetValue<bool>() && target.IsValidTarget(W.Range) &&
                        (target.Health <
                         W.GetDamage(target) + R.GetDamage(target) + 3*Player.GetAutoAttackDamage(target) +
                         OktwCommon.GetIncomingDamage(target)) && !target.HasBuff("rebirth"))
                    {
                        SebbySpell(R, target);
                        if (Q.IsReady())
                            Q.Cast();
                    }
            }
        }

        private static void Harass()
        {
            var SpellQ = Menu.Item("HarassUseQ").GetValue<bool>();
            var SpellW = Menu.Item("HarassUseW").GetValue<bool>();
            var target = Orbwalker.GetTarget() as AIHeroClient;
            if (target == null)
                target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

            if ((target != null) && target.IsValidTarget())
            {
                if (target.IsValidTarget(W.Range) && SpellW)
                    SebbySpell(W, target);

                if (Q.IsReady() && SpellQ && target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null)))
                    Q.Cast();
            }
        }

        private static void LaneClear()
        {
            var SpellQ = Menu.Item("LaneClearUseQ").GetValue<bool>();
            var SpellW = Menu.Item("LaneClearUseW").GetValue<bool>();
            var allMinionsW = Cache.GetMinions(Player.Position, W.Range, MinionTeam.Enemy);
            if (allMinionsW.Count > 1)
                foreach (var minion in allMinionsW)
                {
                    if (!minion.IsValidTarget() || (minion == null))
                        return;
                    if (SpellW && W.IsReady())
                        W.Cast(minion);
                    if (SpellQ && Q.IsReady())
                        Q.Cast();
                }
        }

        private static float CalculateDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (W.IsReady() && (Player.Mana > W.Instance.SData.Mana))
                damage += W.GetDamage(enemy);
            if (R.IsReady() && (Player.Mana > R.Instance.SData.Mana))
                damage += R.GetDamage(enemy);
            damage += (float) Player.GetAutoAttackDamage(enemy);

            return damage;
        }

        #region Declaration

        private static Spell Q, W, E, R;
        private static Orbwalking.Orbwalker Orbwalker;
        private static Menu Menu;

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static readonly string ChampionName = "Ashe";
        private static int lvl1, lvl2, lvl3, lvl4;

        #endregion
    }
}