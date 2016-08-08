using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class TwistedFate
    {
        private Menu Config = Program.Config;
        public static SebbyLib.Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;
        private Spell Q, W, E, R;
        private float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        private string temp = null;
        private bool cardok = true;
        private int FindCard = 0;
        public AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }
        public void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 1400);
            E = new Spell(SpellSlot.E, 700);
            W = new Spell(SpellSlot.W, 1200);
            R = new Spell(SpellSlot.R, 5500);

            Q.SetSkillshot(0.25f, 40f, 1000, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1f, 40f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetTargetted(0.25f, 2000f);

            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("onlyRdy", "Draw only ready spells", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("qRange", "Q range", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("rRangeMini", "R range minimap", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("cardInfo", "Show card info", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Draw").AddItem(new MenuItem("notR", "R info helper", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Q config").AddItem(new MenuItem("autoQ", "Auto Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Q config").AddItem(new MenuItem("harrasQ", "Harass Q", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("Wmode", "W mode", true).SetValue(new StringList(new[] { "Auto", "Manual" }, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("Wgold", "Gold key", true).SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Press))); //32 == space 
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("Wblue", "Blue key", true).SetValue(new KeyBind("U".ToCharArray()[0], KeyBindType.Press))); //32 == space 
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("Wred", "RED key", true).SetValue(new KeyBind("I".ToCharArray()[0], KeyBindType.Press))); //32 == space 

            
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("WblockAA", "Block AA if seeking GOLD card", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("harasW", "Harass GOLD low range", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("W Config").AddItem(new MenuItem("ignoreW", "Ignore first card", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("useR", "Semi-manual cast R key", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))); //32 == space 
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("autoR", "Auto R", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("Renemy", "Don't R if enemy in x range", true).SetValue(new Slider(1000, 2000, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("RenemyA", "Don't R if ally in x range near target", true).SetValue(new Slider(800, 2000, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("R Config").AddItem(new MenuItem("turetR", "Don't R under turret ", true).SetValue(true));

            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("WredFarm", "LaneClear red card above % mana", true).SetValue(new Slider(80, 100, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmQ", "Lane clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("farmW", "Lane clear W Blue / Red card", true).SetValue(false));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("jungleQ", "Jungle clear Q", true).SetValue(true));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("Mana", "LaneClear Mana", true).SetValue(new Slider(80, 100, 0)));
            Config.SubMenu(Player.ChampionName).SubMenu("Farm").AddItem(new MenuItem("LCminions", "LaneClear minimum minions", true).SetValue(new Slider(2, 10, 0)));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Drawing.OnDraw += Drawing_OnDraw;
            SebbyLib.Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Game.OnWndProc += Game_OnWndProc;
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            if (args.WParam == 16)
            {
                Config.Item("Wgold", true).Show(Config.Item("Wmode", true).GetValue<StringList>().SelectedIndex == 1);
                Config.Item("Wblue", true).Show(Config.Item("Wmode", true).GetValue<StringList>().SelectedIndex == 1);
                Config.Item("Wred", true).Show(Config.Item("Wmode", true).GetValue<StringList>().SelectedIndex == 1);
                Config.Item("harasW", true).Show(Config.Item("Wmode", true).GetValue<StringList>().SelectedIndex == 0);
            }
        }

        private void Orbwalking_BeforeAttack(SebbyLib.Orbwalking.BeforeAttackEventArgs args)
        {
            if(Program.Combo && W.LSIsReady() && FindCard == 1 && W.Instance.Name != "PickACard" &&  Config.Item("WblockAA", true).GetValue<bool>())
            {
                args.Process = false;
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {

            if (!Config.Item("ignoreW", true).GetValue<bool>())
                cardok = true;

            if (W.LSIsReady())
            {
                if(Config.Item("Wmode", true).GetValue<StringList>().SelectedIndex == 0)
                    LogicW();
                else
                    LogicWmaunal();
            }
            else if (W.Instance.Name == "PickACard")
            {
                temp = null;
                cardok = false;
            }

            if(Program.LagFree(2)  && Q.LSIsReady() && Config.Item("autoQ", true).GetValue<bool>())
                LogicQ();

            if (Program.LagFree(4) && Q.LSIsReady())
                Jungle();

            if (R.LSIsReady())
            {
                if(Program.LagFree(3) && W.LSIsReady() && Config.Item("autoR", true).GetValue<bool>())
                    LogicR();

                if (Config.Item("useR", true).GetValue<KeyBind>().Active)
                {
                    if (Player.HasBuff("destiny_marker"))
                    {
                        var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                        if (t.LSIsValidTarget())
                        {
                            R.Cast(t);
                        }
                    }
                    else
                    {
                        R.Cast();
                    }
                }
            }
                //Program.debug("" + (W.Instance.CooldownExpires - Game.Time));
        }

        private void LogicWmaunal()
        {
            var wName = W.Instance.Name;
            if (wName == "PickACard" && Utils.TickCount - W.LastCastAttemptT > 150)
            {
                if (R.LSIsReady() && (Player.HasBuff("destiny_marker") || Player.HasBuff("gate")))
                {
                    FindCard = 1;
                    W.Cast();
                }
                else if (Config.Item("Wgold", true).GetValue<KeyBind>().Active)
                {
                    FindCard = 1;
                    W.Cast();
                }
                else if (Config.Item("Wblue", true).GetValue<KeyBind>().Active)
                {
                    FindCard = 2;
                    W.Cast();
                }
                else if (Config.Item("Wred", true).GetValue<KeyBind>().Active)
                {
                    FindCard = 3;
                    W.Cast();
                }
            }
            else if (Player.HasBuff("pickacard_tracker"))
            {
                if (temp == null)
                    temp = wName;
                else if (temp != wName)
                    cardok = true;

                if (cardok)
                {
                    if (R.LSIsReady() && (Player.HasBuff("destiny_marker") || Player.HasBuff("gate")))
                    {
                        FindCard = 1;
                        if (wName == "GoldCardLock")
                            W.Cast();
                    }
                    else if (FindCard == 1)
                    {
                        if (wName == "GoldCardLock")
                            W.Cast();
                    }
                    else if (FindCard == 2)
                    {
                        if (wName == "BlueCardLock")
                            W.Cast();
                    }
                    else if (FindCard == 3)
                    {
                        if (wName == "RedCardLock")
                            W.Cast();
                    }
                }
            }
        }

        private void LogicW()
        {
            var wName = W.Instance.Name;
            var t = TargetSelector.GetTarget(1100, TargetSelector.DamageType.Magical);
            if (wName == "PickACard" && Utils.TickCount - W.LastCastAttemptT > 150)
            {
                if (R.LSIsReady() && (Player.HasBuff("destiny_marker") || Player.HasBuff("gate")))
                    W.Cast();
                else if (t.LSIsValidTarget() && Program.Combo)
                    W.Cast();
                else if ( Orbwalker.GetTarget() != null)
                {
                    if (Program.Farm && Orbwalker.GetTarget().Type == GameObjectType.AIHeroClient && Config.Item("harasW", true).GetValue<bool>())
                        W.Cast();
                    else if (Program.LaneClear && (Orbwalker.GetTarget().Type == GameObjectType.obj_AI_Minion || Orbwalker.GetTarget().Type == GameObjectType.obj_AI_Turret) && Config.Item("farmW", true).GetValue<bool>())
                        W.Cast();
                }
            }
            else if(Player.HasBuff("pickacard_tracker"))
            {
                if (temp == null)
                    temp = wName;
                else if (temp != wName)
                    cardok = true;

                if (cardok)
                {
                    AIHeroClient orbTarget = null;
                    
                    var getTarget = Orbwalker.GetTarget();
                    if (getTarget != null && getTarget.Type == GameObjectType.AIHeroClient)
                    {
                        orbTarget = (AIHeroClient)getTarget;
                    }

                    if (R.LSIsReady() && (Player.HasBuff("destiny_marker") || Player.HasBuff("gate")))
                    {
                        FindCard = 1;
                        if (wName == "GoldCardLock")
                            W.Cast();
                    }
                    else if (Program.Combo && orbTarget.LSIsValidTarget() &&  W.GetDamage(orbTarget) + Player.LSGetAutoAttackDamage(orbTarget) > orbTarget.Health)
                    {
                        W.Cast();
                        Program.debug("1" + wName);
                    }
                    else if ( Player.Mana < RMANA + QMANA + WMANA)
                    {
                        FindCard = 2;
                        if (wName == "BlueCardLock")
                            W.Cast();
                    }
                    else if (Program.Farm && orbTarget.LSIsValidTarget())
                    {
                        FindCard = 1;
                        if (wName == "GoldCardLock")
                            W.Cast();
                    }
                    else if (Player.ManaPercent > Config.Item("WredFarm", true).GetValue<Slider>().Value && Program.LaneClear && Config.Item("farmW", true).GetValue<bool>())
                    {
                        FindCard = 3;
                        if (wName == "RedCardLock")
                            W.Cast();
                    }
                    else if ((Program.LaneClear || Player.Mana < RMANA + QMANA) && Config.Item("farmW", true).GetValue<bool>())
                    {
                        FindCard = 2;
                        if (wName == "BlueCardLock")
                            W.Cast();
                    }
                    else if(Program.Combo)
                    {
                        FindCard = 1;
                        if (wName == "GoldCardLock")
                            W.Cast();
                    }
                }
            }
        }

        private void Jungle()
        {
            if (Program.LaneClear)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 700, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];

                    if (Q.LSIsReady() && Config.Item("jungleQ", true).GetValue<bool>())
                    {
                        Q.Cast(mob);
                        return;
                    }
                }
            }
        }

        private void LogicR()
        {
            if (Player.LSCountEnemiesInRange(Config.Item("Renemy", true).GetValue<Slider>().Value) == 0)
            {
                var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                if (t.LSIsValidTarget() && t.LSDistance(Player.Position) > Q.Range && t.LSCountAlliesInRange(Config.Item("RenemyA", true).GetValue<Slider>().Value) == 0)
                {
                    if (Q.GetDamage(t) + W.GetDamage(t) + Player.LSGetAutoAttackDamage(t) * 3 > t.Health && t.LSCountEnemiesInRange(1000) < 3)
                    {
                        var rPos = R.GetPrediction(t).CastPosition;
                        if (Config.Item("turetR", true).GetValue<bool>())
                        {
                            if (!rPos.LSUnderTurret(true))
                                R.Cast(rPos);
                        }
                        else
                        {
                            R.Cast(rPos);
                        }
                    }
                }
            }
        }

        private void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (t.LSIsValidTarget())
            {
                    if (OktwCommon.GetKsDamage(t, Q)> t.Health && !SebbyLib.Orbwalking.InAutoAttackRange(t))
                        Program.CastSpell(Q, t);

                    if (W.Instance.CooldownExpires - Game.Time < W.Instance.Cooldown - 1.3 && W.Instance.Name == "PickACard" && (W.Instance.CooldownExpires - Game.Time > 3 || Player.LSCountEnemiesInRange(950) == 0))
                    {
                        if (Program.Combo && Player.Mana > RMANA + QMANA)
                            Program.CastSpell(Q, t);
                        if (Program.Farm && Player.Mana > RMANA + QMANA + WMANA + EMANA && Config.Item("harrasQ", true).GetValue<bool>() && OktwCommon.CanHarras())
                            Program.CastSpell(Q, t);
                    }

                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.LSIsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy, true, true);
                
            }
            else if (Program.LaneClear && Player.ManaPercent > Config.Item("Mana", true).GetValue<Slider>().Value && Config.Item("farmQ", true).GetValue<bool>() && Player.Mana > RMANA + QMANA)
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var farmPosition = Q.GetLineFarmLocation(minionList, Q.Width);
                if (farmPosition.MinionsHit > Config.Item("LCminions", true).GetValue<Slider>().Value)
                    Q.Cast(farmPosition.Position);
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
                RMANA = WMANA - Player.PARRegenRate * W.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (R.LSIsReady() && Config.Item("rRangeMini", true).GetValue<bool>())
            {
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Aqua, 1, 20, true);
            }
        }

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] + weight, color, msg);
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Config.Item("qRange", true).GetValue<bool>())
            {
                if (Config.Item("onlyRdy", true).GetValue<bool>())
                {
                    if (Q.LSIsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }

            if(Config.Item("cardInfo", true).GetValue<bool>() && W.Instance.Name != "PickACard")
            {
                if(FindCard == 1)
                    drawText("SEEK YELLOW" , Player.Position, System.Drawing.Color.Yellow, -70);
                if (FindCard == 2)
                    drawText("SEEK BLUE ", Player.Position, System.Drawing.Color.Aqua, -70);
                if (FindCard == 3)
                    drawText("SEEK RED ", Player.Position, System.Drawing.Color.OrangeRed, -70);

            }


            if (R.LSIsReady() && Config.Item("notR", true).GetValue<bool>() )
            {
                var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                if (t.LSIsValidTarget() )
                {
                    var comboDMG = Q.GetDamage(t) + W.GetDamage(t) + Player.LSGetAutoAttackDamage(t) * 3;
                    if (Player.HasBuff("destiny_marker"))
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Yellow, "AUTO R TARGET: " + t.ChampionName + " Heal " + t.Health + " My damage: " + comboDMG);
                    else if (comboDMG > t.Health)
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "You can kill: " + t.ChampionName + " Heal " + t.Health + " My damage: " + comboDMG + " PRESS semi-manual cast");
                }
            }
        }
    }
}
