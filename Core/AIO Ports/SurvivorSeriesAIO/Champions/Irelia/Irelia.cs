using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using SurvivorSeriesAIO.Core;
using SurvivorSeriesAIO.Utility;
using Orbwalking = SebbyLib.Orbwalking;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Champions
{
    internal class Irelia : ChampionBase
    {
        public Items.Item
            Sheen = new Items.Item(2057),
            Trinity = new Items.Item(3078),
            Gauntlet = new Items.Item(3025),
            LichBane = new Items.Item(3100);

        public List<Spell> SpellList = new List<Spell>();

        public Irelia(IRootMenu menu, Orbwalking.Orbwalker Orbwalker) : base(menu, Orbwalker)
        {
            // manual override - default is spell values from LeagueSharp.Data
            R.SetSkillshot(0.25f, 45, 1600, false, SkillshotType.SkillshotLine);
            // Subsriptions
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnSpellCast += WUsage;
            Drawing.OnDraw += Drawing_OnDraw;

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

        private void WUsage(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.IsAutoAttack() && args.Target is AIHeroClient &&
                (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) && W.IsReady())
                W.Cast();
        }

        private void JungleClear()
        {
            if (Player.ManaPercent < Config.JungleClearManaManager.GetValue<Slider>().Value)
                return;

            var jgcq = Config.JGCQ.GetValue<bool>();
            var jgcw = Config.JGCW.GetValue<bool>();
            var jgce = Config.JGCE.GetValue<bool>();

            var mob =
                MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (mob == null)
                return;

            if (jgcq && Q.Instance.IsReady())
                Q.CastOnUnit(mob);
            if (jgce && E.Instance.IsReady())
                E.CastOnUnit(mob);
            if (jgcw && W.Instance.IsReady())
                W.Cast();
        }

        public float GetSheenDamage(Obj_AI_Base target)
        {
            float totalDamage = 0;

            if (Items.HasItem(3057, Player))
            {
                if (Items.CanUseItem(3057))
                    totalDamage +=
                        (float) Player.CalcDamage(target, Damage.DamageType.Physical, Player.TotalAttackDamage);
            }
            else if (Items.HasItem(3025, Player))
            {
                if (Items.CanUseItem(3025))
                    totalDamage +=
                        (float) Player.CalcDamage(target, Damage.DamageType.Physical, Player.TotalAttackDamage);
            }
            else if (Items.HasItem(3078, Player))
            {
                if (Items.CanUseItem(3078))
                    totalDamage +=
                        (float) Player.CalcDamage(target, Damage.DamageType.Physical, Player.TotalAttackDamage*2);
            }
            else if (Items.HasItem(3100, Player))
            {
                if (Items.CanUseItem(3100))
                    totalDamage +=
                        (float)
                        Player.CalcDamage(target, Damage.DamageType.Magical,
                            75/Player.TotalAttackDamage*100 + 50/Player.TotalMagicalDamage*100);
            }
            return totalDamage;
        }

        public void Combo()
        {
            var useq = Config.comboQ.GetValue<bool>();
            var usew = Config.comboW.GetValue<bool>();
            var usee = Config.comboE.GetValue<bool>();
            var useR = Config.comboR.GetValue<bool>();
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            //var targetextend = TargetSelector.GetTarget();
            if ((target == null) || !target.IsValidTarget())
                return;

            //Chat.Print("Q Damage: " + Q.GetDamage(target) + " Sheen Damage:" + GetSheenDamage(target));
            //var distancebetweenmeandtarget = ObjectManager.Player.ServerPosition.Distance(target.ServerPosition);
            /*if (Q.IsReady() && target.Distance(Player.Position) > 650)
            {
                var gapclosingMinion = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < 650 &&
                    m.IsEnemy && m.ServerPosition.Distance(target.ServerPosition) < distancebetweenmeandtarget && m.Health > 1 && m.Health < Q.GetDamage(m) + GetSheenDamage(m)).OrderBy(m => m.Position.Distance(target.ServerPosition)).FirstOrDefault();
                if (gapclosingMinion != null)
                {
                    Q.Cast(gapclosingMinion);
                }
            }*/

            if (target.IsValidTarget(Q.Range) && !target.IsValidTarget(Player.AttackRange) && Q.IsReady() && useq)
            {
                Q.CastOnUnit(target);
                if (E.Instance.IsReady() && target.IsValidTarget(E.Range) && usee)
                    E.CastOnUnit(target);
                if (W.Instance.IsReady() && usew && target.IsValidTarget(Player.AttackRange))
                    // Yes lads E.Range is intentionally here :gosh:
                    W.Cast();
                if (target.IsValidTarget(R.Range) && useR)
                {
                    var pred = R.GetPrediction(target);
                    if ((pred.Hitchance >= HitChance.High) && HasRBuff())
                    {
                        if (OktwCommon.CollisionYasuo(Player.Position, pred.CastPosition))
                            return;
                        R.Cast(pred.CastPosition);
                    }
                }
            }
            else if (target.IsValidTarget(E.Range))
            {
                if (Q.Instance.IsReady() && useq)
                    Q.CastOnUnit(target);
                if (E.Instance.IsReady() && usee && target.IsValidTarget(E.Range))
                    E.CastOnUnit(target);
                if (W.Instance.IsReady() && usew && target.IsValidTarget(Player.AttackRange))
                    // Yes lads E.Range is intentionally here :gosh:
                    W.Cast();
                if (R.Instance.IsReady() && useR && target.IsValidTarget(R.Range))
                {
                    var pred = R.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        if (OktwCommon.CollisionYasuo(Player.Position, pred.CastPosition))
                            return;
                        R.Cast(pred.CastPosition);
                    }
                }
            }
        }

        public void LaneClear()
        {
            if (Player.ManaPercent < Config.LaneClearManaManager.GetValue<Slider>().Value)
                return;
            var lcq = Config.LCQ.GetValue<bool>();
            var lcw = Config.LCW.GetValue<bool>();
            var lce = Config.LCE.GetValue<bool>();
            var lcwslider = Config.LCWSlider.GetValue<Slider>().Value;

            var minionq =
                Cache.GetMinions(Player.Position, Q.Range)
                    .FirstOrDefault(x => Q.GetDamage(x) + GetSheenDamage(x) > x.Health);

            var minionwe = Cache.GetMinions(Player.Position, 275);

            if (minionq == null)
                return;

            if (lcq && Q.Instance.IsReady())
                Q.CastOnUnit(minionq);

            if (minionwe == null)
                return;

            foreach (var minion in minionwe)
            {
                if (W.Instance.IsReady() && lcw && (minion.HealthPercent > 5) && (minionwe.Count > lcwslider))
                    W.Cast();
                if (E.Instance.IsReady() && lce && (minion.Health < E.GetDamage(minion)) && lce)
                    E.CastOnUnit(minion);
            }
        }

        public void Harass()
        {
            if (Player.ManaPercent < Config.HarassManaManager.GetValue<Slider>().Value)
                return;

            var harassq = Config.harassQ.GetValue<bool>();
            var harasse = Config.harassE.GetValue<bool>();

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if ((target == null) || !target.IsValidTarget())
                return;

            if (Q.IsReady() && harassq)
            {
                Q.CastOnUnit(target);
                if (harasse && E.IsReady())
                {
                    E.CastOnUnit(target);
                    Orbwalker.ForceTarget(target);
                }
                else
                {
                    Orbwalker.ForceTarget(target);
                }
            }

            if (harasse && E.IsReady() && !Q.IsReady())
                if (target.IsValidTarget(E.Range))
                {
                    E.CastOnUnit(target);
                    Orbwalker.ForceTarget(target);
                }
        }

        private void KSChecking()
        {
            if (Config.KSCheck.GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

                if ((target == null) || !target.IsValidTarget())
                    return;

                if ((target.Health < Q.GetDamage(target) + GetSheenDamage(target)) && Q.IsReady())
                    Q.CastOnUnit(target);

                if ((target.Health < E.GetDamage(target)) && E.IsReady())
                    E.CastOnUnit(target);
            }
        }

        public bool HasRBuff() => Player.HasBuff("ireliatranscendentbladesspell");

        public bool Stunnable(AIHeroClient enemy)
        {
            return enemy.HealthPercent > Player.HealthPercent;
        }

        private float CalculateDamage(AIHeroClient enemy)
        {
            float damage = 0;

            if (Q.Instance.IsReady() && (Player.Mana > Q.Instance.SData.Mana))
                damage += Q.GetDamage(enemy) + GetSheenDamage(enemy);
            if (W.Instance.IsReady() && (Player.Mana > W.Instance.SData.Mana))
                damage += W.GetDamage(enemy);
            if (E.Instance.IsReady() && (Player.Mana > E.Instance.SData.Mana))
                damage += E.GetDamage(enemy);
            if (HasRBuff() || R.Instance.IsReady())
                damage += R.GetDamage(enemy);
            damage += (float) Player.GetAutoAttackDamage(enemy);

            return damage;
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Config.DrawAA.GetValue<bool>())
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Orbwalking.GetRealAutoAttackRange(null),
                    Color.BlueViolet);
            if (Config.DrawQ.GetValue<bool>())
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Chartreuse);
            if (Config.DrawR.GetValue<bool>() && (R.Level > 0) && R.Instance.IsReady())
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.DeepPink);

            if (Config.QMinionDrawHelper.GetValue<bool>())
                foreach (
                    var creature in
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            x =>
                                x.Name.ToLower().Contains("minion") && x.IsHPBarRendered && x.IsValidTarget(1000) &&
                                (x.Health < Q.GetDamage(x) + GetSheenDamage(x)) && x.IsEnemy))
                    Render.Circle.DrawCircle(creature.Position, 35, Color.Chartreuse);

            if (!Config.DrawStunnable.GetValue<bool>())
                return;

            foreach (var enemy in
                ObjectManager.Get<AIHeroClient>().Where(x => Stunnable(x) && x.IsValidTarget()))
            {
                var drawPos = Drawing.WorldToScreen(enemy.Position);
                var textSize = Drawing.GetTextEntent(("Stunnable"), 15);
                Drawing.DrawText(drawPos.X - textSize.Width/2f, drawPos.Y, Color.DeepPink, "Stunnable");
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    JungleClear();
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }
            KSChecking();
        }

        public class Configuration
        {
            public Configuration(Menu root)
            {
                ComboMenu = MenuFactory.CreateMenu(root, ":: Combo");
                HarassMenu = MenuFactory.CreateMenu(root, ":: Harass");
                LaneClearMenu = MenuFactory.CreateMenu(root, ":: Lane Clear");
                JungleClearMenu = MenuFactory.CreateMenu(root, ":: Jungle Clear");
                MiscMenu = MenuFactory.CreateMenu(root, ":: Misc");
                DrawingMenu = MenuFactory.CreateMenu(root, ":: Drawing");

                Combo(MenuItemFactory.Create(ComboMenu));
                Harass(MenuItemFactory.Create(HarassMenu));
                LaneClear(MenuItemFactory.Create(LaneClearMenu));
                JungleClear(MenuItemFactory.Create(JungleClearMenu));
                Misc(MenuItemFactory.Create(MiscMenu));
                Drawings(MenuItemFactory.Create(DrawingMenu));
            }

            public Menu DrawingMenu { get; set; }

            public Menu MiscMenu { get; set; }

            public Menu JungleClearMenu { get; set; }

            public Menu LaneClearMenu { get; set; }

            public Menu HarassMenu { get; set; }

            public Menu ComboMenu { get; set; }

            public MenuItem comboR { get; set; }

            public MenuItem comboE { get; set; }

            public MenuItem comboW { get; set; }

            public MenuItem comboQ { get; set; }

            public MenuItem LaneClearManaManager { get; set; }

            public MenuItem LCE { get; set; }

            public MenuItem LCWSlider { get; set; }

            public MenuItem LCW { get; set; }

            public MenuItem LCQ { get; set; }

            public MenuItem JungleClearManaManager { get; set; }

            public MenuItem JGCE { get; set; }

            public MenuItem JGCW { get; set; }

            public MenuItem JGCQ { get; set; }

            public MenuItem HarassManaManager { get; set; }

            public MenuItem harassE { get; set; }

            public MenuItem harassQ { get; set; }

            public MenuItem FillColor { get; set; }

            public MenuItem DrawComboDamage { get; set; }

            public MenuItem QMinionDrawHelper { get; set; }

            public MenuItem DrawStunnable { get; set; }

            public MenuItem DrawR { get; set; }

            public MenuItem DrawQ { get; set; }

            public MenuItem DrawAA { get; set; }

            public MenuItem KSCheck { get; set; }

            private void Combo(MenuItemFactory factory)
            {
                comboQ = factory.WithName("Use (Q)").WithValue(true).Build();
                comboW = factory.WithName("Use (W)").WithValue(true).Build();
                comboE = factory.WithName("Use (E)").WithValue(true).Build();
                comboR = factory.WithName("Use (R)").WithValue(true).Build();
            }

            private void LaneClear(MenuItemFactory factory)
            {
                LCQ = factory.WithName("Use (Q)").WithValue(true).Build();
                LCW = factory.WithName("Use (W)").WithValue(true).Build();

                LCWSlider = factory.WithName("Minimum Minions in Range to (W)").WithValue(new Slider(2, 0, 10)).Build();
                LCE = factory.WithName("Use (E)").WithValue(false).Build();
                LaneClearManaManager =
                    factory.WithName("LaneClear Mana Manager(%)").WithValue(new Slider(40, 0, 100)).Build();
            }

            private void JungleClear(MenuItemFactory factory)
            {
                JGCQ = factory.WithName("Use (Q)").WithValue(true).Build();
                JGCW = factory.WithName("Use (W)").WithValue(true).Build();
                JGCE = factory.WithName("Use (E)").WithValue(false).Build();
                JungleClearManaManager =
                    factory.WithName("JungleClear Mana Manager(%)").WithValue(new Slider(40, 0, 100)).Build();
            }

            private void Harass(MenuItemFactory factory)
            {
                harassQ = factory.WithName("Use (Q)").WithValue(true).Build();
                harassE = factory.WithName("Use (E)").WithValue(true).Build();
                HarassManaManager =
                    factory.WithName("Harass Mana Manager (%)").WithValue(new Slider(40, 0, 100)).Build();
            }

            private void Drawings(MenuItemFactory factory)
            {
                DrawAA = factory.WithName("Draw (AA) Range").WithValue(false).Build();
                DrawQ = factory.WithName("Draw (Q) Range").WithValue(true).Build();
                DrawR = factory.WithName("Draw (R) Range").WithValue(true).Build();
                DrawStunnable = factory.WithName("Draw Stunnable?").WithValue(true).Build();
                QMinionDrawHelper = factory.WithName("Draw Q Circle on Minion if killable?").WithValue(true).Build();

                DrawComboDamage = factory
                    .WithName("Draw Combo Damage")
                    .WithValue(true)
                    .Build();

                FillColor = factory
                    .WithName("Fill Color")
                    .WithValue(new Circle(true, Color.FromArgb(204, 255, 0, 1)))
                    .Build();
            }

            private void Misc(MenuItemFactory factory)
            {
                KSCheck = factory.WithName("KS if Possible?").WithValue(true).Build();
            }
        }
    }
}