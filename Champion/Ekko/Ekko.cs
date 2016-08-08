using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy;

namespace OneKeyToWin_AIO_Sebby
{
    class Ekko
    {
        private Menu Config = Program.Config;
        public static SebbyLib.Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;
        private Spell E, Q, Q1,  R, W;
        private AIHeroClient Player { get { return ObjectManager.Player; } }
        private float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0, Wtime = 0, Wtime2 = 0;
        private static GameObject RMissile, WMissile2, WMissile;
        public static Core.MissileReturn missileManager;

        public void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 750); 
            Q1 = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, 1620);
            E = new Spell(SpellSlot.E, 330f);
            R = new Spell(SpellSlot.R, 280f);

            Q.SetSkillshot(0.25f, 60f, 1650f, false, SkillshotType.SkillshotLine);
            Q1.SetSkillshot(0.5f, 150f, 1000f, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(2.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.4f, 280f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            missileManager = new Core.MissileReturn("ekkoqmis", "ekkoqreturn", Q);

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("Qhelp", "Show Q,W helper", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            
            Config.SubMenu(Player.ChampionName).SubMenu("W option").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W option").AddItem(new MenuItem("Waoe", "Cast if 2 targets", true).SetValue(false));

            Config.SubMenu(Player.ChampionName).SubMenu("R option").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R option").AddItem(new MenuItem("rCount", "Auto R if enemies in range", true).SetValue(new Slider(3, 0, 5)));

            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("Harras").AddItem(new MenuItem("haras" + enemy.ChampionName, enemy.ChampionName).SetValue(true));                            

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "Lane clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmW", "Farm W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("LCminions", " LaneClear minimum minions", true).SetValue(new Slider(2, 10, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("Mana", "LaneClear Mana", true).SetValue(new Slider(80, 100, 30)));

            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            
            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }

            if (Program.LagFree(1) && Q.LSIsReady() )
                LogicQ();
            if (Program.LagFree(2) && W.LSIsReady() && Config.Item("autoW", true).GetValue<bool>() && Player.Mana > RMANA + WMANA + EMANA + QMANA)
                LogicW();
            if (Program.LagFree(3) && E.LSIsReady() )
                LogicE();
            if ( R.LSIsReady() )
                LogicR();
        }

        private void LogicR()
        {
            if (Config.Item("autoR", true).GetValue<bool>())
            {
                if (Program.LagFree(4) && Program.Combo && RMissile != null && RMissile.IsValid)
                {
                    if (RMissile.Position.LSCountEnemiesInRange(R.Range) >= Config.Item("rCount", true).GetValue<Slider>().Value && Config.Item("rCount", true).GetValue<Slider>().Value > 0)
                        R.Cast();

                    foreach (var t in HeroManager.Enemies.Where(t => t.LSIsValidTarget() && RMissile.Position.LSDistance(Prediction.GetPrediction(t, R.Delay).CastPosition) < R.Range && RMissile.Position.LSDistance(t.ServerPosition) < R.Range))
                    {
                        var comboDMG = OktwCommon.GetKsDamage(t, R);

                        if (Q.LSIsReady())
                            comboDMG += Q.GetDamage(t);

                        if (E.LSIsReady())
                            comboDMG += E.GetDamage(t);

                        if (W.LSIsReady())
                            comboDMG += W.GetDamage(t);

                        if (t.Health < comboDMG && OktwCommon.ValidUlt(t))
                            R.Cast();

                        Program.debug("ks");

                    }
                }

                double dmg = OktwCommon.GetIncomingDamage(Player, 1);
                var enemys = Player.LSCountEnemiesInRange(800);

                if (dmg > 0 || enemys > 0)
                {
                    if (dmg > Player.Level * 50)
                    {
                        R.Cast();
                    }
                    else if (Player.Health - dmg < enemys * Player.Level * 20)
                    {
                        R.Cast();

                    }
                    else if (Player.Health - dmg < Player.Level * 10)
                    {
                        R.Cast();
                    }
                } 
            }
        }

        private void LogicE()
        {
            if (Program.Combo && WMissile != null && WMissile.IsValid)
            {
                if (WMissile.Position.LSCountEnemiesInRange(200) > 0 && WMissile.Position.LSDistance(Player.ServerPosition) < 100)
                {
                    E.Cast(Player.Position.LSExtend(WMissile.Position, E.Range), true);
                }
            }

            var t = TargetSelector.GetTarget(800, TargetSelector.DamageType.Magical);

            if (E.LSIsReady() && Player.Mana > RMANA + EMANA
                 && Player.LSCountEnemiesInRange(260) > 0
                 && Player.Position.LSExtend(Game.CursorPos, E.Range).LSCountEnemiesInRange(500) < 3
                 && t.Position.LSDistance(Game.CursorPos) > t.Position.LSDistance(Player.Position))
            {
                E.Cast(Player.Position.LSExtend(Game.CursorPos, E.Range), true);
            }
            else if (Program.Combo && Player.Health > Player.MaxHealth * 0.4
                && Player.Mana > RMANA + EMANA
                && !Player.LSUnderTurret(true)
                && Player.Position.LSExtend(Game.CursorPos, E.Range).LSCountEnemiesInRange(700) < 3)
            {
                if (t.LSIsValidTarget() && Player.Mana > QMANA + EMANA + WMANA && t.Position.LSDistance(Game.CursorPos) + 300 < t.Position.LSDistance(Player.Position))
                {
                    E.Cast(Player.Position.LSExtend(Game.CursorPos, E.Range), true);
                }
            }
            else if (t.LSIsValidTarget() && Program.Combo  && E.GetDamage(t) + W.GetDamage(t) > t.Health)
            {
                E.Cast(Player.Position.LSExtend(t.Position, E.Range), true);
            }
        }

        private void Jungle()
        {
            if (Program.LaneClear && Player.Mana > QMANA + RMANA )
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 500 , MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (W.LSIsReady() && Config.Item("jungleW", true).GetValue<bool>())
                    {
                        W.Cast(mob.Position);
                        return;
                    }
                    if (Q.LSIsReady() && Config.Item("jungleQ", true).GetValue<bool>())
                    {
                        Q.Cast(mob.Position);
                        return;
                    }  
                }
            }
        }
        private void Obj_AI_Base_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsValid  )
            {
                if (obj.Name == "Ekko" && obj.IsAlly)
                    RMissile = obj;
                if (obj.Name == "Ekko_Base_W_Indicator.troy")
                {
                    WMissile = obj;
                    Wtime = Game.Time;
                }
                if (obj.Name == "Ekko_Base_W_Cas.troy")
                {
                    WMissile2 = obj;
                    Wtime2 = Game.Time;
                }
            }     
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var t1 = TargetSelector.GetTarget(Q1.Range, TargetSelector.DamageType.Magical);
            if (t.LSIsValidTarget())
            {
                missileManager.Target = t;
                if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Program.CastSpell(Q, t);
                else if (Program.Farm && Config.Item("haras" + t.ChampionName).GetValue<bool>() && Player.Mana > RMANA + WMANA + QMANA + QMANA && OktwCommon.CanHarras())
                        Program.CastSpell(Q, t);
                else if (OktwCommon.GetKsDamage(t, Q) * 2 > t.Health)
                    Program.CastSpell(Q, t);
                if (Player.Mana > RMANA + QMANA + WMANA )
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.LSIsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy,true,true);
                }

            }
            else if (t1.LSIsValidTarget())
            {
                missileManager.Target = t1;
                if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Program.CastSpell(Q1, t1);
                else if (Program.Farm && Config.Item("haras" + t1.ChampionName).GetValue<bool>() && Player.Mana > RMANA + WMANA + QMANA + QMANA && OktwCommon.CanHarras())
                    Program.CastSpell(Q1, t1);
                else if (OktwCommon.GetKsDamage(t1, Q1) * 2 > t1.Health)
                    Program.CastSpell(Q1, t1);
                if (Player.Mana > RMANA + QMANA + WMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.LSIsValidTarget(Q1.Range) && !OktwCommon.CanMove(enemy)))
                        Q1.Cast(enemy, true, true);
                }
            }
            else if (Program.LaneClear && Player.ManaPercent > Config.Item("Mana", true).GetValue<Slider>().Value && Config.Item("farmQ", true).GetValue<bool>() && Player.Mana > RMANA + QMANA + WMANA)
            {
                var allMinionsQ = Cache.GetMinions(Player.ServerPosition, Q1.Range);
                var Qfarm = Q.GetLineFarmLocation(allMinionsQ, 100);
                if (Qfarm.MinionsHit >= Config.Item("LCminions", true).GetValue<Slider>().Value)
                    Q.Cast(Qfarm.Position);
            }
            
        }

        private void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (t.LSIsValidTarget() )
            {
                
                if (Config.Item("Waoe", true).GetValue<bool>())
                {
                    W.CastIfWillHit(t, 2, true);
                    if (t.LSCountEnemiesInRange(250) > 1)
                    {
                        Program.CastSpell(W, t);
                    }
                }
                    
                if (Program.Combo  && W.GetPrediction(t).CastPosition.LSDistance(t.Position) > 200)
                    Program.CastSpell(W, t);
            }
            if (!Program.None)
            {
                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.LSIsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                    W.Cast(enemy, true, true);
            }
        }
        private void SetMana()
        {
            if ((Config.Item("manaDisable", true).GetValue<bool>() && Program.Combo) || Player.HealthPercent < 20)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
                return;
            }

            QMANA = Q.Instance.SData.Mana;
            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;

            if (!R.LSIsReady())
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        public static void drawText2(string msg, Vector3 Hero, System.Drawing.Color color)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] - 200, color, msg);
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if ( Config.Item("Qhelp", true).GetValue<bool>())
            {
                if (WMissile != null && WMissile.IsValid)
                {
                    LeagueSharp.Common.Utility.DrawCircle(WMissile.Position, 300, System.Drawing.Color.Yellow, 1, 1);
                    drawText2("W:  " + String.Format("{0:0.0}", Wtime + 3 - Game.Time), WMissile.Position, System.Drawing.Color.White);

                }
                if (WMissile2 != null && WMissile2.IsValid)
                {
                    LeagueSharp.Common.Utility.DrawCircle(WMissile2.Position, 300, System.Drawing.Color.Red, 1, 1);
                    drawText2("W:  " + String.Format("{0:0.0}", Wtime2 + 1 - Game.Time), WMissile2.Position, System.Drawing.Color.Red);

                }
            }

            if (Config.Item("qRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.LSIsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            if (Config.Item("wRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (W.LSIsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                
            }
            if (Config.Item("eRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (E.LSIsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, 800, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, 800, System.Drawing.Color.Yellow, 1, 1);
            }
            if (Config.Item("rRange", true).GetValue<bool>())
            {
                if (RMissile != null && RMissile.IsValid)
                {
                    if (Config.Item("rRange", true).GetValue<bool>())
                    {
                        if (Config.Item("onlyRdy", true).GetValue<bool>())
                        {
                            if (R.LSIsReady())
                                LeagueSharp.Common.Utility.DrawCircle(RMissile.Position, R.Width, System.Drawing.Color.YellowGreen, 1, 1);
                        }
                        else
                            LeagueSharp.Common.Utility.DrawCircle(RMissile.Position, R.Width, System.Drawing.Color.YellowGreen, 1, 1);

                        drawLine(RMissile.Position, Player.Position, 10, System.Drawing.Color.YellowGreen);
                    }
                }
            }
        }
    }
}
