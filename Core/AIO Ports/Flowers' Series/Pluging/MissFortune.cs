using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_ADC_Series.Pluging
{
    using ADCCOMMON;
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;
    using Orbwalking = ADCCOMMON.Orbwalking;

    internal class MissFortune : Logic
    {
        public MissFortune()
        {
            Q = new Spell(SpellSlot.Q, 700f);
            QExtend = new Spell(SpellSlot.Q, 1300f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 1350f);

            QExtend.SetSkillshot(0.25f, 70f, 1500f, true, SkillshotType.SkillshotLine);
            Q.SetTargetted(0.25f, 1400f);
            E.SetSkillshot(0.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 50f, 3000f, false, SkillshotType.SkillshotCircle);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboQ1", "Use Second Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassQ1", "Use Second Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(false));
                harassMenu.AddItem(
                    new MenuItem("AutoHarass", "Auto Harass?", true).SetValue(new KeyBind('G', KeyBindType.Toggle)));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearECount", "If E CanHit Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "If Player ManaPercent >= %", true).SetValue(new Slider(60)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                miscMenu.AddItem(
                    new MenuItem("SemiR", "Semi-manual R Key", true).SetValue(new KeyBind('T', KeyBindType.Press)));
            }

            var utilityMenu = Menu.AddSubMenu(new Menu("Utility", "Utility"));
            {
                var skinMenu = utilityMenu.AddSubMenu(new Menu("Skin Change", "Skin Change"));
                {
                    SkinManager.AddToMenu(skinMenu);
                }

                var autoLevelMenu = utilityMenu.AddSubMenu(new Menu("Auto Levels", "Auto Levels"));
                {
                    LevelsManager.AddToMenu(autoLevelMenu);
                }

                var itemsMenu = utilityMenu.AddSubMenu(new Menu("Items", "Items"));
                {
                    ItemsManager.AddToMenu(itemsMenu);
                }
            }

            var drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                DamageIndicator.AddToMenu(drawMenu);
            }

            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    var target = Args.Target as AIHeroClient;

                    if (target != null)
                    {
                        if (Menu.GetBool("ComboQ") && Q.IsReady())
                        {
                            Q.Cast(target, true);
                        }
                        else if (Menu.GetBool("ComboW") && W.IsReady())
                        {
                            W.Cast();
                        }
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
                    {
                        var mobs = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral,
                            MinionOrderTypes.MaxHealth);

                        if (mobs.Any())
                        {
                            var mob = mobs.FirstOrDefault();

                            if (mob != null)
                            {
                                if (Menu.GetBool("JungleClearQ") && Q.IsReady())
                                {
                                    Q.Cast(mob, true);
                                }
                                else if (Menu.GetBool("JungleClearW") && W.IsReady())
                                {
                                    W.Cast();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            if (Me.HasBuff("missfortunebulletsound") || Me.IsCastingInterruptableSpell())
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                return;
            }

            Orbwalker.SetAttack(true);
            Orbwalker.SetMovement(true);

            SemiRLogic();
            AutoHarass();
            KillSteal();

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    FarmHarass();
                    LaneClear();
                    JungleClear();
                    break;
            }
        }

        private void SemiRLogic()
        {
            if (Menu.GetKey("SemiR") && R.IsReady())
            {
                var target = TargetSelector.GetSelectedTarget() ??
                             TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (target.Check(R.Range))
                {
                    SpellManager.PredCast(R, target, true);
                }
            }
        }

        private void AutoHarass()
        {
            if (Menu.GetKey("AutoHarass") && !Orbwalking.isCombo && !Orbwalking.isHarass && !Me.IsRecalling())
            {
                Harass();
            }
        }

        private void KillSteal()
        {
            if (Menu.GetBool("KillStealQ") && Q.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(x => x.IsValidTarget(QExtend.Range) && x.Health < Q.GetDamage(x)))
                {
                    QLogic(target, true);
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                         TargetSelector.GetTarget(QExtend.Range, TargetSelector.DamageType.Physical);

            if (target.Check(QExtend.Range))
            {
                if (Menu.GetBool("ComboQ") && Q.IsReady() && target.IsValidTarget(QExtend.Range))
                {
                    QLogic(target, Menu.GetBool("ComboQ1"));
                }

                if (Menu.GetBool("ComboE") && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.Cast(target.Position, true);
                }
            }
        }

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                var target = TargetSelector.GetTarget(QExtend.Range, TargetSelector.DamageType.Physical);

                if (target.Check(QExtend.Range))
                {
                    if (Menu.GetBool("HarassQ") && Q.IsReady() && target.IsValidTarget(QExtend.Range))
                    {
                        QLogic(target, Menu.GetBool("HarassQ1"));
                    }

                    if (Menu.GetBool("HarassE") && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        E.Cast(target.Position, true);
                    }
                }
            }
        }

        private void FarmHarass()
        {
            if (ManaManager.SpellHarass)
            {
                Harass();
            }
        }

        private void LaneClear()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearMana")) && ManaManager.SpellFarm)
            {
                var minions = MinionManager.GetMinions(Me.Position, E.Range);

                if (minions.Any())
                {
                    if (Menu.GetBool("LaneClearE") && E.IsReady())
                    {
                        var eFarm =
                            MinionManager.GetBestCircularFarmLocation(minions.Select(x => x.Position.To2D()).ToList(),
                                E.Width, E.Range);

                        if (eFarm.MinionsHit >= Menu.GetSlider("LaneClearECount"))
                        {
                            E.Cast(eFarm.Position);
                        }
                    }

                    if (Menu.GetBool("LaneClearQ") && Q.IsReady() && minions.Count > 2)
                    {
                        Q.Cast(minions.FirstOrDefault());
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                if (Menu.GetBool("JungleClearE") && E.IsReady())
                {
                    var mobs = MinionManager.GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);

                    if (mobs.Any())
                    {
                        var bigmobs = mobs.FirstOrDefault(x => !x.Name.ToLower().Contains("mini"));

                        if (bigmobs != null)
                        {
                            E.Cast(bigmobs.Position);
                        }
                        else if (mobs.Count >= 2)
                        {
                            E.Cast(mobs.FirstOrDefault());
                        }
                    }
                }
            }
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (Menu.GetBool("DrawQ") && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.Green, 1);
                }

                if (Menu.GetBool("DrawE") && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(188, 6, 248), 1);
                }

                if (Menu.GetBool("DrawR") && R.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, R.Range, Color.FromArgb(19, 130, 234), 1);
                }
            }
        }

        private void QLogic(AIHeroClient target, bool UseQ1 = false)// SFX Challenger MissFortune QLogic (im so lazy, kappa)
        {
            if (target != null)
            {
                if (target.IsValidTarget(Q.Range))
                {
                    Q.CastOnUnit(target);
                }
                else if (UseQ1 && target.IsValidTarget(QExtend.Range) && target.DistanceToPlayer() > Q.Range)
                {
                    var heroPositions = (from t in HeroManager.Enemies
                                         where t.IsValidTarget(QExtend.Range)
                                         let prediction = Q.GetPrediction(t)
                                         select new Position(t, prediction.UnitPosition)).Where(
                        t => t.UnitPosition.Distance(Me.Position) < QExtend.Range).ToList();

                    if (heroPositions.Any())
                    {
                        var minions = MinionManager.GetMinions(QExtend.Range, MinionTypes.All, MinionTeam.NotAlly);

                        if (minions.Any(m => m.IsMoving) &&
                            !heroPositions.Any(h => h.Hero.HasBuff("missfortunepassive")))
                        {
                            return;
                        }

                        var outerMinions = minions.Where(m => m.Distance(Me) > Q.Range).ToList();
                        var innerPositions = minions.Where(m => m.Distance(Me) < Q.Range).ToList();

                        foreach (var minion in innerPositions)
                        {
                            var lMinion = minion;
                            var coneBuff = new Geometry.Polygon.Sector(
                                minion.Position,
                                Me.Position.Extend(minion.Position, Me.Distance(minion) + Q.Range * 0.5f),
                                (float)(40 * Math.PI / 180), QExtend.Range - Q.Range);
                            var coneNormal = new Geometry.Polygon.Sector(
                                minion.Position,
                                Me.Position.Extend(minion.Position, Me.Distance(minion) + Q.Range * 0.5f),
                                (float)(60 * Math.PI / 180), QExtend.Range - Q.Range);

                            foreach (var enemy in
                                heroPositions.Where(
                                    m => m.UnitPosition.Distance(lMinion.Position) < QExtend.Range - Q.Range))
                            {
                                if (coneBuff.IsInside(enemy.Hero) && enemy.Hero.HasBuff("missfortunepassive"))
                                {
                                    Q.CastOnUnit(minion);
                                    return;
                                }
                                if (coneNormal.IsInside(enemy.UnitPosition))
                                {
                                    var insideCone =
                                        outerMinions.Where(m => coneNormal.IsInside(m.Position)).ToList();

                                    if (!insideCone.Any() ||
                                        enemy.UnitPosition.Distance(minion.Position) <
                                        insideCone.Select(
                                                m => m.Position.Distance(minion.Position) - m.BoundingRadius)
                                            .DefaultIfEmpty(float.MaxValue)
                                            .Min())
                                    {
                                        Q.CastOnUnit(minion);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private struct Position
        {
            public readonly AIHeroClient Hero;
            public readonly Vector3 UnitPosition;

            public Position(AIHeroClient hero, Vector3 unitPosition)
            {
                Hero = hero;
                UnitPosition = unitPosition;
            }
        }
    }
}
