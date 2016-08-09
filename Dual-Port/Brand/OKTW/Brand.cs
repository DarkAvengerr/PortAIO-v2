using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Brand
    {
        private Menu Config = Program.Config;
        public static SebbyLib.Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;
        private Spell Q, W, E, R;
        private float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        public AIHeroClient Player { get { return ObjectManager.Player; } }
        public void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, 940);
            E = new Spell(SpellSlot.E, 625);
            R = new Spell(SpellSlot.R, 750);

            Q.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(1.15f, 230f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetTargetted(0.25f, 2000f);

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("noti", "Show notification & line", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("QAblazed", "Q only if ablazed", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("harrasQ", "Harass Q", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("gapQ", "Gapcloser E + Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("intQ", "Interrupt spells E + Q", true).SetValue(true));


            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("autoW", "Auto W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("harrasW", "Harass W", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("harrasE", "Harass E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("minionE", "use E on ablazed minion", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("rCount", "Auto R if can hit x enemies", true).SetValue(new Slider(3, 0, 5)));

            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("Harass").AddItem(new MenuItem("harras" + enemy.ChampionName, enemy.ChampionName).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmE", "Lane clear E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmW", "Lane clear W", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("Mana", "LaneClear Mana", true).SetValue(new Slider(80, 100, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("LCminions", "LaneClear minimum minions", true).SetValue(new Slider(2, 10, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleE", "Jungle clear E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleW", "Jungle clear W", true).SetValue(true));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Config.Item("gapQ", true).GetValue<bool>() || Player.Mana <  QMANA + EMANA)
                return;

            var t = gapcloser.Sender;

            if ( t.IsValidTarget(E.Range) && (t.HasBuff("brandablaze") || E.IsReady()))
            {

                E.CastOnUnit(t);
                if (Q.IsReady())
                    Q.Cast(t);
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient t, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Config.Item("intQ", true).GetValue<bool>() || Player.Mana < QMANA + EMANA)
                return;

            if (t.IsValidTarget(E.Range) && (t.HasBuff("brandablaze") || E.IsReady()))
            { 
                E.CastOnUnit(t);
                if (Q.IsReady())
                    Q.Cast(t);
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.Combo)
            {
                if (!E.IsReady())
                    SebbyLib.Orbwalking.Attack = true;

                else
                    SebbyLib.Orbwalking.Attack = false;
            }
            else
                SebbyLib.Orbwalking.Attack = true;

            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }

            if (Program.LagFree(1) && E.IsReady() && Config.Item("autoE", true).GetValue<bool>())
                LogicE();
            if (Program.LagFree(2) && Q.IsReady() && Config.Item("autoQ", true).GetValue<bool>())
                LogicQ();
            if (Program.LagFree(3) && W.IsReady()  && Config.Item("autoW", true).GetValue<bool>())
                LogicW();
            if (Program.LagFree(4) && R.IsReady() && Config.Item("autoR", true).GetValue<bool>())
                LogicR();
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (OktwCommon.GetKsDamage(t, Q) + BonusDmg(t) + OktwCommon.GetEchoLudenDamage(t) > t.Health)
                    Program.CastSpell(Q, t);

                if (!t.HasBuff("brandablaze") && Config.Item("QAblazed", true).GetValue<bool>())
                {
                    var otherEnemy = t;

                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && enemy.HasBuff("brandablaze")))
                        t = enemy;

                    if (otherEnemy == t && !LogicQuse(t))
                        return;
                }

                if(Program.Combo && Player.Mana > RMANA + QMANA )
                    Program.CastSpell(Q, t);
                else if (Program.Farm && Config.Item("harrasQ", true).GetValue<bool>() && Player.Mana > RMANA + EMANA + WMANA + EMANA)
                    Program.CastSpell(Q, t);

                if (Player.Mana > RMANA + QMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy);
                }
            }
        }

        private bool LogicQuse(Obj_AI_Base t)
        {
            if (t.HasBuff("brandablaze"))
                return true;
            else if (E.Instance.CooldownExpires - Game.Time + 2 >= Q.Instance.Cooldown && W.Instance.CooldownExpires - Game.Time + 2 >= Q.Instance.Cooldown)
                return true;
            else
                return false;
        }

        private void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + WMANA)
                    Program.CastSpell(W, t);
                else if (Program.Farm && Config.Item("harrasW", true).GetValue<bool>() && Config.Item("harras" + t.ChampionName).GetValue<bool>()  && Player.Mana > RMANA + WMANA + EMANA + QMANA + WMANA && OktwCommon.CanHarras())
                    Program.CastSpell(W, t);
                else
                {
                    var qDmg = Q.GetDamage(t);
                    var wDmg = OktwCommon.GetKsDamage(t, W) + BonusDmg(t);
                    if (wDmg > t.Health)
                    {
                        Program.CastSpell(W, t);
                    }
                    else if (wDmg + qDmg > t.Health && Player.Mana > QMANA + EMANA)
                        Program.CastSpell(W, t);
                }

                if (Player.Mana > RMANA + WMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                        W.Cast(enemy, true);
                }
            }
            else if (Program.LaneClear && Player.ManaPercent> Config.Item("Mana", true).GetValue<Slider>().Value && Config.Item("farmW", true).GetValue<bool>() && Player.Mana > RMANA + QMANA + WMANA)
            {
                var allMinions = Cache.GetMinions(Player.ServerPosition, W.Range);
                var farmPos = W.GetCircularFarmLocation(allMinions, W.Width);
                if (farmPos.MinionsHit >= Config.Item("LCminions", true).GetValue<Slider>().Value)
                    W.Cast(farmPos.Position);
            }
        }

        private void LogicE()
        {
            var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + EMANA)
                    E.CastOnUnit(t);
                else if (Program.Farm && Config.Item("harrasE", true).GetValue<bool>() && Player.Mana > RMANA + EMANA + WMANA + EMANA)
                    E.CastOnUnit(t);
                else
                {
                    var eDmg = OktwCommon.GetKsDamage(t, E) + BonusDmg(t) + OktwCommon.GetEchoLudenDamage(t);
                    var wDmg = W.GetDamage(t);
                    if (eDmg > t.Health)
                        E.CastOnUnit(t);
                    else if (wDmg + eDmg > t.Health && Player.Mana > WMANA + EMANA)
                        E.CastOnUnit(t);
                }
            }
            else 
            {
                if(Config.Item("minionE", true).GetValue<bool>())
                {
                    if ((Program.Combo && Player.Mana > RMANA + EMANA) || (Program.Farm && Config.Item("harrasE", true).GetValue<bool>() && Player.Mana > RMANA + EMANA ))
                    {
                        foreach (var minion in Cache.GetMinions(Player.Position, E.Range).Where(minion => minion.IsValidTarget(E.Range) && minion.CountEnemiesInRange(300) > 0 && minion.HasBuff("brandablaze")))
                        {
                            E.CastOnUnit(minion);
                        }
                    }
                    if (Program.LaneClear && Player.ManaPercent > Config.Item("Mana", true).GetValue<Slider>().Value && Config.Item("farmE", true).GetValue<bool>() && Player.Mana > RMANA + EMANA)
                    {
                        foreach (var minion in Cache.GetMinions(Player.Position, E.Range).Where(minion => minion.IsValidTarget(E.Range) && minion.HasBuff("brandablaze") && CountMinionsInRange(400,minion.Position) >= Config.Item("LCminions", true).GetValue<Slider>().Value))
                        {
                            E.CastOnUnit(minion);
                        }
                    }
                }
            }
        }

        private void LogicR()
        {
            var bounceRange = 430;
            var t2 = TargetSelector.GetTarget(R.Range + bounceRange, TargetSelector.DamageType.Magical);

            if (t2.IsValidTarget(R.Range) && t2.CountEnemiesInRange(bounceRange) >= Config.Item("rCount", true).GetValue<Slider>().Value && Config.Item("rCount", true).GetValue<Slider>().Value > 0)
                R.Cast(t2);

            if (t2.IsValidTarget() && OktwCommon.ValidUlt(t2))
            {
                if (t2.CountAlliesInRange(550) == 0 || Player.HealthPercent < 50 || t2.CountEnemiesInRange(bounceRange) > 1)
                {
                    var prepos = R.GetPrediction(t2).CastPosition;
                    var dmgR = R.GetDamage(t2);

                    if (t2.Health < dmgR * 3)
                    {
                        var totalDmg = dmgR;
                        var minionCount = CountMinionsInRange(bounceRange, prepos);

                        if (t2.IsValidTarget(R.Range))
                        {
                            if (prepos.CountEnemiesInRange(bounceRange) > 1)
                            {
                                if (minionCount > 2)
                                    totalDmg = dmgR * 2;
                                else
                                    totalDmg = dmgR * 3;
                            }
                            else if (minionCount > 0)
                            {
                                totalDmg = dmgR * 2;
                            }

                            if (W.IsReady())
                            {
                                totalDmg += W.GetDamage(t2);
                            }

                            if (E.IsReady())
                            {
                                totalDmg += E.GetDamage(t2);
                            }

                            if (Q.IsReady())
                            {
                                totalDmg += Q.GetDamage(t2);
                            }

                            totalDmg += BonusDmg(t2);
                            totalDmg += OktwCommon.GetEchoLudenDamage(t2);

                            if (Items.HasItem(3155, t2))
                            {
                                totalDmg = totalDmg - 250;
                            }

                            if (Items.HasItem(3156, t2))
                            {
                                totalDmg = totalDmg - 400;
                            }

                            if (totalDmg > t2.Health - OktwCommon.GetIncomingDamage(t2) && Player.GetAutoAttackDamage(t2) * 2 < t2.Health)
                            {

                                R.CastOnUnit(t2);
                            }

                        }
                        else if (t2.Health - OktwCommon.GetIncomingDamage(t2) < dmgR * 2 + BonusDmg(t2))
                        {
                            if (Player.CountEnemiesInRange(R.Range) > 0)
                            {
                                foreach (var t in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(R.Range) && enemy.Distance(prepos) < bounceRange))
                                {
                                    R.CastOnUnit(t);
                                }
                            }
                            else
                            {
                                var minions = Cache.GetMinions(Player.Position, R.Range);
                                foreach (var minion in minions.Where(minion => minion.IsValidTarget(R.Range) && minion.Distance(prepos) < bounceRange))
                                {
                                    R.CastOnUnit(minion);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + WMANA + RMANA + WMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (W.IsReady() && Config.Item("jungleW", true).GetValue<bool>())
                    {
                        W.Cast(mob.ServerPosition);
                        return;
                    }
                    
                    if (Q.IsReady() && Config.Item("jungleQ", true).GetValue<bool>())
                    {
                        Q.Cast(mob.ServerPosition);
                        return;
                    }

                    if (E.IsReady() && Config.Item("jungleE", true).GetValue<bool>() && mob.HasBuff("brandablaze"))
                    {
                        E.Cast(mob);
                        return;
                    }
                }
            }
        }

        private int CountMinionsInRange(float range, Vector3 pos)
        {
            var minions = Cache.GetMinions(pos, range);
            int count = 0;
            foreach (var minion in minions)
            {
                count++;
            }
            return count;
        }

        private float BonusDmg(AIHeroClient target)
        {
            return (float)Player.CalcDamage(target, Damage.DamageType.Magical, (target.MaxHealth * 0.08) - (target.HPRegenRate * 5));
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

            if (!R.IsReady())
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

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("qRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }

            if (Config.Item("wRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (W.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
            }

            if (Config.Item("eRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
            }

            if (Config.Item("rRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }

            if (Config.Item("noti", true).GetValue<bool>() && R.IsReady())
            {
                var t = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);

                if (t.IsValidTarget())
                {
                    var rDamage = R.GetDamage(t);
                    if (rDamage * 3 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "3 x Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Yellow);
                    }
                    else if (rDamage * 2 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "2 x Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Yellow);
                    }
                    else if (rDamage > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "1 x Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Yellow);
                    }
                }
            }
        }
    }
}
