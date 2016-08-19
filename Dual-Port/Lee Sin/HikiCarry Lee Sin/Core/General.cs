using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HikiCarry_Lee_Sin.Plugins;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Lee_Sin.Core
{
    class General
    {
        public static void GeneralCombo()
        {
            if (Program.Config.Item("passive.usage").GetValue<bool>())
            {
                if (Spells.Q.IsReady() && Program.Config.Item("q1.combo").GetValue<bool>() && Spells.Q.QOne())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q.Range) && Spells.Q.GetPrediction(x).Hitchance >= HitChance.High
                        && Spells.Q.GetPrediction(x).CollisionObjects.Count <= 0))
                    {
                        Spells.Q.Cast(enemy);
                    }
                }
                if (Spells.Q2.IsReady() && Program.Config.Item("q2.combo").GetValue<bool>() && Spells.Q.QTwo())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q2.Range)))
                    {
                        Spells.Q2.Cast();
                    }
                }
                if (Spells.E.IsReady() && Program.Config.Item("e1.combo").GetValue<bool>() && Spells.E.EOne() && !ObjectManager.Player.HasBuff("blindmonkpassive_cosmetic"))
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.E.Range)))
                    {
                        Spells.E.Cast();
                    }
                }
                if (Spells.E2.IsReady() && Program.Config.Item("e2.combo").GetValue<bool>() && Spells.E2.ETwo() && !ObjectManager.Player.HasBuff("blindmonkpassive_cosmetic"))
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.E2.Range)))
                    {
                        Spells.E2.Cast();
                    }
                }
                if (Spells.W.IsReady() && Program.Config.Item("w1.combo").GetValue<bool>() && Spells.W.WOne() && !ObjectManager.Player.HasBuff("blindmonkpassive_cosmetic"))
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.E.Range)))
                    {
                        Spells.W.CastOnUnit(ObjectManager.Player);
                    }
                }
                if (Spells.W2.IsReady() && Program.Config.Item("w2.combo").GetValue<bool>() && Spells.W.WTwo() && !ObjectManager.Player.HasBuff("blindmonkpassive_cosmetic"))
                {
                    Spells.W2.Cast();
                }
                if (Spells.R.IsReady() && Program.Config.Item("r.combo").GetValue<bool>())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.R.Range) && Spells.R.GetDamage(x) > x.Health))
                    {
                        Spells.R.Cast(enemy);
                    }
                }
            }
            else
            {
                if (Spells.Q.IsReady() && Program.Config.Item("q1.combo").GetValue<bool>() && Spells.Q.QOne())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q.Range) && Spells.Q.GetPrediction(x).Hitchance >= HitChance.High
                        && Spells.Q.GetPrediction(x).CollisionObjects.Count <= 0))
                    {
                        Spells.Q.Cast(enemy);
                    }
                }
                if (Spells.Q2.IsReady() && Program.Config.Item("q2.combo").GetValue<bool>() && Spells.Q.QTwo())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q2.Range)))
                    {
                        Spells.Q2.Cast();
                    }
                }
                if (Spells.E.IsReady() && Program.Config.Item("e1.combo").GetValue<bool>() && Spells.E.EOne())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.E.Range)))
                    {
                        Spells.E.Cast();
                    }
                }
                if (Spells.E2.IsReady() && Program.Config.Item("e2.combo").GetValue<bool>() && Spells.E2.ETwo())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.E2.Range)))
                    {
                        Spells.E2.Cast();
                    }
                }
                if (Spells.W.IsReady() && Program.Config.Item("w1.combo").GetValue<bool>() && Spells.W.WOne())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.E.Range)))
                    {
                        Spells.W.CastOnUnit(ObjectManager.Player);
                    }
                }
                if (Spells.W2.IsReady() && Program.Config.Item("w2.combo").GetValue<bool>() && Spells.W.WTwo())
                {
                    Spells.W2.Cast();
                }
                if (Spells.R.IsReady() && Program.Config.Item("r.combo").GetValue<bool>())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.R.Range) && Spells.R.GetDamage(x) > x.Health))
                    {
                        Spells.R.Cast(enemy);
                    }
                }
            }
        }

        public static void MultipleUltimate(int min)// all credits goes to yol0
        {
            foreach (var enemy in from enemy in HeroManager.Enemies
                                  let input = new PredictionInput()
                                  {
                                      Aoe = false,
                                      Collision = true,
                                      CollisionObjects = new[] { CollisionableObjects.Heroes },
                                      Delay = 0.1f,
                                      Radius = 100f,
                                      Range = Spells.R.Range,
                                      Speed = 1500f,
                                      From = ObjectManager.Player.ServerPosition,
                                  }
                                  let output = Prediction.GetPrediction(input)
                                  where output.Hitchance >= HitChance.Medium && ObjectManager.Player.Distance(output.CastPosition) < Spells.R.Range
                                  let endPos = (ObjectManager.Player.ServerPosition + output.CastPosition - ObjectManager.Player.ServerPosition).Normalized() * 1000
                                  let colObjs = output.CollisionObjects
                                  where ObjectManager.Player.Distance(endPos) < 1200 && colObjs.Any()
                                  where colObjs.Count >= min
                                  select enemy)
            {
                Spells.R.CastOnUnit(enemy);
            }
        }
    }
}
