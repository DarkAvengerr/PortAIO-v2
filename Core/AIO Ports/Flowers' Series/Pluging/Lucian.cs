using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_ADC_Series.Pluging
{
    using ADCCOMMON;
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Color = System.Drawing.Color;
    using Orbwalking = ADCCOMMON.Orbwalking;

    internal class Lucian : Logic
    {
        private int CastSpellTime;

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

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboQExtended", "Use Q Extended", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboELogic", "Use E|First E Logic?", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassQExtended", "Use Q Extended", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(false));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
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
                killStealMenu.AddItem(new MenuItem("KillStealW", "Use W", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var eMenu = miscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    eMenu.AddItem(new MenuItem("Anti", "Anti Gapcloser E", true).SetValue(true));
                    eMenu.AddItem(new MenuItem("ShortELogic", "Smart Short E Logic", true).SetValue(true));
                    eMenu.AddItem(new MenuItem("underE", "Dont E to Enemy Turret", true).SetValue(true));
                    eMenu.AddItem(new MenuItem("ECheck", "Check Wall/ Building", true).SetValue(true));
                    eMenu.AddItem(new MenuItem("SafeCheck", "Safe Check", true).SetValue(true));
                }

                var rMenu = miscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rMenu.AddItem(new MenuItem("RMove", "Auto Move|If R Is Casting?", true).SetValue(true));
                    rMenu.AddItem(new MenuItem("RYoumuu", "Auto Youmuu|If R Is Casting?", true).SetValue(true));
                }
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
                drawMenu.AddItem(new MenuItem("DrawQEx", "Draw QEx Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                DamageIndicator.AddToMenu(drawMenu);
            }

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Drawing.OnDraw += OnDraw;
        }

        private void OnUpdate(EventArgs args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            if (Menu.GetBool("RMove") && Me.HasBuff("LucianR"))
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
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
                    FarmHarass();
                    LaneClear();
                    break;
            }
        }

        private void KillSteal()
        {
            if (Menu.GetBool("KillStealW") && W.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.Check(W.Range) && x.Health < W.GetDamage(x)))
                {
                    if (target.Check(W.Range))
                    {
                        SpellManager.PredCast(W, target, true);
                    }
                }
            }

            if (Menu.Item("KillStealQ", true).GetValue<bool>() && Q.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(QExtend.Range) && x.Health < Q.GetDamage(x)))
                {
                    if (target.Check(QExtend.Range))
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
                                var poly = new Geometry.Polygon.Rectangle(Me.ServerPosition,
                                    Me.ServerPosition.Extend(minion.ServerPosition, QExtend.Range), QExtend.Width);

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
            if (Menu.GetBool("ComboELogic") && E.IsReady())
            {
                var target = TargetSelector.GetTarget(975f, TargetSelector.DamageType.Physical);

                if (target.IsValidTarget(975f) && target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me))
                {
                    if (Utils.TickCount - CastSpellTime > 400)
                    {
                        Cast_E(target, true);
                    }
                }
            }

            if (Menu.GetBool("ComboQExtended") && Q.IsReady() && !Me.IsDashing() && !Me.Spellbook.IsAutoAttacking)
            {
                var target = TargetSelector.GetTarget(QExtend.Range, TargetSelector.DamageType.Physical);

                if (target.Check(QExtend.Range) && target.DistanceToPlayer() > Q.Range &&
                    (!E.IsReady() || (E.IsReady() && target.DistanceToPlayer() > 975f)))
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
                        var poly = new Geometry.Polygon.Rectangle(Me.ServerPosition,
                            Me.ServerPosition.Extend(minion.ServerPosition, QExtend.Range), QExtend.Width);

                        if (poly.IsInside(pred.UnitPosition))
                        {
                            Q.Cast(minion);
                        }
                    }
                }
            }

            if (Menu.GetBool("ComboR") && R.IsReady())
            {
                var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (target.Check(R.Range) &&
                    R.GetDamage(target) * (7.5 + 7.5 * Me.AttackSpeedMod) > target.Health &&
                    target.Distance(Me) > Orbwalking.GetAttackRange(Me) + 300)
                {
                    R.Cast(target);
                }
            }
        }

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                if (Menu.GetBool("HarassQ") && Q.IsReady())
                {
                    var target = TargetSelector.GetTarget(QExtend.Range, TargetSelector.DamageType.Physical);

                    if (target.Check(QExtend.Range))
                    {
                        if (target.IsValidTarget(Q.Range))
                        {
                            Q.Cast(target);
                        }
                        else if (target.IsValidTarget(QExtend.Range) && Menu.GetBool("HarassQExtended"))
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
                                var poly = new Geometry.Polygon.Rectangle(Me.ServerPosition,
                                    Me.ServerPosition.Extend(minion.ServerPosition, QExtend.Range), QExtend.Width);

                                if (poly.IsInside(pred.UnitPosition))
                                {
                                    Q.Cast(minion);
                                }
                            }
                        }
                    }
                }

                if (Menu.GetBool("HarassW") && W.IsReady())
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

                    if (target.Check(W.Range))
                    {
                        SpellManager.PredCast(W, target, true);
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
                if (Menu.GetBool("LaneClearQ") && Utils.TickCount - CastSpellTime >= 400)
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

                if (Menu.GetBool("LaneClearW") && Utils.TickCount - CastSpellTime >= 400)
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

            if (Me.GetSpellSlot(Args.SData.Name) == SpellSlot.Q || Me.GetSpellSlot(Args.SData.Name) == SpellSlot.W ||
                Me.GetSpellSlot(Args.SData.Name) == SpellSlot.E)
            {
                CastSpellTime = Utils.TickCount;
            }

            if (Me.GetSpellSlot(Args.SData.Name) == SpellSlot.R && Menu.GetBool("RYoumuu"))
            {
                if (Items.HasItem(3142))
                {
                    Items.UseItem(3142);
                }
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                if (Args.Target is Obj_LampBulb)
                {
                    return;
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    var target = Args.Target as AIHeroClient;

                    if (target != null)
                    {
                        if (Menu.GetBool("ComboE") && E.IsReady())
                        {
                            Cast_E(target, false);
                        }
                        else if (Menu.GetBool("ComboQ") && Q.IsReady())
                        {
                            Q.Cast(target, true);
                        }
                        else if (Menu.GetBool("ComboW") && W.IsReady())
                        {
                            W.Cast(target.Position, true);
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

                            if (Menu.GetBool("JungleClearE") && E.IsReady())
                            {
                                var ex = Me.Position.Extend(Game.CursorPos, 150);

                                E.Cast(ex, true);
                            }
                            else if (Menu.GetBool("JungleClearQ") && Q.IsReady())
                            {
                                Q.Cast(mob, true);
                            }
                            else if (Menu.GetBool("JungleClearW") && W.IsReady())
                            {
                                W.Cast(mob, true);
                            }
                        }
                    }
                }
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.GetBool("Anti") && E.IsReady())
            {
                if (gapcloser.End.Distance(Me.Position) <= 200 || gapcloser.Sender.Distance(Me) < 250)
                {
                    E.Cast(Me.Position.Extend(gapcloser.Sender.Position, -E.Range));
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

                if (Menu.GetBool("DrawQEx") && QExtend.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, QExtend.Range, Color.Green, 1);
                }

                if (Menu.GetBool("DrawW") && W.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, W.Range, Color.FromArgb(9, 253, 242), 1);
                }

                if (Menu.GetBool("DrawE") && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(188, 6, 248), 1);
                }
            }
        }

        private void Cast_E(AIHeroClient target, bool FirstE)
        {
            if (FirstE)
            {
                var castpos = Me.ServerPosition.Extend(target.ServerPosition, 220);
                var maxepos = Me.ServerPosition.Extend(target.ServerPosition, E.Range);

                if (maxepos.UnderTurret(true) && Menu.GetBool("underE"))
                {
                    return;
                }

                if (NavMesh.GetCollisionFlags(maxepos).HasFlag(CollisionFlags.Wall) ||
                      NavMesh.GetCollisionFlags(maxepos).HasFlag(CollisionFlags.Building) &&
                    Menu.GetBool("ECheck"))
                {
                    return;
                }

                if (maxepos.CountEnemiesInRange(500) >= 3 && maxepos.CountAlliesInRange(400) < 3 && Menu.GetBool("SafeCheck"))
                {
                    return;
                }

                if (!Orbwalking.InAutoAttackRange(target) &&
                         target.ServerPosition.Distance(castpos) > Orbwalking.GetRealAutoAttackRange(Me) &&
                         target.ServerPosition.Distance(maxepos) <= Orbwalking.GetRealAutoAttackRange(Me))
                {
                    E.Cast(maxepos, true);
                }
            }
            else
            {
                var castpos = Me.ServerPosition.Extend(Game.CursorPos, 220);
                var maxepos = Me.ServerPosition.Extend(Game.CursorPos, E.Range);

                if ((castpos.UnderTurret(true) || maxepos.UnderTurret(true)) && Menu.GetBool("underE"))
                {
                    return;
                }

                if ((NavMesh.GetCollisionFlags(castpos).HasFlag(CollisionFlags.Wall) ||
                     NavMesh.GetCollisionFlags(castpos).HasFlag(CollisionFlags.Building) &&
                     (NavMesh.GetCollisionFlags(maxepos).HasFlag(CollisionFlags.Wall) ||
                      NavMesh.GetCollisionFlags(maxepos).HasFlag(CollisionFlags.Building))) &&
                    Menu.GetBool("ECheck"))
                {
                    return;
                }

                if (((castpos.CountEnemiesInRange(500) >= 3 && castpos.CountAlliesInRange(400) < 3) ||
                     (maxepos.CountEnemiesInRange(500) >= 3 && maxepos.CountAlliesInRange(400) < 3)) &&
                    Menu.GetBool("SafeCheck"))
                {
                    return;
                }

                if (Orbwalking.InAutoAttackRange(target) &&
                    target.ServerPosition.Distance(castpos) <= Orbwalking.GetRealAutoAttackRange(Me))
                {
                    E.Cast(Menu.GetBool("ShortELogic") ? castpos : maxepos, true);
                }
                else if (!Orbwalking.InAutoAttackRange(target) && target.ServerPosition.Distance(castpos) <=
                    Orbwalking.GetRealAutoAttackRange(Me))
                {
                    E.Cast(Menu.GetBool("ShortELogic") ? castpos : maxepos, true);
                }
                else if (!Orbwalking.InAutoAttackRange(target) &&
                         target.ServerPosition.Distance(castpos) > Orbwalking.GetRealAutoAttackRange(Me) &&
                         target.ServerPosition.Distance(maxepos) <= Orbwalking.GetRealAutoAttackRange(Me))
                {
                    E.Cast(maxepos, true);
                }
            }
        }
    }
}
