using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
using LeagueSharp.Common; 
namespace LeeSin_EloClimber
{
    class rUtility
    {
        public Vector3 pos;
        public int hit;

        // Default constructor:
        public rUtility()
        {
        }

        // Constructor:
        public rUtility(Vector3 pos, int hit)
        {
            this.pos = pos;
            this.hit = hit;
        }
    }

    internal class Combo
    {      
        private static AIHeroClient Target;
        private static float jumpToPos = Environment.TickCount;

        internal static void Load()
        {
            Game.OnUpdate += Update;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            if (Target != null && Target.IsValidTarget(LeeSin.Q.Range + LeeSin.W.Range) && MenuManager.myMenu.Item("combo.target").GetValue<Boolean>())
            {
                Render.Circle.DrawCircle(Target.Position, 100, Color.Aquamarine);
            }
        }

        private static void Update(EventArgs args)
        {
            Target = TargetSelector.GetTarget(LeeSin.Q.Range + 300, TargetSelector.DamageType.Physical);

            if (LeeSin.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                launchCombo(Target);
            }
        }

        private static void launchCombo(AIHeroClient unit)
        {
            if (unit != null & unit.IsValidTarget(LeeSin.Q.Range + LeeSin.W.Range) && unit.IsEnemy && unit.IsChampion())
            {
                if (MenuManager.myMenu.Item("combo.useQ").GetValue<Boolean>())
                {
                    CastQ(unit);
                }
                if (MenuManager.myMenu.Item("combo.useW").GetValue<Boolean>() && LeeSin.myHero.Mana > 100)
                {
                    CastW(unit);
                }
                if (MenuManager.myMenu.Item("combo.useE").GetValue<Boolean>() && LeeSin.myHero.Mana > 100)
                {
                    CastE(unit);
                }
                if (MenuManager.myMenu.Item("combo.useR").GetValue<Boolean>())
                {
                    CastR(unit);
                }
            }
        }

        private static void CastW(AIHeroClient target)
        {
            if(target.CountEnemiesInRange(2000) == 1 || !LeeSin.R.IsReady() || !MenuManager.myMenu.Item("combo.rLogic").GetValue<Boolean>())
            {
                if (Environment.TickCount - LeeSin.qCast < 1000)
                    return;

                var allyMinion = MinionManager.GetMinions(LeeSin.W.Range, MinionTypes.All, MinionTeam.Ally, MinionOrderTypes.None);

                if (target.Position.Distance(LeeSin.myHero.Position) > 600)
                {
                    var ally = HeroManager.Allies.Where(unit => (unit.Position.Distance(target.Position) < LeeSin.Q.Range && (unit.NetworkId != LeeSin.myHero.NetworkId)));
                    if (ally.Count() > 0)
                    {
                        LeeSin.W.Cast(ally.First());
                        return;
                    }

                    var minion = allyMinion.Where(unit => (unit.Distance(target.Position) < LeeSin.Q.Range));
                    if (minion.Count() > 0)
                    {
                        LeeSin.W.Cast(minion.First());
                        return;
                    }

                    if (MenuManager.myMenu.Item("combo.ward").GetValue<Boolean>() && LeeSin.myHero.Position.Distance(target.Position) > LeeSin.Q.Range )
                    {
                        Vector3 wardPos = LeeSin.myHero.Position + (target.Position - LeeSin.myHero.Position).Normalized() * 600;
                        LeeSin.WardJump_Position(wardPos);
                        return;
                    }
                }             
                else if (target.Position.Distance(LeeSin.myHero.Position) < 400)
                {
                    LeeSin.W.Cast(LeeSin.myHero);
                    return;
                }
            }
        }

        private static void CastQ(AIHeroClient unit)
        {
            if (LeeSin.Q.IsReady() && !LeeSin.IsSecondCast(LeeSin.Q))
            {
                if (MenuManager.myMenu.Item("pred.list2").GetValue<StringList>().SelectedIndex == 0)
                {
                    PredictionOutput qPred = LeeSin.Q.GetPrediction(unit);
                    if ((int)qPred.Hitchance >= MenuManager.myMenu.Item("combo.qHitChance").GetValue<Slider>().Value)
                        LeeSin.Q.Cast(qPred.CastPosition);
                }
                else if(MenuManager.myMenu.Item("pred.list2").GetValue<StringList>().SelectedIndex == 1)
                {
                    resultPred qPred = myPred.GetPrediction(unit, LeeSin.Q);
                    if (qPred.Hitchance >= MenuManager.myMenu.Item("combo.qHitChance").GetValue<Slider>().Value)
                        LeeSin.Q.Cast(qPred.predPos);
                }              
            }
            else if (LeeSin.Q.IsReady() && LeeSin.IsSecondCast(LeeSin.Q) && unit.HasBuff("BlindMonkQOne"))
            {
                if (LeeSin.myHero.Position.Distance(unit.Position) > 700 || Environment.TickCount - LeeSin.lastQ > 2800 || LeeSin.GetDamage_Q2(unit, 0) > unit.Health)
                LeeSin.Q.Cast();
            }
        }

