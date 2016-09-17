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
    class BadaoMissFortuneAuto
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                return;
            if (Utils.GameTimeTickCount - BadaoMissFortuneVariables.Rcount <= 500)
                return;
            if (ObjectManager.Player.IsChannelingImportantSpell())
            {
                return;
            }
            if (!BadaoMissFortuneHelper.CanAutoMana())
                return;
            foreach (AIHeroClient heroX in HeroManager.Enemies.Where(x => x.BadaoIsValidTarget() &&
                                                                   x.Position.To2D().Distance(ObjectManager.Player.Position.To2D())
                                                                   <= BadaoMainVariables.Q.Range + 500))
            {
                if (heroX.BadaoIsValidTarget() && BadaoMissFortuneHelper.UseQ2Auto(heroX))
                {
                    if (Orbwalking.CanMove(80))
                    {
                        List<Obj_AI_Base> a = new List<Obj_AI_Base>();
                        if (BadaoMissFortuneVariables.TapTarget.BadaoIsValidTarget() &&
                               heroX.NetworkId == BadaoMissFortuneVariables.TapTarget.NetworkId)
                        {
                            foreach (AIHeroClient hero in HeroManager.Enemies.Where(x => x.NetworkId != heroX.NetworkId &&
                                                                                    x.BadaoIsValidTarget(BadaoMainVariables.Q.Range)))
                            {
                                var Qpred = BadaoMainVariables.Q.GetPrediction(hero);
                                var PredHero = Prediction.GetPrediction(hero, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                1400 + Game.Ping / 1000));
                                var PredheroX = Prediction.GetPrediction(heroX, 0.25f +
                                                        ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                                Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredHero.UnitPosition.To2D(),
                                    ObjectManager.Player.Position.To2D().Distance(PredHero.UnitPosition.To2D()) + 500);
                                if (BadaoChecker.BadaoInTheCone(PredheroX.UnitPosition.To2D(), PredHero.UnitPosition.To2D(), endpos, 40)
                                    && BadaoChecker.BadaoInTheCone(heroX.Position.To2D(), PredHero.UnitPosition.To2D(), endpos - 100, 40 - 5))
                                {
                                    a.Add(hero);
                                }
                            }
                            foreach (Obj_AI_Minion minion in MinionManager.GetMinions(BadaoMainVariables.Q.Range))
                            {
                                var Qpred = BadaoMainVariables.Q.GetPrediction(minion);
                                var PredMinion = Prediction.GetPrediction(minion, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                1400 + Game.Ping / 1000));
                                var PredheroX = Prediction.GetPrediction(heroX, 0.25f +
                                                        ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                                Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredMinion.UnitPosition.To2D(),
                                    ObjectManager.Player.Position.To2D().Distance(PredMinion.UnitPosition.To2D()) + 500);
                                if (BadaoChecker.BadaoInTheCone(PredheroX.UnitPosition.To2D(), PredMinion.UnitPosition.To2D(), endpos, 40)
                                    && BadaoChecker.BadaoInTheCone(heroX.Position.To2D(), PredMinion.UnitPosition.To2D(), endpos - 100, 40 - 5))
                                {
                                    a.Add(minion);
                                }
                            }
                            var targetQ = a.OrderBy(x => (float)(180f - BadaoChecker.BadaoAngleBetween(heroX.Position.To2D(), Prediction.GetPrediction(x, 0.25f + ObjectManager.Player.Position.To2D().Distance(BadaoMainVariables.Q.GetPrediction(x).UnitPosition.To2D() /
                                                                1400 + Game.Ping / 1000)).UnitPosition.To2D(), ObjectManager.Player.Position.To2D())) / 40f + Math.Abs(x.Position.To2D().Distance(heroX.Position.To2D()) - 300) / 100f).FirstOrDefault();
                            if (targetQ.BadaoIsValidTarget())
                            {
                                BadaoMainVariables.Q.Cast(targetQ);
                            }
                        }
                        else if (!BadaoMissFortuneVariables.TapTarget.IsValidTarget() ||
                            heroX.NetworkId != BadaoMissFortuneVariables.TapTarget.NetworkId)
                        {
                            //40
                            foreach (AIHeroClient hero in HeroManager.Enemies.Where(x => x.NetworkId != heroX.NetworkId &&
                                                                                    x.BadaoIsValidTarget(BadaoMainVariables.Q.Range)))
                            {
                                var Qpred = BadaoMainVariables.Q.GetPrediction(hero);
                                var PredHero = Prediction.GetPrediction(hero, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                1400 + Game.Ping / 1000));
                                var PredheroX = Prediction.GetPrediction(heroX, 0.25f +
                                                        ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                                Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredHero.UnitPosition.To2D(),
                                    ObjectManager.Player.Position.To2D().Distance(PredHero.UnitPosition.To2D()) + 500);
                                if (BadaoChecker.BadaoInTheCone(PredheroX.UnitPosition.To2D(), PredHero.UnitPosition.To2D(), endpos, 40) &&
                                    BadaoChecker.BadaoInTheCone(heroX.Position.To2D(), PredHero.UnitPosition.To2D(), endpos - 100, 40 - 5) &&
                                    !MinionManager.GetMinions(BadaoMainVariables.Q.Range + 600).Any(x =>
                                    BadaoChecker.BadaoInTheCone(Prediction.GetPrediction(x, 0.25f +
                                                                                        ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                                        1400 + Game.Ping / 1000)).UnitPosition.To2D(),
                                                                                        PredHero.UnitPosition.To2D(), endpos, 40 + 5)))
                                {
                                    BadaoMainVariables.Q.Cast(hero);
                                    goto abc;
                                }
                            }
                            foreach (Obj_AI_Minion minion in MinionManager.GetMinions(BadaoMainVariables.Q.Range))
                            {
                                var Qpred = BadaoMainVariables.Q.GetPrediction(minion);
                                var PredMinion = Prediction.GetPrediction(minion, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                1400 + Game.Ping / 1000));
                                var PredheroX = Prediction.GetPrediction(heroX, 0.25f +
                                                        ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                                Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredMinion.UnitPosition.To2D(),
                                    ObjectManager.Player.Position.To2D().Distance(PredMinion.UnitPosition.To2D()) + 500);
                                if (BadaoChecker.BadaoInTheCone(PredheroX.UnitPosition.To2D(), PredMinion.UnitPosition.To2D(), endpos, 40) &&
                                    BadaoChecker.BadaoInTheCone(heroX.Position.To2D(), PredMinion.UnitPosition.To2D(), endpos - 100, 40 - 5) &&
                                    !MinionManager.GetMinions(BadaoMainVariables.Q.Range + 500).Any(x =>
                                    x.NetworkId != minion.NetworkId &&
                                    BadaoChecker.BadaoInTheCone(Prediction.GetPrediction(x, 0.25f +
                                                                                        ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                                        1400 + Game.Ping / 1000)).UnitPosition.To2D(),
                                                                                        PredMinion.UnitPosition.To2D(), endpos, 40 + 5)))
                                {
                                    BadaoMainVariables.Q.Cast(minion);
                                    goto abc;
                                }
                            }
                            //20
                            foreach (AIHeroClient hero in HeroManager.Enemies.Where(x => x.NetworkId != heroX.NetworkId &&
                                                                                    x.BadaoIsValidTarget(BadaoMainVariables.Q.Range)))
                            {
                                var Qpred = BadaoMainVariables.Q.GetPrediction(hero);
                                var PredHero = Prediction.GetPrediction(hero, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                1400 + Game.Ping / 1000));
                                var PredheroX = Prediction.GetPrediction(heroX, 0.25f +
                                                        ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                                Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredHero.UnitPosition.To2D(),
                                    ObjectManager.Player.Position.To2D().Distance(PredHero.UnitPosition.To2D()) + 500);
                                if (BadaoChecker.BadaoInTheCone(PredheroX.UnitPosition.To2D(), PredHero.UnitPosition.To2D(), endpos, 20) &&
                                    BadaoChecker.BadaoInTheCone(heroX.Position.To2D(), PredHero.UnitPosition.To2D(), endpos - 100, 20 - 3) &&
                                    !MinionManager.GetMinions(BadaoMainVariables.Q.Range + 600).Any(x =>
                                    BadaoChecker.BadaoInTheCone(Prediction.GetPrediction(x, 0.25f +
                                                                                        ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                                        1400 + Game.Ping / 1000)).UnitPosition.To2D(),
                                                                                        PredHero.UnitPosition.To2D(), endpos, 20 + 3)))
                                {
                                    BadaoMainVariables.Q.Cast(hero);
                                    goto abc;
                                }
                            }
                            foreach (Obj_AI_Minion minion in MinionManager.GetMinions(BadaoMainVariables.Q.Range))
                            {
                                var Qpred = BadaoMainVariables.Q.GetPrediction(minion);
                                var PredMinion = Prediction.GetPrediction(minion, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                1400 + Game.Ping / 1000));
                                var PredheroX = Prediction.GetPrediction(heroX, 0.25f +
                                                        ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                                Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredMinion.UnitPosition.To2D(),
                                    ObjectManager.Player.Position.To2D().Distance(PredMinion.UnitPosition.To2D()) + 500);
                                if (BadaoChecker.BadaoInTheCone(PredheroX.UnitPosition.To2D(), PredMinion.UnitPosition.To2D(), endpos, 20) &&
                                    BadaoChecker.BadaoInTheCone(heroX.Position.To2D(), PredMinion.UnitPosition.To2D(), endpos - 100, 20 - 3) &&
                                    !MinionManager.GetMinions(BadaoMainVariables.Q.Range + 500).Any(x =>
                                    x.NetworkId != minion.NetworkId &&
                                    BadaoChecker.BadaoInTheCone(Prediction.GetPrediction(x, 0.25f +
                                                                                        ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                                        1400 + Game.Ping / 1000)).UnitPosition.To2D(),
                                                                                        PredMinion.UnitPosition.To2D(), endpos, 20 + 3)))
                                {
                                    BadaoMainVariables.Q.Cast(minion);
                                }
                            }
                        abc:;
                        }
                    }
                }
            }
        }
    }
}