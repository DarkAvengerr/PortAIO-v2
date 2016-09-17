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
    class BadaoMissFortuneHarass
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed)
                return;
            if (Utils.GameTimeTickCount - BadaoMissFortuneVariables.Rcount <= 500)
                return;
            if (ObjectManager.Player.IsChannelingImportantSpell())
            {
                return;
            }
            if (!BadaoMissFortuneHelper.CanHarassMana())
                return;
            if (BadaoMissFortuneHelper.UseQ2Harass() && Orbwalking.CanMove(80))
            {
                // Q2 logic
                var targetQ = TargetSelector.GetTarget(BadaoMainVariables.Q.Range + 600, TargetSelector.DamageType.Physical);
                if (targetQ.BadaoIsValidTarget())
                {
                    if (BadaoMissFortuneVariables.TapTarget.BadaoIsValidTarget() &&
                        targetQ.NetworkId == BadaoMissFortuneVariables.TapTarget.NetworkId)
                    {
                        foreach (AIHeroClient hero in HeroManager.Enemies.Where(x => x.NetworkId != targetQ.NetworkId &&
                                    x.BadaoIsValidTarget(BadaoMainVariables.Q.Range)))
                        {
                            var Qpred = BadaoMainVariables.Q.GetPrediction(hero);
                            var PredHero = Prediction.GetPrediction(hero, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                            1400 + Game.Ping / 1000));
                            var PredTargetQ = Prediction.GetPrediction(targetQ, 0.25f +
                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                            Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredHero.UnitPosition.To2D(),
                                ObjectManager.Player.Position.To2D().Distance(PredHero.UnitPosition.To2D()) + 500);
                            if (BadaoMissFortuneHelper.Q1Damage(hero) >= hero.Health &&
                                BadaoChecker.BadaoInTheCone(PredTargetQ.UnitPosition.To2D(), PredHero.UnitPosition.To2D(), endpos, 40))
                            {
                                if (BadaoMainVariables.Q.Cast(hero) == Spell.CastStates.SuccessfullyCasted)
                                    goto abc;
                            }
                        }
                        foreach (Obj_AI_Minion minion in MinionManager.GetMinions(BadaoMainVariables.Q.Range))
                        {
                            var Qpred = BadaoMainVariables.Q.GetPrediction(minion);
                            var PredMinion = Prediction.GetPrediction(minion, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                            1400 + Game.Ping / 1000));
                            var PredTargetQ = Prediction.GetPrediction(targetQ, 0.25f +
                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                            Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredMinion.UnitPosition.To2D(),
                                ObjectManager.Player.Position.To2D().Distance(PredMinion.UnitPosition.To2D()) + 500);
                            if (BadaoMissFortuneHelper.Q1Damage(minion) >= minion.Health &&
                                BadaoChecker.BadaoInTheCone(PredTargetQ.UnitPosition.To2D(), PredMinion.UnitPosition.To2D(), endpos, 40))
                            {
                                if (BadaoMainVariables.Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                                    goto abc;
                            }
                        }
                        foreach (AIHeroClient hero in HeroManager.Enemies.Where(x => x.NetworkId != targetQ.NetworkId &&
                                                                                x.BadaoIsValidTarget(BadaoMainVariables.Q.Range)))
                        {
                            var Qpred = BadaoMainVariables.Q.GetPrediction(hero);
                            var PredHero = Prediction.GetPrediction(hero, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                            1400 + Game.Ping / 1000));
                            var PredTargetQ = Prediction.GetPrediction(targetQ, 0.25f +
                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                            Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredHero.UnitPosition.To2D(),
                                ObjectManager.Player.Position.To2D().Distance(PredHero.UnitPosition.To2D()) + 500);
                            if (BadaoChecker.BadaoInTheCone(PredTargetQ.UnitPosition.To2D(), PredHero.UnitPosition.To2D(), endpos, 40))
                            {
                                if (BadaoMainVariables.Q.Cast(hero) == Spell.CastStates.SuccessfullyCasted)
                                    goto abc;
                            }
                        }
                        foreach (Obj_AI_Minion minion in MinionManager.GetMinions(BadaoMainVariables.Q.Range))
                        {
                            var Qpred = BadaoMainVariables.Q.GetPrediction(minion);
                            var PredMinion = Prediction.GetPrediction(minion, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                            1400 + Game.Ping / 1000));
                            var PredTargetQ = Prediction.GetPrediction(targetQ, 0.25f +
                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                            Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredMinion.UnitPosition.To2D(),
                                ObjectManager.Player.Position.To2D().Distance(PredMinion.UnitPosition.To2D()) + 500);
                            if (BadaoChecker.BadaoInTheCone(PredTargetQ.UnitPosition.To2D(), PredMinion.UnitPosition.To2D(), endpos, 40))
                            {
                                if (BadaoMainVariables.Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                                    goto abc;
                            }
                        }
                    }
                    else if (!BadaoMissFortuneVariables.TapTarget.IsValidTarget() ||
                        targetQ.NetworkId != BadaoMissFortuneVariables.TapTarget.NetworkId)
                    {
                        //40
                        foreach (AIHeroClient hero in HeroManager.Enemies.Where(x => x.NetworkId != targetQ.NetworkId &&
                                                                                x.BadaoIsValidTarget(BadaoMainVariables.Q.Range)))
                        {
                            var Qpred = BadaoMainVariables.Q.GetPrediction(hero);
                            var PredHero = Prediction.GetPrediction(hero, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                            1400 + Game.Ping / 1000));
                            var PredTargetQ = Prediction.GetPrediction(targetQ, 0.25f +
                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                            Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredHero.UnitPosition.To2D(),
                                ObjectManager.Player.Position.To2D().Distance(PredHero.UnitPosition.To2D()) + 500);
                            if (BadaoMissFortuneHelper.Q1Damage(hero) >= hero.Health &&
                                BadaoChecker.BadaoInTheCone(PredTargetQ.UnitPosition.To2D(), PredHero.UnitPosition.To2D(), endpos, 40) &&
                                !MinionManager.GetMinions(BadaoMainVariables.Q.Range + 600).Any(x =>
                                BadaoChecker.BadaoInTheCone(Prediction.GetPrediction(x, 0.25f +
                                                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                                    1400 + Game.Ping / 1000)).UnitPosition.To2D(),
                                                                                    PredHero.UnitPosition.To2D(), endpos, 40)))
                            {
                                if (BadaoMainVariables.Q.Cast(hero) == Spell.CastStates.SuccessfullyCasted)
                                    goto abc;
                            }
                        }
                        foreach (Obj_AI_Minion minion in MinionManager.GetMinions(BadaoMainVariables.Q.Range))
                        {
                            var Qpred = BadaoMainVariables.Q.GetPrediction(minion);
                            var PredMinion = Prediction.GetPrediction(minion, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                            1400 + Game.Ping / 1000));
                            var PredTargetQ = Prediction.GetPrediction(targetQ, 0.25f +
                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                            Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredMinion.UnitPosition.To2D(),
                                ObjectManager.Player.Position.To2D().Distance(PredMinion.UnitPosition.To2D()) + 500);
                            if (BadaoMissFortuneHelper.Q1Damage(minion) >= minion.Health &&
                                BadaoChecker.BadaoInTheCone(PredTargetQ.UnitPosition.To2D(), PredMinion.UnitPosition.To2D(), endpos, 40) &&
                                !MinionManager.GetMinions(BadaoMainVariables.Q.Range + 500).Any(x =>
                                x.NetworkId != minion.NetworkId &&
                                BadaoChecker.BadaoInTheCone(Prediction.GetPrediction(x, 0.25f +
                                                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                                    1400 + Game.Ping / 1000)).UnitPosition.To2D(),
                                                                                    PredMinion.UnitPosition.To2D(), endpos, 40)))
                            {
                                if (BadaoMainVariables.Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                                    goto abc;
                            }
                        }
                        foreach (AIHeroClient hero in HeroManager.Enemies.Where(x => x.NetworkId != targetQ.NetworkId &&
                                                                                x.BadaoIsValidTarget(BadaoMainVariables.Q.Range)))
                        {
                            var Qpred = BadaoMainVariables.Q.GetPrediction(hero);
                            var PredHero = Prediction.GetPrediction(hero, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                            1400 + Game.Ping / 1000));
                            var PredTargetQ = Prediction.GetPrediction(targetQ, 0.25f +
                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                            Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredHero.UnitPosition.To2D(),
                                ObjectManager.Player.Position.To2D().Distance(PredHero.UnitPosition.To2D()) + 500);
                            if (BadaoChecker.BadaoInTheCone(PredTargetQ.UnitPosition.To2D(), PredHero.UnitPosition.To2D(), endpos, 40) &&
                                !MinionManager.GetMinions(BadaoMainVariables.Q.Range + 600).Any(x =>
                                BadaoChecker.BadaoInTheCone(Prediction.GetPrediction(x, 0.25f +
                                                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                                    1400 + Game.Ping / 1000)).UnitPosition.To2D(),
                                                                                    PredHero.UnitPosition.To2D(), endpos, 40)))
                            {
                                if (BadaoMainVariables.Q.Cast(hero) == Spell.CastStates.SuccessfullyCasted)
                                    goto abc;
                            }
                        }
                        foreach (Obj_AI_Minion minion in MinionManager.GetMinions(BadaoMainVariables.Q.Range))
                        {
                            var Qpred = BadaoMainVariables.Q.GetPrediction(minion);
                            var PredMinion = Prediction.GetPrediction(minion, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                            1400 + Game.Ping / 1000));
                            var PredTargetQ = Prediction.GetPrediction(targetQ, 0.25f +
                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                            Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredMinion.UnitPosition.To2D(),
                                ObjectManager.Player.Position.To2D().Distance(PredMinion.UnitPosition.To2D()) + 500);
                            if (BadaoChecker.BadaoInTheCone(PredTargetQ.UnitPosition.To2D(), PredMinion.UnitPosition.To2D(), endpos, 40) &&
                                !MinionManager.GetMinions(BadaoMainVariables.Q.Range + 500).Any(x =>
                                x.NetworkId != minion.NetworkId &&
                                BadaoChecker.BadaoInTheCone(Prediction.GetPrediction(x, 0.25f +
                                                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                                    1400 + Game.Ping / 1000)).UnitPosition.To2D(),
                                                                                    PredMinion.UnitPosition.To2D(), endpos, 40)))
                            {
                                if (BadaoMainVariables.Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                                    goto abc;
                            }
                        }
                        //20
                        foreach (AIHeroClient hero in HeroManager.Enemies.Where(x => x.NetworkId != targetQ.NetworkId &&
                                                                                x.BadaoIsValidTarget(BadaoMainVariables.Q.Range)))
                        {
                            var Qpred = BadaoMainVariables.Q.GetPrediction(hero);
                            var PredHero = Prediction.GetPrediction(hero, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                            1400 + Game.Ping / 1000));
                            var PredTargetQ = Prediction.GetPrediction(targetQ, 0.25f +
                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                            Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredHero.UnitPosition.To2D(),
                                ObjectManager.Player.Position.To2D().Distance(PredHero.UnitPosition.To2D()) + 500);
                            if (BadaoMissFortuneHelper.Q1Damage(hero) >= hero.Health &&
                                BadaoChecker.BadaoInTheCone(PredTargetQ.UnitPosition.To2D(), PredHero.UnitPosition.To2D(), endpos, 20) &&
                                !MinionManager.GetMinions(BadaoMainVariables.Q.Range + 600).Any(x =>
                                BadaoChecker.BadaoInTheCone(Prediction.GetPrediction(x, 0.25f +
                                                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                                    1400 + Game.Ping / 1000)).UnitPosition.To2D(),
                                                                                    PredHero.UnitPosition.To2D(), endpos, 20)))
                            {
                                if (BadaoMainVariables.Q.Cast(hero) == Spell.CastStates.SuccessfullyCasted)
                                    goto abc;
                            }
                        }
                        foreach (Obj_AI_Minion minion in MinionManager.GetMinions(BadaoMainVariables.Q.Range))
                        {
                            var Qpred = BadaoMainVariables.Q.GetPrediction(minion);
                            var PredMinion = Prediction.GetPrediction(minion, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                            1400 + Game.Ping / 1000));
                            var PredTargetQ = Prediction.GetPrediction(targetQ, 0.25f +
                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                            Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredMinion.UnitPosition.To2D(),
                                ObjectManager.Player.Position.To2D().Distance(PredMinion.UnitPosition.To2D()) + 500);
                            if (BadaoMissFortuneHelper.Q1Damage(minion) >= minion.Health &&
                                BadaoChecker.BadaoInTheCone(PredTargetQ.UnitPosition.To2D(), PredMinion.UnitPosition.To2D(), endpos, 20) &&
                                !MinionManager.GetMinions(BadaoMainVariables.Q.Range + 500).Any(x =>
                                x.NetworkId != minion.NetworkId &&
                                BadaoChecker.BadaoInTheCone(Prediction.GetPrediction(x, 0.25f +
                                                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                                    1400 + Game.Ping / 1000)).UnitPosition.To2D(),
                                                                                    PredMinion.UnitPosition.To2D(), endpos, 20)))
                            {
                                if (BadaoMainVariables.Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                                    goto abc;
                            }
                        }
                        foreach (AIHeroClient hero in HeroManager.Enemies.Where(x => x.NetworkId != targetQ.NetworkId &&
                                                                                x.BadaoIsValidTarget(BadaoMainVariables.Q.Range)))
                        {
                            var Qpred = BadaoMainVariables.Q.GetPrediction(hero);
                            var PredHero = Prediction.GetPrediction(hero, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                            1400 + Game.Ping / 1000));
                            var PredTargetQ = Prediction.GetPrediction(targetQ, 0.25f +
                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                            Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredHero.UnitPosition.To2D(),
                                ObjectManager.Player.Position.To2D().Distance(PredHero.UnitPosition.To2D()) + 500);
                            if (BadaoChecker.BadaoInTheCone(PredTargetQ.UnitPosition.To2D(), PredHero.UnitPosition.To2D(), endpos, 20) &&
                                !MinionManager.GetMinions(BadaoMainVariables.Q.Range + 600).Any(x =>
                                BadaoChecker.BadaoInTheCone(Prediction.GetPrediction(x, 0.25f +
                                                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                                    1400 + Game.Ping / 1000)).UnitPosition.To2D(),
                                                                                    PredHero.UnitPosition.To2D(), endpos, 20)))
                            {
                                if (BadaoMainVariables.Q.Cast(hero) == Spell.CastStates.SuccessfullyCasted)
                                    goto abc;
                            }
                        }
                        foreach (Obj_AI_Minion minion in MinionManager.GetMinions(BadaoMainVariables.Q.Range))
                        {
                            var Qpred = BadaoMainVariables.Q.GetPrediction(minion);
                            var PredMinion = Prediction.GetPrediction(minion, 0.25f + ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                            1400 + Game.Ping / 1000));
                            var PredTargetQ = Prediction.GetPrediction(targetQ, 0.25f +
                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D()) / 1400 + Game.Ping / 1000);
                            Vector2 endpos = Geometry.Extend(ObjectManager.Player.Position.To2D(), PredMinion.UnitPosition.To2D(),
                                ObjectManager.Player.Position.To2D().Distance(PredMinion.UnitPosition.To2D()) + 500);
                            if (BadaoChecker.BadaoInTheCone(PredTargetQ.UnitPosition.To2D(), PredMinion.UnitPosition.To2D(), endpos, 20) &&
                                !MinionManager.GetMinions(BadaoMainVariables.Q.Range + 500).Any(x =>
                                x.NetworkId != minion.NetworkId &&
                                BadaoChecker.BadaoInTheCone(Prediction.GetPrediction(x, 0.25f +
                                                                                    ObjectManager.Player.Position.To2D().Distance(Qpred.UnitPosition.To2D() /
                                                                                    1400 + Game.Ping / 1000)).UnitPosition.To2D(),
                                                                                    PredMinion.UnitPosition.To2D(), endpos, 20)))
                            {
                                if (BadaoMainVariables.Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                                    goto abc;
                            }
                        }
                    }
                }
            abc:;
            }
            // Q1 logic
            if (BadaoMissFortuneHelper.UseQ1Harass() && Orbwalking.CanMove(80))
            {
                var targetQ1 = TargetSelector.GetTarget(BadaoMainVariables.Q.Range, TargetSelector.DamageType.Physical);
                if (targetQ1.BadaoIsValidTarget())
                {
                    if (BadaoMainVariables.Q.Cast(targetQ1) == Spell.CastStates.SuccessfullyCasted)
                        goto abc;
                }
            abc:;
            }
            // E logic
            if (BadaoMissFortuneHelper.UseEHarass() && Orbwalking.CanMove(80))
            {
                var targetE = TargetSelector.GetTarget(BadaoMainVariables.E.Range + 200, TargetSelector.DamageType.Physical);
                if (targetE.BadaoIsValidTarget())
                {
                    var PredTargetE = Prediction.GetPrediction(targetE, 0.25f);
                    if (PredTargetE.UnitPosition.To2D().Distance(ObjectManager.Player.Position.To2D()) <= BadaoMainVariables.E.Range)
                    {
                        if (BadaoMainVariables.E.Cast(PredTargetE.UnitPosition) == true)
                            goto xyz;
                    }
                }
            xyz:;
            }
        }
    }
}
