using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Pluging
{
    using Common;
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;
    using Orbwalking = Orbwalking;
    using static Common.Common;

    internal class Corki : Program
    {
        private new readonly Menu Menu = Championmenu;

        public Corki()
        {
            Q = new Spell(SpellSlot.Q, 825f);
            W = new Spell(SpellSlot.W, 800f);
            E = new Spell(SpellSlot.E, 600f);
            R = new Spell(SpellSlot.R, 1300f);

            Q.SetSkillshot(0.3f, 200f, 1000f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.1f, (float)(45 * Math.PI / 180), 1500f, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                ComboMenu.AddItem(
                    new MenuItem("ComboRLimit", "Use R|Limit Stack >= x", true).SetValue(new Slider(0, 0, 7)));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(false));
                HarassMenu.AddItem(new MenuItem("HarassR", "Use R", true).SetValue(true));
                HarassMenu.AddItem(
                    new MenuItem("HarassRLimit", "Use R|Limit Stack >= x", true).SetValue(new Slider(4, 0, 7)));
                HarassMenu.AddItem(
                    new MenuItem("AutoHarass", "Auto Harass?", true).SetValue(new KeyBind('G', KeyBindType.Toggle)));
                HarassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearQCount", "If Q CanHit Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                LaneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearECount", "If E CanHit Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                LaneClearMenu.AddItem(new MenuItem("LaneClearR", "Use R", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearRLimit", "Use R|Limit Stack >= x", true).SetValue(new Slider(4, 0, 7)));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearRCount", "If R CanHit Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearMana", "If Player ManaPercent >= %", true).SetValue(new Slider(60)));
            }

            var JungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearR", "Use R", true).SetValue(true));
                JungleClearMenu.AddItem(
                    new MenuItem("JungleClearRLimit", "Use R|Limit Stack >= x", true).SetValue(new Slider(0, 0, 7)));
                JungleClearMenu.AddItem(
                    new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealR", "Use R", true).SetValue(true));
            }

            var FleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                FleeMenu.AddItem(new MenuItem("FleeW", "Use W", true).SetValue(true));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                MiscMenu.AddItem(
                    new MenuItem("SemiR", "Semi-manual R Key", true).SetValue(new KeyBind('T', KeyBindType.Press)));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
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
                        if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady() &&
                            R.Instance.Ammo >= Menu.Item("ComboRLimit", true).GetValue<Slider>().Value)
                        {
                            R.CastTo(target);
                        }
                        else if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady())
                        {
                            Q.CastTo(target, true);
                        }
                        else if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady())
                        {
                            E.Cast();
                        }
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    if (Me.ManaPercent < Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
                    {
                        return;
                    }

                    var mobs = MinionManager.GetMinions(R.Range, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);

                    if (mobs.Any())
                    {
                        var mob = mobs.FirstOrDefault();

                        if (Menu.Item("JungleClearR", true).GetValue<bool>() && R.IsReady() &&
                            R.Instance.Ammo >= Menu.Item("JungleClearRLimit", true).GetValue<Slider>().Value)
                        {
                            R.Cast(mob, true);
                        }
                        else if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady())
                        {
                            Q.Cast(mob, true);
                        }
                        else if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady())
                        {
                            E.Cast();
                        }
                    }
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (R.Level > 0)
            {
                R.Range = Me.HasBuff("CorkiMissileBarrageCounterBig") ? 1500f : 1300f;
            }

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
                case Orbwalking.OrbwalkingMode.Flee:
                    Flee();
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
            if (Me.UnderTurret(true))
            {
                return;
            }

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
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)))
                {
                    Q.CastTo(target);
                    return;
                }
            }

            if (Menu.Item("KillStealR", true).GetValue<bool>() && R.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && x.Health < R.GetDamage(x)))
                {
                    R.CastTo(target);
                    return;
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (CheckTarget(target, R.Range))
            {
                if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady() &&
                    R.Instance.Ammo >= Menu.Item("ComboRLimit", true).GetValue<Slider>().Value &&
                    target.IsValidTarget(R.Range))
                {
                    R.CastTo(target);
                }

                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.CastTo(target, true);
                }

                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.Cast();
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
                var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, R.Range))
                {
                    if (Menu.Item("HarassR", true).GetValue<bool>() && R.IsReady() &&
                        R.Instance.Ammo >= Menu.Item("HarassRLimit", true).GetValue<Slider>().Value &&
                        target.IsValidTarget(R.Range))
                    {
                        R.CastTo(target);
                    }

                    if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.CastTo(target, true);
                    }

                    if (Menu.Item("HarassE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        E.Cast();
                    }
                }
            }
        }

        private void LaneClear()
        {
            if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("LaneClearQ", true).GetValue<bool>() && Q.IsReady())
                {
                    var minions = MinionManager.GetMinions(Me.Position, Q.Range);

                    if (minions.Any())
                    {
                        var QFram = Q.GetCircularFarmLocation(minions, Q.Width);

                        if (QFram.MinionsHit >= Menu.Item("LaneClearQCount", true).GetValue<Slider>().Value)
                        {
                            Q.Cast(QFram.Position);
                        }
                    }
                }

                if (Menu.Item("LaneClearE", true).GetValue<bool>() && Q.IsReady())
                {
                    var eMinions = MinionManager.GetMinions(Me.Position, E.Range);

                    if (eMinions.Any())
                    {
                        if (eMinions.Count >= Menu.Item("LaneClearECount", true).GetValue<Slider>().Value)
                        {
                            E.Cast();
                        }
                    }
                }

                if (Menu.Item("LaneClearR", true).GetValue<bool>() && R.IsReady() &&
                    R.Instance.Ammo >= Menu.Item("LaneClearRLimit", true).GetValue<Slider>().Value)
                {
                    var rMinions = MinionManager.GetMinions(Me.Position, R.Range);

                    if (rMinions.Any())
                    {
                        var RFarm = R.GetLineFarmLocation(rMinions, R.Width);

                        if (RFarm.MinionsHit >= Menu.Item("LaneClearRCount", true).GetValue<Slider>().Value)
                        {
                            R.Cast(RFarm.Position);
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
            {
                var mobs = MinionManager.GetMinions(R.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    var mob = mobs.FirstOrDefault();

                    if (Menu.Item("JungleClearR", true).GetValue<bool>() && R.IsReady() &&
                        R.Instance.Ammo >= Menu.Item("JungleClearRLimit", true).GetValue<Slider>().Value)
                    {
                        R.Cast(mob, true);
                    }

                    if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady())
                    {
                        Q.Cast(mob, true);
                    }
                }
            }
        }

        private void Flee()
        {
            if (Menu.Item("FleeW", true).GetValue<bool>() && W.IsReady())
            {
                W.Cast(Me.Position.Extend(Game.CursorPos, W.Range));
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

                if (Menu.Item("DrawW", true).GetValue<bool>() && W.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, W.Range, Color.FromArgb(9, 253, 242), 1);
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
    }
}