        private static void CastE(AIHeroClient target)
        {
            if (LeeSin.E.IsReady() && !LeeSin.IsSecondCast(LeeSin.E) && LeeSin.myHero.Position.Distance(target.Position) < LeeSin.E.Range - 20)
                LeeSin.E.Cast();
            else if (LeeSin.E.IsReady() && LeeSin.IsSecondCast(LeeSin.E) && (LeeSin.myHero.Position.Distance(target.Position) > LeeSin.E.Range || LeeSin.PassiveStack == 0))
                LeeSin.E.Cast();
        }

        private static void CastR(AIHeroClient target)
        {
            if (LeeSin.R.IsReady())
            {
                if (MenuManager.myMenu.Item("combo.rLogic").GetValue<Boolean>())
                {
                    // Find best pos to kick
                    Spell wardSpell = LeeSin.FindWard();
                    rUtility value = Find_R_BestPos(target);
                    if (value.hit > 1 && value.pos.Distance(LeeSin.myHero.Position) < 600)
                    {
                        if (target.Position.Distance(LeeSin.myHero.Position) < LeeSin.R.Range && Environment.TickCount - jumpToPos < 2000)
                        {
                            LeeSin.R.Cast(target);
                        }
                        else if (LeeSin.W.IsReady() && !LeeSin.IsSecondCast(LeeSin.W) && wardSpell != null)
                        {
                            jumpToPos = Environment.TickCount;
                            LeeSin.WardJump_Position(value.pos);
                        }
                    }
                }
                // Kill
                if (LeeSin.myHero.Position.Distance(target.Position) < LeeSin.R.Range)
                {
                    var r_dmg = LeeSin.GetDamage_R(target);
                    if (target.Health < r_dmg)
                        LeeSin.R.Cast(target);

                    if (LeeSin.Q.IsReady() && !LeeSin.IsSecondCast(LeeSin.Q))
                    {
                        if (LeeSin.GetDamage_Q(target, r_dmg) + r_dmg > target.Health)
                            LeeSin.R.Cast(target);
                    }

                    if (LeeSin.Q.IsReady() && LeeSin.IsSecondCast(LeeSin.Q) && target.HasBuff("BlindMonkQOne"))
                    {
                        if (LeeSin.GetDamage_Q2(target, r_dmg) + r_dmg > target.Health)
                            LeeSin.R.Cast(target);
                    }
                }   

            }         
        }

        private static rUtility Find_R_BestPos(Obj_AI_Base target)
        {
            rUtility result = new rUtility(new Vector3(), 0);
            Vector3 bestPos = new Vector3();
            int unitHit = 1;
            int maxHit = 1;

            foreach (var unit in HeroManager.Enemies)
            {
                if (unit.NetworkId != target.NetworkId && unit.IsValidTarget())
                {                    
                    var pred = LeagueSharp.Common.Prediction.GetPrediction(unit, 300);
                    if (target.Position.Distance(pred.UnitPosition) < 900)
                    {
                        Vector3 startPos = target.Position;
                        Vector3 endPos = pred.UnitPosition;
                        endPos = startPos + (endPos - startPos).Normalized() * 900;
                        var zone = new LeagueSharp.Common.Geometry.Polygon.Rectangle(startPos, endPos, target.BoundingRadius - 5);

                        foreach (var unit2 in HeroManager.Enemies)
                        {
                            if (unit2.NetworkId != target.NetworkId && unit2.NetworkId != unit.NetworkId && unit2.IsValidTarget())
                            {
                                pred = LeagueSharp.Common.Prediction.GetPrediction(unit2, 300);
                                if (zone.IsInside(pred.UnitPosition))
                                {
                                    unitHit++;
                                    if (unitHit > maxHit)
                                    {
                                        maxHit = unitHit;
                                        bestPos = target.Position + (target.Position - unit2.Position).Normalized() * 250;
                                        result.pos = bestPos;
                                        result.hit = maxHit;
                                    }
                                }
                            }
                        }
                        if (maxHit == 1)
                        {
                            maxHit = 2;
                            bestPos = target.Position + (target.Position - unit.Position).Normalized() * 250;
                            result.pos = bestPos;
                            result.hit = maxHit;
                        }
                    }
                }
            }

            return result;
        }
    }
}
