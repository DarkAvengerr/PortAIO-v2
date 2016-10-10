using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HeavenStrikeRyze
{
    public static class Lane
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Program._orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;

            if (Player.Mana * 100 / Player.MaxMana > Program.ManaLaneClear)
            {
                var tarqs = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.IsValidTarget() && x.IsMinion && x.Distance(Player.Position) <= Program._q.Range);
                var tars = tarqs.Where(x => x.Distance(Player.Position) <= Program._e.Range);
                if (tarqs.Count() <= 2 || Player.Mana <= Program._e.ManaCost * 2 + Program._q.ManaCost)
                {
                    //Chat.Print("case 1");
                    var targetq = MinionManager.GetMinions(Player.Position, Program._q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health).FirstOrDefault();
                    var target = MinionManager.GetMinions(Player.Position, 600, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health).FirstOrDefault();
                    if (Program._q2.IsReady() && Program.QlaneClear)
                    {
                        if (targetq != null)
                        {
                            Program._q2.Cast(targetq);
                        }
                    }
                    else if (Program._e.IsReady() && Program.ElaneClear)
                    {
                        if (target != null)
                        {
                            Program._e.Cast(target);
                        }
                    }
                    else if (Program._w.IsReady() && Program.WlaneClear)
                    {
                        if (target != null)
                        {
                            Program._w.Cast(targetq);
                        }
                    }
                }
                else
                {
                    //Chat.Print("case 2");
                    Obj_AI_Minion MainTarget = null;
                    int AoEcount = 0;

                    // in investigation
                    var tarqss = tarqs;
                    var tarqsss = tarqs;
                    MainTarget = tarqss.Where(x => Helper.HasEBuff(x) && Program._q.GetPrediction(x).Hitchance >= HitChance.Low && Program._q.IsReady()
                        && Program.QlaneClear && Helper.GetchainedTarget(x).Count() >= 3).MaxOrDefault(x => Helper.GetchainedTarget(x).Count());
                    if (MainTarget != null)
                    {
                        Program._q.Cast(Program._q.GetPrediction(MainTarget).UnitPosition);
                    }

                    MainTarget = tars.Where(x => Helper.HasEBuff(x)  && Program._e.IsReady() && Program.ElaneClear
                        && tarqs.Where(y => y.Distance(x.Position) <= 300).Count() >= 3).MaxOrDefault(x => tarqs.Where(y => y.Distance(x.Position) <= 300).Count());
                    if (MainTarget != null)
                    {
                        Program._e.Cast(MainTarget);
                    }

                    MainTarget = tars.Where(x => x.Health <= Helper.Edamge(x) && Program._e.IsReady() && Program.ElaneClear
                        && tarqs.Where(y => y.Distance(x.Position) <= 300).Count() >= 3).MaxOrDefault(x => tarqs.Where(y => y.Distance(x.Position) <= 300).Count());
                    if (MainTarget != null)
                    {
                        if (Program._e.IsReady() && Program.ElaneClear)
                        {
                            Program._e.Cast(MainTarget);
                        }
                    }

                    MainTarget = tars.Where(x => x.Health <= Helper.Wdamge(x) && Program._w.IsReady() && Program.WlaneClear
                        && Helper.HasEBuff(x) && tarqs.Where(y => y.Distance(x.Position) <= 300).Count() >= 3).MaxOrDefault(x => tarqs.Where(y => y.Distance(x.Position) <= 300).Count());
                    if (MainTarget != null)
                    {
                        //Chat.Print("2");
                        if (Program._w.IsReady() && Program.WlaneClear)
                        {
                            Program._w.Cast(MainTarget);
                        }
                    }

                    MainTarget = tars.Where(x => x.Health <= Helper.Qdamage(x) && Program._q.IsReady() && Program.QlaneClear
                        && Helper.HasEBuff(x) && Program._q.GetPrediction(x).Hitchance >= HitChance.Low
                        && tarqs.Where(y => y.Distance(x.Position) <= 300).Count() >= 3)
                        .MaxOrDefault(x => tarqs.Where(y => y.Distance(x.Position) <= 300).Count());
                    if (MainTarget != null)
                    {
                        if (Program._q.IsReady() && Program.QlaneClear)
                        {
                            Program._q.Cast(Program._q.GetPrediction(MainTarget).UnitPosition);
                        }
                    }

                    MainTarget = tars.Where(x => x.Health <= Helper.Edamge(x) + Helper.Wdamge(x) && Program._e.IsReady() && Program.ElaneClear && Program._w.IsReady() && Program.WlaneClear
                        && tarqs.Where(y => y.Distance(x.Position) <= 300).Count() >= 3)
                        .MaxOrDefault(x => tarqs.Where(y => y.Distance(x.Position) <= 300).Count());
                    if (MainTarget != null)
                    {
                        if (Program._e.IsReady() && Program.ElaneClear && Program._w.IsReady() && Program.WlaneClear)
                        {
                            Program._e.Cast(MainTarget);
                        }
                    }

                    MainTarget = tars.Where(x => x.Health <= Helper.Edamge(x) + Helper.Qdamage(x,true) && Program._e.IsReady() && Program.ElaneClear && Program._q.IsReady() && Program.QlaneClear
                        && Program._q.GetPrediction(x).Hitchance >= HitChance.Low
                        && tarqs.Where(y => y.Distance(x.Position) <= 300).Count() >= 3)
                        .MaxOrDefault(x => tarqs.Where(y => y.Distance(x.Position) <= 300).Count());
                    if (MainTarget != null)
                    {
                        if (Program._e.IsReady() && Program.ElaneClear && Program._q.IsReady() && Program.QlaneClear)
                        {
                            Program._e.Cast(MainTarget);
                        }
                    }
                    
                    foreach (var tar in tars)
                    {
                        int count = tarqs.Where(x => x.Distance(tar.Position) <= 300).Count();
                        if (count > AoEcount)
                        {
                            AoEcount = count;
                            MainTarget = tar;
                        }
                    }
                    if (AoEcount <3)
                    {
                        //Chat.Print("case 3");
                        var targetq = MinionManager.GetMinions(Player.Position, Program._q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health).FirstOrDefault();
                        var target = MinionManager.GetMinions(Player.Position, 600, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health).FirstOrDefault();
                        if (Program._q2.IsReady() && Program.QlaneClear)
                        {
                            if (targetq != null)
                            {
                                Program._q2.Cast(targetq);
                            }
                        }
                        else if (Program._e.IsReady() && Program.ElaneClear)
                        {
                            if (target != null)
                            {
                                Program._e.Cast(target);
                            }
                        }
                        else if (Program._w.IsReady() && Program.WlaneClear)
                        {
                            if (target != null)
                            {
                                Program._w.Cast(targetq);
                            }
                        }
                    }
                    if (MainTarget != null)
                    {
                        Program._e.Cast(MainTarget);
                    }
                }
            }
        }
    }
}
