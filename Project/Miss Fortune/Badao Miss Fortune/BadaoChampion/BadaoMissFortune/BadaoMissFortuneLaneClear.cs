using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using BadaoMissFortune;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoMissFortune
{
    class BadaoMissFortuneLaneClear
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.OnAttack += Orbwalking_OnAttack;
        }

        private static void Orbwalking_OnAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe || BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !(target is Obj_AI_Minion) || target.Team == GameObjectTeam.Neutral)
                return;
            if (Utils.GameTimeTickCount - BadaoMissFortuneVariables.Rcount <= 500)
                return;
            if (ObjectManager.Player.IsChannelingImportantSpell())
            {
                return;
            }
            if (!BadaoMissFortuneHelper.CanLaneClearMana())
                return;
            if (BadaoMissFortuneHelper.UseWLaneClear() && target.BadaoIsValidTarget())
            {
                BadaoMainVariables.W.Cast();
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;
            if (Utils.GameTimeTickCount - BadaoMissFortuneVariables.Rcount <= 500)
                return;
            if (ObjectManager.Player.IsChannelingImportantSpell())
            {
                return;
            }
            if (!BadaoMissFortuneHelper.CanLaneClearMana())
                return;
            // Q logic
            if (BadaoMissFortuneHelper.UseQLaneClear() && Orbwalking.CanMove(80))
            {
                foreach (Obj_AI_Minion minion in MinionManager.GetMinions(BadaoMainVariables.Q.Range).OrderBy(x => x.Health))
                {
                    if (minion.BadaoIsValidTarget())
                    {
                        var Qpred = BadaoMainVariables.Q.GetPrediction(minion);
                        var PredMinion = Prediction.GetPrediction(minion, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()
                                                        / 1400 + Game.Ping / 1000));
                        Vector2 endpos1 = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredMinion.UnitPosition.To2D(),
                            ObjectManager.Player.Position.To2D().Distance(PredMinion.UnitPosition.To2D()) + 500);
                        Vector2 endpos2 = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredMinion.UnitPosition.To2D(),
                            ObjectManager.Player.Position.To2D().Distance(PredMinion.UnitPosition.To2D()) + 150);
                        if (MinionManager.GetMinions(BadaoMainVariables.Q.Range + 500).Any(x =>  x.BadaoIsValidTarget() &&
                                                        x.NetworkId != minion.NetworkId &&
                                                        BadaoChecker.BadaoInTheCone(Prediction.GetPrediction(x,
                                                        0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                        1400 + Game.Ping / 1000)).UnitPosition.To2D(), PredMinion.UnitPosition.To2D(), endpos1, 110))
                            || MinionManager.GetMinions(BadaoMainVariables.Q.Range + 150).Any(x => x.NetworkId != minion.NetworkId &&
                                                        BadaoChecker.BadaoInTheCone(Prediction.GetPrediction(x,
                                                        0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                        1400 + Game.Ping / 1000)).UnitPosition.To2D(), PredMinion.UnitPosition.To2D(), endpos2, 160)))
                        {
                            if (BadaoMainVariables.Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                                goto xyz;
                        }
                    }
                }
                var minionQ1 = MinionManager.GetMinions(BadaoMainVariables.Q.Range).FirstOrDefault();
                {
                    if (minionQ1.BadaoIsValidTarget() && BadaoMainVariables.Q.Cast(minionQ1) == Spell.CastStates.SuccessfullyCasted)
                        goto xyz;
                }
            xyz:;
            }
            // E logic
            if (BadaoMissFortuneHelper.UseELaneClear() && Orbwalking.CanMove(80))
            {
                var minion = MinionManager.GetMinions(BadaoMainVariables.E.Range).OrderByDescending
                    (x => MinionManager.GetMinions(x.Position, 250).Count()).FirstOrDefault();
                if (minion.BadaoIsValidTarget())
                {
                    BadaoMainVariables.E.Cast(minion);
                }
            }
        }
    }
}
