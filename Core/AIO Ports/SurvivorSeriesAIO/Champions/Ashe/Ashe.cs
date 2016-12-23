/*
 * 
 * SurvivorSeries Ashe (AIO Addon/Plugin)
 * 
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using SurvivorSeriesAIO.Core;
using SurvivorSeriesAIO.SurvivorMain;
using SurvivorSeriesAIO.Utility;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Champions
{
    internal class Ashe : ChampionBase
    {
        public List<Spell> SpellList = new List<Spell>();

        public Ashe(IRootMenu menu, Orbwalking.Orbwalker Orbwalker)
            : base(menu, Orbwalker)
        {
            // manual override - default is spell values from LeagueSharp.Data
            W.SetSkillshot(0.5f, 20f, 2000f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.5f, 300f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 130f, 1600f, false, SkillshotType.SkillshotLine);
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            //AntiGapcloser.OnEnemyGapcloser += this.AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;

            Config = new Configuration(menu.Champion);
            InitHpbarOverlay();
        }

        public Configuration Config { get; }

        private void InitHpbarOverlay()
        {
            DrawDamage.DamageToUnit = CalculateDamage;
            DrawDamage.Enabled = () => Config.DrawComboDamage.GetValue<bool>();
            DrawDamage.Fill = () => Config.FillColor.GetValue<Circle>().Active;
            DrawDamage.FillColor = () => Config.FillColor.GetValue<Circle>().Color;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            // Accurate Draw AutoAttack
            if (Config.drawAA.GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, Orbwalking.GetRealAutoAttackRange(null), Color.Gold);
            if (Config.drawW.GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.SkyBlue);
        }

        public void Obj_AI_Base_OnProcessSpell(Obj_AI_Base enemy, GameObjectProcessSpellCastEventArgs Spell)
        {
            if (!Config.EOnFlash.GetValue<bool>() || (enemy.Team == Player.Team))
                return;

            if ((Spell.SData.Name.ToLower() == "summonerflash") && (enemy.Distance(Player.Position) < 2200))
                E.Cast(Spell.End);
        }

        /*private void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe)
            {
                if (Orbwalking.InAutoAttackRange(target))
                {
                    switch (Orbwalker.ActiveMode)
                    {
                        case Orbwalking.OrbwalkingMode.LaneClear:
                            {
                                if (Q.IsReady() && Config.Item("LaneClearUseQ").GetValue<bool>())
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

        private void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
                return;

            if (Config.UseSkin.GetValue<bool>())
                //Player.SetSkin(Player.CharData.BaseSkinName, Config.SkinID.GetValue<Slider>().Value);

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (Player.ManaPercent > Config.HarassManaManager.GetValue<Slider>().Value)
                        Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    JungleClear();
                    if (Player.ManaPercent > Config.LaneClearManaManager.GetValue<Slider>().Value)
                        LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    KSAshe();
                    break;
            }

            if (Config.InstaRSelectedTarget.GetValue<KeyBind>().Active)
                InstantlyR();

            if (Config.BuyBlueTrinket.GetValue<bool>() && (Player.Level >= 6) && Player.InShop() &&
                !(Items.HasItem(3342) || Items.HasItem(3363)))
                Shop.BuyItem(ItemId.Farsight_Alteration);
        }

        /*private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.IsReady())
            {
                var GapCloser = gapcloser.Sender;
                if (Config.Item("GapCloserEnemies" + GapCloser.ChampionName).GetValue<bool>() && GapCloser.IsValidTarget(750))
                {
                    R.Cast(GapCloser.ServerPosition);
                }
            }
        }*/

        private void Interrupter2_OnInterruptableTarget(AIHeroClient t, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Config.RToInterrupt.GetValue<bool>() && t.IsValidTarget(2400) && R.IsReady())
                R.Cast(t);
        }

        private void InstantlyR()
        {
            var target = TargetSelector.GetSelectedTarget();
            if ((target == null) || !target.IsValidTarget())
                target = TargetSelector.GetTarget(1400, TargetSelector.DamageType.Physical);

            if (target.IsValidTarget() && (target != null))
                if ((Player.Mana > R.Instance.SData.Mana) && !target.HasBuff("rebirth"))
                    SpellCast.SebbySpellMain(R, target);
        }

        private void KSAshe()
        {
            if (Config.KillSteal.GetValue<bool>())
            {
                var target = Orbwalker.GetTarget() as AIHeroClient;

                if (target.IsValidTarget(W.Range) &&
                    (target.Health < W.GetDamage(target) + R.GetDamage(target) + OktwCommon.GetIncomingDamage(target)))
                {
                    if (!OktwCommon.IsSpellHeroCollision(target, R))
                        SpellCast.SebbySpellMain(R, target);
                    if (!target.CanMove || target.IsStunned)
                        SpellCast.SebbySpellMain(W, target);
                }
                if (target.IsValidTarget(1600) &&
                    (target.Health < R.GetDamage(target) + OktwCommon.GetIncomingDamage(target)))
                    if (!OktwCommon.IsSpellHeroCollision(target, R))
                        SpellCast.SebbySpellMain(R, target);
                if (target.IsValidTarget(W.Range) &&
                    (target.Health < W.GetDamage(target) + OktwCommon.GetIncomingDamage(target)))
                    if (target.CanMove)
                        SpellCast.SebbySpellMain(W, target);
                    else
                        SpellCast.SebbySpellMain(W, target);
                // More to be Added
            }
        }

        private void Combo()
        {
            // If target's champion target him, if it's not target surrounding enemy [creep, etc]
            var target = Orbwalker.GetTarget() as AIHeroClient;
            if (target == null)
                target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

            if ((target != null) && target.IsValidTarget())
            {
                if ((Config.UseQ.GetValue<bool>() && Q.IsReady() &&
                     (Player.Mana > Q.Instance.SData.Mana + R.Instance.SData.Mana)) ||
                    (target.Health < 5*Player.GetAutoAttackDamage(Player)))
                    Q.Cast();
                if (Config.UseW.GetValue<bool>() && W.IsReady() &&
                    (Player.Mana > W.Instance.SData.Mana + R.Instance.SData.Mana) && target.IsValidTarget(W.Range))
                    SpellCast.SebbySpellMain(W, target);
                foreach (
                    var ulttarget in
                    HeroManager.Enemies.Where(ulttarget => target.IsValidTarget(2000) && OktwCommon.ValidUlt(ulttarget))
                )
                    if (Config.UseR.GetValue<bool>() && target.IsValidTarget(W.Range) &&
                        (target.Health <
                         W.GetDamage(target) + R.GetDamage(target) + 3*Player.GetAutoAttackDamage(target) +
                         OktwCommon.GetIncomingDamage(target)) && !target.HasBuff("rebirth"))
                    {
                        SpellCast.SebbySpellMain(R, target);
                        if (Q.IsReady())
                            Q.Cast();
                    }
            }
        }

        private void Harass()
        {
            if (Player.ManaPercent < Config.HarassManaManager.GetValue<Slider>().Value)
                return;

            var SpellQ = Config.HarassQ.GetValue<bool>();
            var SpellW = Config.HarassW.GetValue<bool>();
            var target = Orbwalker.GetTarget() as AIHeroClient;
            if (target == null)
                target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

            if ((target != null) && target.IsValidTarget())
            {
                if (target.IsValidTarget(W.Range) && SpellW)
                    SpellCast.SebbySpellMain(W, target);

                if (Q.IsReady() && SpellQ)
                    Q.Cast();
            }
        }

        private void LaneClear()
        {
            if (Player.ManaPercent < Config.LaneClearManaManager.GetValue<Slider>().Value)
                return;

            var SpellQ = Config.laneclearQ.GetValue<bool>();
            var SpellW = Config.laneclearW.GetValue<bool>();
            var allMinionsW = MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.Enemy);
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

        private void JungleClear()
        {
            if (Player.ManaPercent < Config.JungleClearManaManager.GetValue<Slider>().Value)
                return;

            var SpellQ = Config.jungleclearQ.GetValue<bool>();
            var SpellW = Config.jungleclearW.GetValue<bool>();
            var allMinionsW = MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.Neutral);
            if (allMinionsW.Count > 0)
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

        private float CalculateDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (W.IsReady() && (Player.Mana > W.Instance.SData.Mana))
                damage += W.GetDamage(enemy);
            if (R.IsReady() && (Player.Mana > R.Instance.SData.Mana))
                damage += R.GetDamage(enemy);
            damage += (float) Player.GetAutoAttackDamage(enemy);

            return damage;
        }

        public class Configuration
        {
            public Configuration(Menu root)
            {
                ComboMenu = MenuFactory.CreateMenu(root, "Combo");
                HarassMenu = MenuFactory.CreateMenu(root, "Harass");
                LaneClearMenu = MenuFactory.CreateMenu(root, "Lane Clear");
                JungleClearMenu = MenuFactory.CreateMenu(root, "Jungle Clear");
                MiscMenu = MenuFactory.CreateMenu(root, "Misc");
                SkinMenu = MenuFactory.CreateMenu(root, "Skin Menu");
                UltimateMenu = MenuFactory.CreateMenu(root, "Ultimate Menu");
                DrawingMenu = MenuFactory.CreateMenu(root, "Drawing");

                Combos(MenuItemFactory.Create(ComboMenu));
                Harass(MenuItemFactory.Create(HarassMenu));
                LaneClear(MenuItemFactory.Create(LaneClearMenu));
                JungleClear(MenuItemFactory.Create(JungleClearMenu));
                Misc(MenuItemFactory.Create(MiscMenu));
                Skins(MenuItemFactory.Create(SkinMenu));
                Ultimate(MenuItemFactory.Create(UltimateMenu));
                Drawings(MenuItemFactory.Create(DrawingMenu));
            }

            public Menu ComboMenu { get; }
            public Menu HarassMenu { get; }
            public Menu LaneClearMenu { get; }
            public Menu JungleClearMenu { get; }
            public Menu MiscMenu { get; }
            public Menu DrawingMenu { get; }
            public Menu UltimateMenu { get; }
            public MenuItem EOnFlash { get; private set; }
            public MenuItem BuyBlueTrinket { get; private set; }
            public MenuItem KillSteal { get; private set; }
            public MenuItem InstaRSelectedTarget { get; private set; }
            public MenuItem JungleClearManaManager { get; private set; }
            public MenuItem jungleclearW { get; private set; }
            public MenuItem jungleclearQ { get; private set; }
            public MenuItem LaneClearManaManager { get; private set; }
            public MenuItem laneclearW { get; private set; }
            public MenuItem laneclearQ { get; private set; }
            public MenuItem HarassManaManager { get; private set; }
            public MenuItem HarassW { get; private set; }
            public MenuItem HarassQ { get; private set; }
            public MenuItem FillColor { get; private set; }
            public MenuItem DrawComboDamage { get; private set; }
            public MenuItem drawW { get; private set; }
            public MenuItem drawAA { get; private set; }
            public MenuItem UseR { get; private set; }
            public MenuItem UseW { get; private set; }
            public MenuItem UseQ { get; private set; }
            public MenuItem RToInterrupt { get; private set; }
            public MenuItem SkinID { get; private set; }
            public MenuItem UseSkin { get; private set; }
            public Menu SkinMenu { get; }

            private void Combos(MenuItemFactory factory)
            {
                UseQ = factory.WithName("Use Q").WithValue(true).Build();
                UseW = factory.WithName("Use W").WithValue(true).Build();
                UseR = factory.WithName("Use R").WithValue(true).Build();
            }

            private void Drawings(MenuItemFactory factory)
            {
                // Drawing Menu
                drawAA = factory.WithName("Draw AA range").WithValue(true).Build();
                drawW = factory.WithName("Draw W range").WithValue(true).Build();
                DrawComboDamage = factory
                    .WithName("Draw Combo Damage")
                    .WithValue(true)
                    .Build();

                FillColor = factory
                    .WithName("Fill Color")
                    .WithValue(new Circle(true, Color.FromArgb(204, 255, 0, 1)))
                    .Build();
            }

            private void Skins(MenuItemFactory factory)
            {
                // Skins Menu
                SkinID = factory.WithName("Skin ID").WithValue(new Slider(8, 0, 8)).Build();
                UseSkin = factory.WithName("Enabled?").WithValue(true).Build();
                UseSkin.ValueChanged += (sender, eventArgs) =>
                {
                };
            }

            private void Harass(MenuItemFactory factory)
            {
                HarassQ = factory.WithName("Use Q").WithValue(true).Build();
                HarassW = factory.WithName("Use W").WithValue(true).Build();
                HarassManaManager = factory
                    .WithName("Minimum Mana%")
                    .WithValue(new Slider(30))
                    .WithTooltip("Minimum Mana that you need to have to Harass with Q/W.")
                    .Build();
            }

            private void LaneClear(MenuItemFactory factory)
            {
                // LaneClear Menu
                laneclearQ = factory.WithName("Use Q to LaneClear").WithValue(true).Build();
                laneclearW = factory.WithName("Use W to LaneClear").WithValue(true).Build();

                LaneClearManaManager = factory
                    .WithName("Minimum Mana%")
                    .WithValue(new Slider(30))
                    .WithTooltip("Minimum Mana that you need to have to LaneClear with Q/W.")
                    .Build();
            }

            private void JungleClear(MenuItemFactory factory)
            {
                jungleclearQ = factory.WithName("Use Q").WithValue(true).Build();
                jungleclearW = factory.WithName("Use W").WithValue(true).Build();

                JungleClearManaManager = factory
                    .WithName("Minimum Mana%")
                    .WithValue(new Slider(30))
                    .WithTooltip("Minimum Mana that you need to have to JungleClear with Q/W.")
                    .Build();
            }

            private void Ultimate(MenuItemFactory factory)
            {
                InstaRSelectedTarget =
                    factory.WithName("Instantly Ult [Selected Target] or Nearby")
                        .WithValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))
                        .WithTooltip("It'll Use the Ultimate if there's Enemy selected or enemy nearby.")
                        .WithPerma("[Ashe]Insta Ult")
                        .Build();
            }

            private void Misc(MenuItemFactory factory)
            {
                // Misc Menu
                // Todo: Add more KillSteal Variants/Spells
                // foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                // ConfigMenu.SubMenu("Misc Menu").SubMenu("GapCloser R").AddItem(new MenuItem("gapcloserR" + enemy.ChampionName, enemy.ChampionName).SetValue(false).SetTooltip("Use R on GapClosing Champions"));
                KillSteal = factory.WithName("Activate KillStealer?").WithValue(true).Build();
                BuyBlueTrinket = factory.WithName("Buy Blue Trinket on level 6?").WithValue(true).Build();
                EOnFlash = factory.WithName("Cast E if Enemy Player flashes?").WithValue(true).Build();
                RToInterrupt = factory.WithName("Auto R to interrupt enemies?").WithValue(true).Build();
                // Gapcloser fix/add
            }
        }
    }
}