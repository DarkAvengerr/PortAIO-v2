using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Pluging
{
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;
    using Orbwalking = Orbwalking;
    using static Common;

    internal class Lucian
    {
        private static Spell Q;
        private static Spell QExtend;
        private static Spell W;
        private static Spell E;
        private static Spell R;

        private static int CastSpellTime;

        private static readonly Menu Menu = Program.Championmenu;
        private static readonly AIHeroClient Me = Program.Me;
        private static readonly Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;

        private static HpBarDraw HpBarDraw = new HpBarDraw();

        public Lucian()
        {
            Q = new Spell(SpellSlot.Q, 650f);
            QExtend = new Spell(SpellSlot.Q, 900f);
            W = new Spell(SpellSlot.W, 1000f);
            E = new Spell(SpellSlot.E, 425f);
            R = new Spell(SpellSlot.R, 1200f);

            Q.SetTargetted(0.25f, float.MaxValue);
            QExtend.SetSkillshot(0.35f, 25f, float.MaxValue, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.3f, 80f, 1600, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 1f, float.MaxValue, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.2f, 110f, 2800, true, SkillshotType.SkillshotLine);

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboQExtended", "Use Q Extended", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassQExtended", "Use Q Extended", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(false));
                HarassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
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
                KillStealMenu.AddItem(new MenuItem("KillStealW", "Use W", true).SetValue(true));
            }

            var EMenu = Menu.AddSubMenu(new Menu("E Menu", "E Menu"));
            {
                EMenu.AddItem(new MenuItem("Anti", "Anti Gapcloser E", true).SetValue(true));
                EMenu.AddItem(new MenuItem("CheckECast", "   Check E Cast", true));
                EMenu.AddItem(new MenuItem("underE", "Dont E to Enemy Turret", true).SetValue(true));
                EMenu.AddItem(new MenuItem("ECheck", "Check Wall/ Building", true).SetValue(true));
                EMenu.AddItem(new MenuItem("SafeCheck", "Safe Check", true).SetValue(true));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Drawing.OnDraw += OnDraw;
        }

        private void OnUpdate(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

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
                    break;
            }
        }

        private void KillSteal()
        {
            if (Menu.Item("KillStealW", true).GetValue<bool>() && W.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x => x.IsValidTarget(W.Range) && CheckTargetSureCanKill(x) && x.Health < W.GetDamage(x)))
                {
                    if (CheckTarget(target, W.Range))
                    {
                        W.CastTo(target);
                    }
                }
            }

            if (Menu.Item("KillStealQ", true).GetValue<bool>() && Q.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x => x.IsValidTarget(QExtend.Range) && CheckTargetSureCanKill(x) && x.Health < Q.GetDamage(x)))
                {
                    if (CheckTarget(target, QExtend.Range))
                    {
                        if (target.IsValidTarget(Q.Range))
                        {
                            Q.Cast(target, true);
                        }
                        else
                        {
                            var pred = QExtend.GetPrediction(target, true);
                            var collisions = MinionManager.GetMinions(Me.ServerPosition, Q.Range, MinionTypes.All,
                                MinionTeam.NotAlly);

                            if (!collisions.Any())
                            {
                                return;
                            }

                            foreach (var minion in collisions)
                            {
                                var poly = new Geometry.Polygon.Rectangle(Me.ServerPosition, Me.ServerPosition.Extend(minion.ServerPosition, QExtend.Range), QExtend.Width);

                                if (poly.IsInside(pred.UnitPosition))
                                {
                                    Q.Cast(minion);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Combo()
        {
            if (Menu.Item("ComboQExtended", true).GetValue<bool>() && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(QExtend.Range, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, QExtend.Range) && target.DistanceToPlayer() > Q.Range)
                {
                    var pred = QExtend.GetPrediction(target, true);
                    var collisions = MinionManager.GetMinions(Me.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

                    if (!collisions.Any())
                    {
                        return;
                    }

                    foreach (var minion in collisions)
                    {
                        var poly = new Geometry.Polygon.Rectangle(Me.ServerPosition, Me.ServerPosition.Extend(minion.ServerPosition, QExtend.Range), QExtend.Width);

                        if (poly.IsInside(pred.UnitPosition))
                        {
                            Q.Cast(minion);
                        }
                    }
                }
            }

            if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady())
            {
                var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, R.Range) &&
                    R.GetDamage(target) * (7.5 + 7.5 * Me.AttackSpeedMod) > target.Health &&
                    target.Distance(Me) > Orbwalking.GetAttackRange(Me) + 150)
                {
                    R.Cast(target);
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
                if (Menu.Item("HarassW", true).GetValue<bool>() && Q.IsReady())
                {
                    var target = TargetSelector.GetTarget(QExtend.Range, TargetSelector.DamageType.Physical);

                    if (CheckTarget(target, QExtend.Range))
                    {
                        if (target.IsValidTarget(Q.Range))
                        {
                            Q.Cast(target);
                        }
                        else if (target.IsValidTarget(QExtend.Range) && Menu.Item("HarassQExtended", true).GetValue<bool>())
                        {
                            var pred = QExtend.GetPrediction(target, true);
                            var collisions = MinionManager.GetMinions(Me.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

                            if (!collisions.Any())
                            {
                                return;
                            }

                            foreach (var minion in collisions)
                            {
                                var poly = new Geometry.Polygon.Rectangle(Me.ServerPosition, Me.ServerPosition.Extend(minion.ServerPosition, QExtend.Range), QExtend.Width);

                                if (poly.IsInside(pred.UnitPosition))
                                {
                                    Q.Cast(minion);
                                }
                            }
                        }
                    }
                }

                if (Menu.Item("HarassW", true).GetValue<bool>() && W.IsReady())
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

                    if (CheckTarget(target, W.Range))
                    {
                        W.CastTo(target);
                    }
                }
            }
        }

        private void LaneClear()
        {
            if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("LaneClearQ", true).GetValue<bool>() && Utils.TickCount - CastSpellTime >= 400)
                {
                    var allMinions = MinionManager.GetMinions(Me.Position, Q.Range);

                    if (allMinions.Any())
                    {
                        var minion = allMinions.FirstOrDefault();

                        if (minion != null)
                        {
                            var qExminions = MinionManager.GetMinions(Me.Position, 900);

                            if (QExtend.CountHits(allMinions, Me.Position.Extend(minion.Position, 900)) >= 2)
                            {
                                Q.CastOnUnit(minion);
                            }
                        }
                    }
                }
                if (Menu.Item("LaneClearW", true).GetValue<bool>() && Utils.TickCount - CastSpellTime >= 400)
                {
                    var allMinionE = MinionManager.GetMinions(Me.ServerPosition, W.Range);

                    if (allMinionE.Count > 2)
                    {
                        var pred = W.GetCircularFarmLocation(allMinionE);

                        W.Cast(pred.Position);
                    }
                }
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Me.GetSpellSlot(Args.SData.Name) == SpellSlot.Q || Me.GetSpellSlot(Args.SData.Name) == SpellSlot.W)
            {
                CastSpellTime = Utils.TickCount;
            }
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
                        if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady())
                        {
                            Cast_E(target);
                        }
                        else if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady())
                        {
                            Q.Cast(target, true);
                        }
                        else if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady())
                        {
                            W.Cast(target.Position, true);
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

                        if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady())
                        {
                            var ex = Me.Position.Extend(Game.CursorPos, 150);

                            E.Cast(ex, true);
                        }
                        else if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady())
                        {
                            Q.Cast(mob, true);
                        }
                        else if (Menu.Item("JungleClearW", true).GetValue<bool>() && W.IsReady())
                        {
                            W.Cast(mob, true);
                        }
                    }
                }
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("Anti", true).GetValue<bool>() && E.IsReady())
            {
                if (gapcloser.End.Distance(Me.Position) <= 200 || gapcloser.Sender.Distance(Me) < 250)
                {
                    E.Cast(Me.Position.Extend(gapcloser.Sender.Position, -E.Range));
                }
            }
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen)
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

                if (Menu.Item("DrawDamage", true).GetValue<bool>())
                {
                    foreach (
                        var x in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && !e.IsDead && !e.IsZombie))
                    {
                        HpBarDraw.Unit = x;
                        HpBarDraw.DrawDmg((float)ComboDamage(x), new ColorBGRA(255, 204, 0, 170));
                    }
                }
            }
        }

        private void Cast_E(AIHeroClient target)
        {
            var castpos = Me.ServerPosition.Extend(Game.CursorPos, 220);
            var maxepos = Me.ServerPosition.Extend(Game.CursorPos, E.Range);

            if (castpos.UnderTurret(true) && Menu.Item("underE", true).GetValue<bool>())
            {
                return;
            }

            if ((NavMesh.GetCollisionFlags(castpos).HasFlag(CollisionFlags.Wall) ||
                NavMesh.GetCollisionFlags(castpos).HasFlag(CollisionFlags.Building)) &&
                Menu.Item("ECheck", true).GetValue<bool>())
            {
                return;
            }

            if (castpos.CountEnemiesInRange(500) >= 3 && castpos.CountAlliesInRange(400) < 3 &&
                Menu.Item("SafeCheck", true).GetValue<bool>())
            {
                return;
            }

            if (Orbwalking.InAutoAttackRange(target) &&
                target.ServerPosition.Distance(castpos) < Orbwalking.GetAttackRange(Me))
            {
                E.Cast(castpos, true);
            }
            else if (!Orbwalking.InAutoAttackRange(target) && target.ServerPosition.Distance(castpos) > 
                Orbwalking.GetAttackRange(Me))
            {
                E.Cast(castpos, true);
            }
            else if (!Orbwalking.InAutoAttackRange(target) &&
                     target.ServerPosition.Distance(castpos) > Orbwalking.GetAttackRange(Me) &&
                     target.ServerPosition.Distance(maxepos) < Orbwalking.GetAttackRange(Me))
            {
                E.Cast(maxepos, true);
            }
        }
    }
}
