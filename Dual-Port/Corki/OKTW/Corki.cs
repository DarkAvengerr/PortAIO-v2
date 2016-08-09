using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Corki
    {
        private Menu Config = Program.Config;
        public static SebbyLib.Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;
        private Spell E, Q, R, W;
        private float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        public AIHeroClient Player { get { return ObjectManager.Player; } }

        public void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 825);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 1230);
           
            Q.SetSkillshot(0.3f, 200f, 1000f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("wRange", "W range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("eRange", "E range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRange", "R range", true).SetValue(false));

            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q Config").AddItem(new MenuItem("harassQ", "Q harass", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("nktdE", "NoKeyToDash", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("autoE", "Auto E", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("E Config").AddItem(new MenuItem("harassE", "E harass", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("Rammo", "Minimum R ammo harass", true).SetValue(new Slider(3, 6, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("minionR", "Try R on minion", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("useR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space
            
            foreach (var enemy in HeroManager.Enemies)
                Config.SubMenu(Player.ChampionName).SubMenu("Harass").AddItem(new MenuItem("harras" + enemy.ChampionName, enemy.ChampionName).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("LCminions", "LaneClear minimum minions", true).SetValue(new Slider(2, 10, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("RammoLC", "Minimum R ammo Lane clear", true).SetValue(new Slider(3, 6, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "LaneClear + jungle Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmR", "LaneClear + jungle  R", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("Mana", "LaneClear  Mana", true).SetValue(new Slider(80, 100, 30)));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            SebbyLib.Orbwalking.BeforeAttack += BeforeAttack;
        }

        private void BeforeAttack(SebbyLib.Orbwalking.BeforeAttackEventArgs args)
        {
            if (E.IsReady() && Sheen() && args.Target.IsValid<AIHeroClient>())
            {
                if(Program.Combo && Config.Item("autoE", true).GetValue<bool>() && Player.Mana > EMANA + RMANA)
                    E.Cast(args.Target.Position);
                if (Program.Farm && Config.Item("harassE", true).GetValue<bool>() && Player.Mana > EMANA + RMANA + QMANA && OktwCommon.CanHarras())
                    E.Cast(args.Target.Position);
                if (!Q.IsReady() && !R.IsReady() && args.Target.Health < Player.FlatPhysicalDamageMod * 2)
                    E.Cast();
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
            {
                SetMana();
                farm();
            }
            if (Program.LagFree(1) && Q.IsReady() && !Player.Spellbook.IsAutoAttacking && Sheen())
                LogicQ();
            if (Program.LagFree(2) && Program.Combo && W.IsReady())
                LogicW();
            if (Program.LagFree(4) && R.IsReady() && !Player.Spellbook.IsAutoAttacking && Sheen() && !Player.Spellbook.IsAutoAttacking)
                LogicR();
        }

        private void LogicR()
        {
            float rSplash = 150;
            if (bonusR)
            {
                rSplash = 300;
            }
            
            var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (t.IsValidTarget())
            {
                var rDmg = OktwCommon.GetKsDamage(t,R);
                var qDmg = Q.GetDamage(t);
                if (rDmg * 2> t.Health)
                    CastR(R, t);
                else if (t.IsValidTarget(Q.Range) && qDmg + rDmg > t.Health)
                    CastR(R, t);
                if (Player.Spellbook.GetSpell(SpellSlot.R).Ammo > 1)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(R.Range) && enemy.CountEnemiesInRange(rSplash) > 1))
                        t = enemy;

                    if (Program.Combo && Player.Mana > RMANA * 3 )
                    {
                        CastR(R, t);
                    }
                    else if (Program.Farm && Player.Mana > RMANA + EMANA + QMANA + WMANA && Player.Spellbook.GetSpell(SpellSlot.R).Ammo >= Config.Item("Rammo", true).GetValue<Slider>().Value && OktwCommon.CanHarras())
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(R.Range) && Config.Item("harras" + enemy.ChampionName).GetValue<bool>()))
                            CastR(R, enemy);
                    }

                    if (!Program.None && Player.Mana > RMANA + QMANA + EMANA)
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(R.Range) && !OktwCommon.CanMove(enemy)))
                            CastR(R, t);
                    }
                }
            }
        }

        private void CastR(Spell R , AIHeroClient t)
        {
            Program.CastSpell(R, t);
            if (Config.Item("minionR", true).GetValue<bool>())
            {
                // collision + predictio R
                var poutput = R.GetPrediction(t);
                var col = poutput.CollisionObjects.Count(ColObj => ColObj.IsEnemy && ColObj.IsMinion && !ColObj.IsDead);

                //hitchance
                var prepos = Prediction.GetPrediction(t, 0.4f);

                if (col == 0 && (int)prepos.Hitchance < 5)
                    return;

                float rSplash = 140;
                if (bonusR)
                    rSplash = 290f;
                
                var minions = Cache.GetMinions(Player.ServerPosition, R.Range - rSplash);
                foreach (var minion in minions.Where(minion => minion.Distance(poutput.CastPosition) < rSplash))
                {
                    R.Cast(minion);
                    return;
                }
            }
        }

        private void LogicW()
        {
            var dashPosition = Player.Position.Extend(Game.CursorPos, W.Range);

            if (Game.CursorPos.Distance(Player.Position) > Player.AttackRange + Player.BoundingRadius * 2 && Program.Combo && Config.Item("nktdE", true).GetValue<bool>() && Player.Mana > RMANA + WMANA - 10)
            {
                W.Cast(dashPosition);
            }
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Config.Item("autoQ", true).GetValue<bool>() && Player.Mana > RMANA + QMANA)
                    Program.CastSpell(Q, t);
                else if (Program.Farm && Config.Item("harassQ", true).GetValue<bool>() && Config.Item("harras" + t.ChampionName).GetValue<bool>() && Player.Mana > RMANA + EMANA + WMANA + RMANA && OktwCommon.CanHarras())
                    Program.CastSpell(Q, t);
                else
                {
                    var qDmg = OktwCommon.GetKsDamage(t, Q);
                    var rDmg = R.GetDamage(t);
                    if (qDmg > t.Health)
                        Q.Cast(t);
                    else if (rDmg + qDmg > t.Health && Player.Mana > RMANA + QMANA)
                        Program.CastSpell(Q, t);
                    else if (rDmg + 2 * qDmg > t.Health && Player.Mana > QMANA + RMANA * 2)
                        Program.CastSpell(Q, t);
                }

                if (!Program.None && Player.Mana > RMANA + WMANA + EMANA)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy, true, true);
                }
            }
        }
        public void farm()
        {
            if (Program.LaneClear && !Player.Spellbook.IsAutoAttacking && Sheen())
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Neutral);
                if (mobs.Count > 0 && Player.Mana > RMANA + WMANA + EMANA + QMANA)
                {
                    var mob = mobs[0];
                    if (Q.IsReady() && Config.Item("farmQ", true).GetValue<bool>())
                    {
                        Q.Cast(mob);
                        return;
                    }

                    if (R.IsReady() && Config.Item("farmR", true).GetValue<bool>())
                    {
                        R.Cast(mob);
                        return;
                    }
                }

                if ( Player.ManaPercent > Config.Item("Mana", true).GetValue<Slider>().Value)
                {
                    var minions = Cache.GetMinions(Player.ServerPosition, Q.Range);

                    if (R.IsReady() && Config.Item("farmR", true).GetValue<bool>() && Player.Spellbook.GetSpell(SpellSlot.R).Ammo >= Config.Item("RammoLC", true).GetValue<Slider>().Value)
                    {
                        var rfarm = R.GetCircularFarmLocation(minions, 100);
                        if (rfarm.MinionsHit >= Config.Item("LCminions", true).GetValue<Slider>().Value)
                        {
                            R.Cast(rfarm.Position);
                            return;
                        }
                    }
                    if (Q.IsReady() && Config.Item("farmQ", true).GetValue<bool>())
                    {
                        var qfarm = Q.GetCircularFarmLocation(minions, Q.Width);
                        if (qfarm.MinionsHit >= Config.Item("LCminions", true).GetValue<Slider>().Value)
                        {
                            Q.Cast(qfarm.Position);
                            return;
                        }
                    }
                }
            }
        }

        private bool Sheen()
        {
            var target = Orbwalker.GetTarget();

            if (target.IsValidTarget() &&  Player.HasBuff("sheen") )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool bonusR { get { return Player.HasBuff("corkimissilebarragecounterbig"); } }

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

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] - 200, color, msg);
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("nktdE", true).GetValue<bool>())
            {
                if (Game.CursorPos.Distance(Player.Position) > Player.AttackRange + Player.BoundingRadius * 2)
                    drawText("dash: ON ", Player.Position, System.Drawing.Color.Red);
                else
                    drawText("dash: OFF ", Player.Position, System.Drawing.Color.GreenYellow);
            }
            if (Config.Item("qRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            if (Config.Item("wRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (W.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
            }
            if (Config.Item("eRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
            }
            if (Config.Item("rRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }
        }
    }
}
