using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Pluging
{
    using Common;
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Prediction;
    using SharpDX;
    using Color = System.Drawing.Color;
    using Orbwalking = Orbwalking;
    using static Common.Common;

    internal class MissFortune : Program
    {
        private new readonly Menu Menu = Championmenu;

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

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboQ1", "Use Second Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassQ1", "Use Second Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(false));
                HarassMenu.AddItem(
                    new MenuItem("AutoHarass", "Auto Harass?", true).SetValue(new KeyBind('G', KeyBindType.Toggle)));
                HarassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearECount", "If E CanHit Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearMana", "If Player ManaPercent >= %", true).SetValue(new Slider(60)));
            }

            var JungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                JungleClearMenu.AddItem(
                    new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                MiscMenu.AddItem(
                    new MenuItem("SemiR", "Semi-manual R Key", true).SetValue(new KeyBind('T', KeyBindType.Press)));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
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
                        if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady())
                        {
                            Q.Cast(target, true);
                        }
                        else if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady())
                        {
                            W.Cast();
                        }
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    if (Me.ManaPercent < Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
                        return;

                    var mobs = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);

                    if (mobs.Any())
                    {
                        var mob = mobs.FirstOrDefault();

                        if (mob != null)
                        {
                            if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady() && 
                                mob.Health >= Q.GetDamage(mob))
                            {
                                Q.Cast(mob, true);
                            }
                            else if (Menu.Item("JungleClearW", true).GetValue<bool>() && W.IsReady())
                            {
                                W.Cast();
                            }
                        }
                    }
                }
            }
        }

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
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
                    LaneClear();
                    JungleClear();
                    break;
            }
        }

        private void SemiRLogic()
        {
            if (Menu.Item("SemiR", true).GetValue<KeyBind>().Active && R.IsReady())
            {
                var target = TargetSelector.GetSelectedTarget() ??
                             TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, R.Range))
                {
                    R.CastTo(target);
                }
            }
        }

        private void AutoHarass()
        {
            if (Menu.Item("AutoHarass", true).GetValue<KeyBind>().Active &&
                Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo &&
                Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed &&
                !Me.IsRecalling())
            {
                Harass();
            }
        }

        private void KillSteal()
        {
            if (Menu.Item("KillStealQ", true).GetValue<bool>() && Q.IsReady())
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

            if (CheckTarget(target, QExtend.Range))
            {
                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(QExtend.Range))
                {
                    QLogic(target, Menu.Item("ComboQ1", true).GetValue<bool>());
                }

                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.Cast(target.Position, true);
                }
            }
        }

        private void Harass()
        {
            if (Me.UnderTurret(true))
            {
                return;
            }

            if (Me.ManaPercent >= Menu.Item("HarassMana", true).GetValue<Slider>().Value)
            {
                var target = TargetSelector.GetTarget(QExtend.Range, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, QExtend.Range))
                {
                    if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(QExtend.Range))
                    {
                        QLogic(target, Menu.Item("HarassQ1", true).GetValue<bool>());
                    }

                    if (Menu.Item("HarassE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        E.Cast(target.Position, true);
                    }
                }
            }
        }

        private void LaneClear()
        {
            if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
            {
                var minions = MinionManager.GetMinions(Me.Position, E.Range);

                if (minions.Any())
                {
                    if (Menu.Item("LaneClearE", true).GetValue<bool>() && E.IsReady())
                    {
                        var eFarm = E.GetCircularFarmLocation(minions, E.Width);

                        if (eFarm.MinionsHit >= Menu.Item("LaneClearECount", true).GetValue<Slider>().Value)
                        {
                            E.Cast(eFarm.Position);
                        }
                    }

                    if (Menu.Item("LaneClearQ", true).GetValue<bool>() && Q.IsReady() && minions.Count > 2)
                    {
                        Q.Cast(minions.FirstOrDefault());
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady())
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
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen )
            {
                if (Menu.Item("DrawQ", true).GetValue<bool>() && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.Green, 1);
                }

                if (Menu.Item("DrawE", true).GetValue<bool>() && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(188, 6, 248), 1);
                }

                if (Menu.Item("DrawR", true).GetValue<bool>() && R.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, R.Range, Color.FromArgb(19, 130, 234), 1);
                }

                if (Menu.Item("DrawDamage", true).GetValue<bool>())
                {
                    foreach (
                        var x in HeroManager.Enemies.Where(e => e.IsValidTarget() && !e.IsDead && !e.IsZombie))
                    {
                        HpBarDraw.Unit = x;
                        HpBarDraw.DrawDmg((float)ComboDamage(x), new ColorBGRA(255, 204, 0, 170));
                    }
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
                                         select new CPrediction.Position(t, prediction.UnitPosition)).Where(
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
    }
}
