using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;
using LeagueSharp.Common.Data;
using Color = System.Drawing.Color;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace xBard
{
    class Program
    {
        private static Menu Menu;

        private static AIHeroClient Player = ObjectManager.Player;

        private static Spell Q, W, E, R;

        private static Orbwalking.Orbwalker Orbwalker;
        public static List<Vector3> Cords = new List<Vector3>();

        public static void Game_OnGameLoad()
        {
            Q = new Spell(SpellSlot.Q, true);
            W = new Spell(SpellSlot.W, true);
            E = new Spell(SpellSlot.E, true);
            R = new Spell(SpellSlot.R, true);

            Q.SetSkillshot(250, 60, 1600, true, SkillshotType.SkillshotLine);

            R.SetSkillshot(500, 350, 2100, false, SkillshotType.SkillshotCircle);

            Menu = new Menu("xBard", "xBard", true).SetFontStyle(System.Drawing.FontStyle.Regular, SharpDX.Color.Yellow);

            Menu.AddItem(new MenuItem("Donate", "Buy me coffe"));
            Menu.AddItem(new MenuItem("Donate2", "paypal: nynov3@gmail.com"));

            Menu OrbWalking = Menu.AddSubMenu(new Menu("OrbWalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(OrbWalking);

            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            TargetSelector.AddToMenu(ts);

            Menu Combo = Menu.AddSubMenu(new Menu("Combo", "Combo"));

            Menu QSettings = Combo.AddSubMenu(new Menu("Q settings", "Qsettings"));
            QSettings.AddItem(new MenuItem("UseQcombo", "Use Q").SetValue(true));
            QSettings.AddItem(new MenuItem("UseQcomboStun", "Only if can stun").SetValue(false));

            Menu WSettings = Combo.AddSubMenu(new Menu("W settings", "Wsettings"));
            WSettings.AddItem(new MenuItem("AutoWNearTower", "Auto W near tower").SetValue(true));
            WSettings.AddItem(new MenuItem("AutoWNearTowerMana", "Min mana %").SetValue(new Slider(25, 1, 100)));

            Menu WSettingsSave = new Menu("W heal ally", "WcomboSaveAlly");
            {
                WSettingsSave.AddItem(new MenuItem("HealAlly", "Heal ally with W").SetValue(true));
                foreach (var hero in HeroManager.Allies)
                {
                    WSettingsSave.AddItem(new MenuItem("WSettings." + hero.ChampionName.ToLower(), hero.ChampionName.ToLower()).SetValue(true));
                    WSettingsSave.AddItem(new MenuItem("WSettingsHealthPercent." + hero.ChampionName.ToLower(), "Health % for W").SetValue(new Slider(40, 1, 100)));
                }
                WSettings.AddSubMenu(WSettingsSave);
            }

            Menu ESettings = Combo.AddSubMenu(new Menu("E settings", "Esettings"));
            ESettings.AddItem(new MenuItem("UseEcombo", "Use E").SetValue(false).SetTooltip("if on your path is wall will cast E"));

            Menu RSettings = Combo.AddSubMenu(new Menu("R settings", "Rsettings"));
            RSettings.AddItem(new MenuItem("UseRcombo", "Use R").SetValue(true));
            RSettings.AddItem(new MenuItem("UseRcomboCount", "Only if can hit X enemys").SetValue(new Slider(2, 1, 5)));
            RSettings.AddItem(new MenuItem("UseRcomboAuto", "Auto R").SetValue(true));
            RSettings.AddItem(new MenuItem("UseRcomboCountAuto", "Auto if can hit X enemys").SetValue(new Slider(4, 1, 5)));
            RSettings.AddItem(new MenuItem("UseRcomboTower", "Auto R on tower").SetValue(true));
            RSettings.AddItem(new MenuItem("UseRcomboTowerHP", "if hp under x %").SetValue(new Slider(40, 1, 100)));

            Menu RSettingsSave = new Menu("R save ally", "RcomboSaveAlly");
            {
                RSettingsSave.AddItem(new MenuItem("SaveAlly", "Save ally with R").SetValue(true));
                foreach (var hero in HeroManager.Allies)
                {
                    RSettingsSave.AddItem(new MenuItem("RSettings." + hero.ChampionName.ToLower(), hero.ChampionName.ToLower()).SetValue(true));
                    RSettingsSave.AddItem(new MenuItem("RSettingsHealthPercent." + hero.ChampionName.ToLower(), "Health % for R").SetValue(new Slider(10, 1, 100)));
                }
                RSettings.AddSubMenu(RSettingsSave);
            }

            Menu Mixed = Menu.AddSubMenu(new Menu("Mixed", "Mixed"));
            Mixed.AddItem(new MenuItem("UseQmixed", "Use Q").SetValue(true));
            Mixed.AddItem(new MenuItem("UseQMixedMana", "Min mana %").SetValue(new Slider(30, 0, 100)));
            Mixed.AddItem(new MenuItem("UseQMixedCount", "Only if can hit X enemys").SetValue(new Slider(2, 1, 2)).SetTooltip("Becasue bard can hit max 2 units"));

            Menu LaneClear = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            LaneClear.AddItem(new MenuItem("UseQlaneClear", "Use Q").SetValue(true));
            LaneClear.AddItem(new MenuItem("UseQLaneClearMana", "Min mana %").SetValue(new Slider(30, 0, 100)));
            LaneClear.AddItem(new MenuItem("UseQLaneClearCount", "Only if can hit X minions").SetValue(new Slider(2, 1, 2)).SetTooltip("Becasue bard can hit max 2 units"));

            Menu LastHit = Menu.AddSubMenu(new Menu("LastHit", "LastHit"));
            LastHit.AddItem(new MenuItem("UseQlasthit", "Use Q").SetValue(true));
            LastHit.AddItem(new MenuItem("UseQlasthitMana", "Min mana %").SetValue(new Slider(30, 0, 100)));

            Menu Misc = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            Misc.AddItem(new MenuItem("SupportMode", "Support mode").SetValue(false).SetTooltip("Dont attacking minions in mixed mode"));
            Misc.AddItem(new MenuItem("AFKmode", "Auto collect all chimes").SetValue(false));

            Menu GapCloser = Misc.AddSubMenu(new Menu("GapClose", "GapCloser"));
            GapCloser.AddItem(new MenuItem("UseQGapCloser", "Use Q").SetValue(true));

            Menu interrupt = Misc.AddSubMenu(new Menu("interrupt", "interrupt"));
            interrupt.AddItem(new MenuItem("UseQinterrupt", "Use Q").SetValue(true));
            interrupt.AddItem(new MenuItem("UseQinterruptStun", "Only if can stun").SetValue(false));
            interrupt.AddItem(new MenuItem("UseRinterrupt", "Use R").SetValue(true).SetTooltip("Only spells with High danger"));
            Menu.AddToMainMenu();

            Menu Draw = Menu.AddSubMenu(new Menu("Draw", "Draw"));
            Draw.AddItem(new MenuItem("DrawQ", "Draw Q").SetValue(true));
            Draw.AddItem(new MenuItem("DrawW", "Draw W").SetValue(true));
            Draw.AddItem(new MenuItem("DrawR", "Draw R").SetValue(true));
            Draw.AddItem(new MenuItem("DrawRMinimap", "Draw R on minimap").SetValue(true));
            Draw.AddItem(new MenuItem("DrawRPred", "Draw R prediction").SetValue(true));
            Draw.AddItem(new MenuItem("DrawChimes", "Draw chimes in auto mode").SetValue(true));

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.BeforeAttack += OnBeforeAttack;
            GameObject.OnCreate += GameObject_OnCreate;
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsValid && sender.Name.ToLower() == "bardchimeminion")
            {
                Cords.Add(sender.Position.LSTo2D().To3D());
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Menu.Item("DrawQ").GetValue<bool>())
            {
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, Q.Range, Color.Red, 1, 30, false);
            }
            if (Menu.Item("DrawW").GetValue<bool>())
            {
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, W.Range, Color.Red, 1, 30, false);
            }
            if (Menu.Item("DrawR").GetValue<bool>())
            {
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, Color.Red, 1, 30, false);
            }
            if (Menu.Item("DrawRMinimap").GetValue<bool>())
            {
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, Color.Red, 1, 30, true);
            }
            if (Menu.Item("DrawChimes").GetValue<bool>() && Menu.Item("AFKmode").GetValue<bool>())
            {
                foreach (var item in Cords)
                {
                    LeagueSharp.Common.Utility.DrawCircle(item, 200, Color.Red, 1, 30, false);
                }               
            }

            if (Menu.Item("DrawRPred").GetValue<bool>())
            {
                if (R.LSIsReady())
                {
                    var target2 = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                    if (target2 == null)
                    {
                        return;
                    }
                    var prediction2 = R.GetPrediction(target2);
                    if (prediction2.Hitchance >= HitChance.High)
                    {

                        Drawing.DrawCircle(prediction2.CastPosition, 350, Color.Aqua);
                    }
                }          
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Menu.Item("AFKmode").GetValue<bool>())
            {
                if (Cords.Count != 0)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Cords.FirstOrDefault());
                    if (Cords.FirstOrDefault().LSTo2D() == Player.Position.LSTo2D())
                    {
                        Cords.RemoveAt(0);
                    }
                }
            }          
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                LastHit();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Mixed();
            }
            if (Menu.Item("AutoWNearTower").GetValue<bool>())
            {
                AutoWNearTower();
            }
            if (Menu.Item("HealAlly").GetValue<bool>())
            {
                HealAlly();
            }
            if (Menu.Item("SaveAlly").GetValue<bool>())
            {
                SaveAlly();
            }
            if (Menu.Item("UseRcomboAuto").GetValue<bool>())
            {
                UseRcomboAuto();
            }
            if (Menu.Item("UseRcomboTower").GetValue<bool>())
            {
                UseRcomboTower();
            }
        }

        private static void UseRcomboTower()
        {
            if (Player.LSUnderTurret(true))
            {
                var enemyturret = ObjectManager.Get<Obj_AI_Turret>().Where(turret => turret.IsEnemy && !turret.IsDead).OrderBy(turret => Player.LSDistance(turret.ServerPosition)).FirstOrDefault();
                if (enemyturret.IsAttackingPlayer)
                {
                    if (Player.HealthPercent <= Menu.Item("UseRcomboTowerHP").GetValue<Slider>().Value)
                    {
                        R.Cast(enemyturret.Position);
                    }
                }
            }              
        }

        private static void UseRcomboAuto()
        {
            var target2 = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            var prediction2 = R.GetPrediction(target2);

            if (prediction2.Hitchance >= HitChance.High)
            {
                var hits = HeroManager.Enemies.Where(x => x.LSDistance(target2) <= 350f).ToList();
                if (hits.Any(hit => hits.Count >= Menu.Item("UseRcomboCountAuto").GetValue<Slider>().Value))
                {
                    R.Cast(prediction2.CastPosition);
                }
            }
        }

        private static void HealAlly() 
        {
            var target = HeroManager.Allies.OrderBy(x => x.Health).FirstOrDefault(f => f.LSDistance(Player.Position) < W.Range);
            if (Player.LSIsRecalling())
            {
                return;
            }
            if (Menu.Item("WSettings." + target.ChampionName.ToLower()).GetValue<bool>())
            {
                if (target.HealthPercent <= Menu.Item("WSettingsHealthPercent." + target.ChampionName.ToLower()).GetValue<Slider>().Value)
                {
                    W.CastOnUnit(target);
                }
            }    
        }

        private static void SaveAlly()
        {
            var target = HeroManager.Allies.OrderBy(x => x.Health).FirstOrDefault(f => f.LSDistance(Player.Position) < W.Range);
            if (target == null)
            {
                return;
            }
            if (Menu.Item("RSettings." + target.ChampionName.ToLower()).GetValue<bool>())
            {
                if (target.HealthPercent <= Menu.Item("RSettingsHealthPercent." + target.ChampionName.ToLower()).GetValue<Slider>().Value)
                {
                    if (target.LSCountAlliesInRange(600) >= 1 || target.LSCountEnemiesInRange(600) >= 1)
                    {
                        R.CastIfHitchanceEquals(target, HitChance.Medium);
                    }                
                }
            }
        }

        private static void OnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Menu.Item("SupportMode").GetValue<bool>())
            {
                if (args.Target.Type == GameObjectType.obj_AI_Minion && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    args.Process = false;
                }
            }          
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Menu.Item("UseQinterrupt").GetValue<bool>())
            {            
                if (Menu.Item("UseQinterruptStun").GetValue<bool>())
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                    var prediction = Q.GetPrediction(target);

                    if (IsCollisionable(target))
                    {
                        Q.CastOnUnit(target);
                    }
                }
                else
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                    var prediction = Q.GetPrediction(target);

                    Q.CastIfHitchanceEquals(target, HitChance.Medium);
                }
                
            }
            if (Menu.Item("UseRinterrupt").GetValue<bool>() && args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

                var prediction = R.GetPrediction(target);

                R.CastIfHitchanceEquals(target, HitChance.Medium);
            }
        }

        private static bool IsCollisionable(AIHeroClient target)
        {
            var prediction = Q.GetPrediction(target);

            for (var i = 15; i < 450; i += 75)
            {
                if (i > 450) return false;
                var posCF = NavMesh.GetCollisionFlags(prediction.UnitPosition.LSTo2D().LSExtend(ObjectManager.Player.ServerPosition.LSTo2D(), -i).To3D());
                if (posCF.HasFlag(CollisionFlags.Wall) || posCF.HasFlag(CollisionFlags.Building))
                {
                    return true;
                }
            }
            return false;

        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("UseQGapCloser").GetValue<bool>())
            {
                if (gapcloser.Sender.LSIsValidTarget(Q.Range))
                {
                    Render.Circle.DrawCircle(gapcloser.Sender.Position, gapcloser.Sender.BoundingRadius, Color.Gold, 5);
                    var targetpos = Drawing.WorldToScreen(gapcloser.Sender.Position);
                    Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "Gapcloser");
                    Q.Cast(gapcloser.Sender.ServerPosition);
                }
            }
        }   

        private static void AutoWNearTower()
        {
            var myturret = ObjectManager.Get<Obj_AI_Turret>().Where(turret => turret.IsAlly && !turret.IsDead).OrderBy(turret => Player.LSDistance(turret.ServerPosition)).FirstOrDefault();
            if (Player.LSUnderAllyTurret())
            {
                if (Player.ManaPercent >= Menu.Item("AutoWNearTowerMana").GetValue<Slider>().Value)
                {
                    W.Cast(myturret.Position);
                }
            }
        }

        private static void Mixed()
        {
            if (Menu.Item("UseQmixed").GetValue<bool>())
            {
                if (Player.ManaPercent >= Menu.Item("UseQMixedMana").GetValue<Slider>().Value)
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                    var Prediction = R.GetPrediction(target, true);
                    var AoePrediction = Prediction.AoeTargetsHitCount;
                    var count = Menu.Item("UseQMixedCount").GetValue<Slider>().Value;
                    if (count == 1)
                    {
                        Q.Cast(target);
                    }
                    else
                    {
                        if (AoePrediction >= count)
                        {
                            Q.Cast(Prediction.CastPosition);
                        }
                    }                   
                }
            }
        }

        private static void LaneClear()
        {
            if (Menu.Item("UseQlaneClear").GetValue<bool>())
            {
                if (Player.ManaPercent >= Menu.Item("UseQLaneClearMana").GetValue<Slider>().Value)
                {
                    var minions = MinionManager.GetMinions(Q.Range);
                    var QPrediction = Q.GetLineFarmLocation(minions);
                    if (QPrediction.MinionsHit >= Menu.Item("UseQLaneClearCount").GetValue<Slider>().Value)
                    {
                        Q.Cast(QPrediction.Position);
                    }
                }
            }
        }

        private static void LastHit()
        {
            if (Menu.Item("UseQlasthit").GetValue<bool>())
            {
                if (Player.ManaPercent >= Menu.Item("UseQlasthitMana").GetValue<Slider>().Value)
                {
                    var minions = MinionManager.GetMinions(Q.Range);
                    if (minions.Count == 0)
                    {
                        return;
                    }
                    var minion = minions.FirstOrDefault();
                    if (minion.Health <= Q.GetDamage(minion))
                    {
                        Q.Cast(minion);
                    }
                }
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (Menu.Item("UseQcombo").GetValue<bool>())
            {
                if (Menu.Item("UseQcomboStun").GetValue<bool>())
                {
                    if (IsCollisionable(target))
                    {
                        Q.Cast(target);
                    }
                }
                else
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.Medium)
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }

            if (Menu.Item("UseEcombo").GetValue<bool>())
            {
                var pred = ObjectManager.Player.ServerPosition.LSTo2D() + ObjectManager.Player.Direction.LSTo2D().LSPerpendicular() * (ObjectManager.Player.BoundingRadius * 2.5f);
                if (LeagueSharp.Common.Utility.LSIsWall(pred))
                {
                    var targetpos = Drawing.WorldToScreen(Player.ServerPosition);
                    Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, Color.Gold, "wall");
                    E.Cast(Game.CursorPos);
                }
            }

            if (Menu.Item("UseRcombo").GetValue<bool>())
            {
                var target2 = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                var prediction2 = R.GetPrediction(target2);

                if (prediction2.Hitchance >= HitChance.High)
                {
                    var hits = HeroManager.Enemies.Where(x => x.LSDistance(target2) <= 350f).ToList();
                    if (hits.Any(hit => hits.Count >= Menu.Item("UseRcomboCount").GetValue<Slider>().Value))
                    {
                        R.Cast(prediction2.CastPosition);
                    }
                }

            }
        }
    }
}
