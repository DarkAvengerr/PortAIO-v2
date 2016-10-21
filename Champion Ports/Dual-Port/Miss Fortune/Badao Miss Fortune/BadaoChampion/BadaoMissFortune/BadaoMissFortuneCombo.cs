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
    public static class BadaoMissFortuneCombo
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate; // Q,E
            Orbwalking.AfterAttack += Orbwalking_AfterAttack; // R
            Orbwalking.OnAttack += Orbwalking_OnAttack; // W
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Game.OnWndProc += Game_OnWndProc;
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowsMessages.WM_KEYDOWN && args.WParam == 'R' && BadaoMainVariables.R.IsReady())
            {
                BadaoMissFortuneVariables.Rcount = Utils.GameTimeTickCount;
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || args.Slot != SpellSlot.R)
                return;
            BadaoMissFortuneVariables.Rcount = Utils.GameTimeTickCount;
            Vector2 x1 = new Vector2();
            Vector2 x2 = new Vector2();
            Vector2 CenterPolar = new Vector2();
            Vector2 CenterEnd = new Vector2();
            BadaoMissFortuneHelper.RPrediction(args.End.To2D(), ObjectManager.Player,
                out CenterPolar, out CenterEnd, out x1, out x2);
            BadaoMissFortuneVariables.CenterPolar = CenterPolar;
            BadaoMissFortuneVariables.CenterEnd = CenterEnd;
        }

        private static void Orbwalking_OnAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe || BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;
            if (Utils.GameTimeTickCount - BadaoMissFortuneVariables.Rcount <= 500)
                return;
            if (ObjectManager.Player.IsChannelingImportantSpell())
            {
                return;
            }
            if (BadaoMissFortuneHelper.UseWCombo() && target.BadaoIsValidTarget() && target is AIHeroClient)
            {
                BadaoMainVariables.W.Cast();
            }
        }

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;
            if (Utils.GameTimeTickCount - BadaoMissFortuneVariables.Rcount <= 500)
                return;
            if (BadaoMissFortuneHelper.UseRCombo() && unit.IsMe 
                && target.BadaoIsValidTarget() &&
                target is AIHeroClient && BadaoMissFortuneHelper.Rdamepior() 
                && target.Health <= 0.6f*BadaoMissFortuneHelper.RDamage(target as Obj_AI_Base))
            {
                float reactiontime = 0.5f;
                var PredTarget = Prediction.GetPrediction(target as Obj_AI_Base, 0.25f + Game.Ping/1000f);
                Vector2 x1 = new Vector2();
                Vector2 x2 = new Vector2();
                Vector2 CenterPolar = new Vector2();
                Vector2 CenterEnd = new Vector2();
                BadaoMissFortuneHelper.RPrediction(PredTarget.UnitPosition.To2D(), target as Obj_AI_Base,
                    out CenterPolar, out CenterEnd, out x1, out x2);
                float dis1 = PredTarget.UnitPosition.To2D().Distance(x1);
                float dis2 = PredTarget.UnitPosition.To2D().Distance(x2);
                AIHeroClient Target = target as AIHeroClient;
                if (PredTarget.UnitPosition.To2D().Distance(ObjectManager.Player.Position.To2D()) >= 250 &&
                    (Target.HasBuffOfType(BuffType.Stun) || Target.HasBuffOfType(BuffType.Snare) ||
                    (dis1 >= dis2 && (dis2 / Target.MoveSpeed >= 0.6f * 3f - reactiontime ||
                    BadaoMissFortuneHelper.RDamage(Target) * (dis2 / Target.MoveSpeed + reactiontime)/ 3f >= Target.Health
                    - BadaoMissFortuneHelper.GetAADamage(Target)))))
                {
                    BadaoMainVariables.R.Cast(PredTarget.UnitPosition.To2D());
                    BadaoMissFortuneVariables.TargetRChanneling = target as AIHeroClient;
                    BadaoMissFortuneVariables.CenterPolar = CenterPolar;
                    BadaoMissFortuneVariables.CenterEnd = CenterEnd;
                    BadaoMissFortuneVariables.Rcount = Utils.GameTimeTickCount;
                }
                else if (PredTarget.UnitPosition.To2D().Distance(ObjectManager.Player.Position.To2D()) >= 250 &&
                        (Target.HasBuffOfType(BuffType.Stun) || Target.HasBuffOfType(BuffType.Snare) ||
                        (dis2 >= dis1 && (dis1 / Target.MoveSpeed >= 0.6f * 3f - reactiontime ||
                        BadaoMissFortuneHelper.RDamage(Target)* (dis1 / Target.MoveSpeed + reactiontime)/3f >= Target.Health
                        - BadaoMissFortuneHelper.GetAADamage(Target)))))
                {
                    BadaoMainVariables.R.Cast(PredTarget.UnitPosition.To2D());
                    BadaoMissFortuneVariables.TargetRChanneling = target as AIHeroClient;
                    BadaoMissFortuneVariables.CenterPolar = CenterPolar;
                    BadaoMissFortuneVariables.CenterEnd = CenterEnd;
                    BadaoMissFortuneVariables.Rcount = Utils.GameTimeTickCount;
                }
            }
            if (BadaoMissFortuneHelper.UseRComboWise() && unit.IsMe
                && target.BadaoIsValidTarget() &&
                target is AIHeroClient && BadaoMissFortuneHelper.Rdamepior()
                && target.Health <= 0.8f * BadaoMissFortuneHelper.RDamage(target as Obj_AI_Base))
            {
                float reactiontime = 1f;
                var PredTarget = Prediction.GetPrediction(target as Obj_AI_Base, 0.25f + Game.Ping / 1000f);
                Vector2 x1 = new Vector2();
                Vector2 x2 = new Vector2();
                Vector2 CenterPolar = new Vector2();
                Vector2 CenterEnd = new Vector2();
                BadaoMissFortuneHelper.RPrediction(PredTarget.UnitPosition.To2D(), target as Obj_AI_Base,
                    out CenterPolar, out CenterEnd, out x1, out x2);
                float dis1 = PredTarget.UnitPosition.To2D().Distance(x1);
                float dis2 = PredTarget.UnitPosition.To2D().Distance(x2);
                AIHeroClient Target = target as AIHeroClient;
                if (PredTarget.UnitPosition.To2D().Distance(ObjectManager.Player.Position.To2D()) >= 250 &&
                    (Target.HasBuffOfType(BuffType.Stun) || Target.HasBuffOfType(BuffType.Snare) ||
                    (dis1 >= dis2 && (dis2 / Target.MoveSpeed >= 0.8f * 3f - reactiontime ||
                    BadaoMissFortuneHelper.RDamage(Target) * (dis2 / Target.MoveSpeed + reactiontime) / 3f >= Target.Health
                    - BadaoMissFortuneHelper.GetAADamage(Target)))))
                {
                    BadaoMainVariables.R.Cast(PredTarget.UnitPosition.To2D());
                    BadaoMissFortuneVariables.TargetRChanneling = target as AIHeroClient;
                    BadaoMissFortuneVariables.CenterPolar = CenterPolar;
                    BadaoMissFortuneVariables.CenterEnd = CenterEnd;
                    BadaoMissFortuneVariables.Rcount = Utils.GameTimeTickCount;
                }
                else if (PredTarget.UnitPosition.To2D().Distance(ObjectManager.Player.Position.To2D()) >= 250 &&
                        (Target.HasBuffOfType(BuffType.Stun) || Target.HasBuffOfType(BuffType.Snare) ||
                        (dis2 >= dis1 && (dis1 / Target.MoveSpeed >= 0.8f * 3f - reactiontime ||
                        BadaoMissFortuneHelper.RDamage(Target) * (dis1 / Target.MoveSpeed + reactiontime) / 3f >= Target.Health
                        - BadaoMissFortuneHelper.GetAADamage(Target)))))
                {
                    BadaoMainVariables.R.Cast(PredTarget.UnitPosition.To2D());
                    BadaoMissFortuneVariables.TargetRChanneling = target as AIHeroClient;
                    BadaoMissFortuneVariables.CenterPolar = CenterPolar;
                    BadaoMissFortuneVariables.CenterEnd = CenterEnd;
                    BadaoMissFortuneVariables.Rcount = Utils.GameTimeTickCount;
                }
            }
        }
        private static void Game_OnUpdate(EventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;
            // cancle R
            if (Utils.GameTimeTickCount - BadaoMissFortuneVariables.Rcount <= 500)
                return;
            if (ObjectManager.Player.IsChannelingImportantSpell())
            {
                if (!HeroManager.Enemies.Any(x => x.BadaoIsValidTarget() &&
                BadaoChecker.BadaoInTheCone(x.Position.To2D(),
                BadaoMissFortuneVariables.CenterPolar, BadaoMissFortuneVariables.CenterEnd, 36)))
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
                else
                    return;
            }
            // Q logic
            if (BadaoMissFortuneHelper.UseQ2Combo() && Orbwalking.CanMove(80))
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
            if (BadaoMissFortuneHelper.UseQ1Combo() && Orbwalking.CanMove(80))
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
            if (BadaoMissFortuneHelper.UseECombo() && Orbwalking.CanMove(80))
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
            // R logic
            if (BadaoMainVariables.R.IsReady() && Orbwalking.CanMove(80) && BadaoMissFortuneVariables.ComboRifhit.GetValue<bool>())
            {
                foreach (AIHeroClient hero in HeroManager.Enemies)
                {
                    List<AIHeroClient> a = new List<AIHeroClient>();
                    if (hero.BadaoIsValidTarget())
                    {
                        var PredTarget = Prediction.GetPrediction(hero as Obj_AI_Base, 0.25f + Game.Ping / 1000f);
                        Vector2 x1 = new Vector2();
                        Vector2 x2 = new Vector2();
                        Vector2 CenterPolar = new Vector2();
                        Vector2 CenterEnd = new Vector2();
                        BadaoMissFortuneHelper.RPrediction(PredTarget.UnitPosition.To2D(), hero as Obj_AI_Base,
                            out CenterPolar, out CenterEnd, out x1, out x2);
                        foreach (AIHeroClient hero2 in HeroManager.Enemies)
                        {
                            if (hero2.BadaoIsValidTarget() && BadaoChecker.BadaoInTheCone(hero2.Position.To2D(), CenterPolar, CenterEnd, 36))
                            {
                                a.Add(hero2);
                            }
                        }
                        if (a.Count() >= BadaoMissFortuneVariables.ComboRifwillhit.GetValue<Slider>().Value)
                        {
                            BadaoMainVariables.R.Cast(PredTarget.UnitPosition.To2D());
                            BadaoMissFortuneVariables.TargetRChanneling = hero as AIHeroClient;
                            BadaoMissFortuneVariables.CenterPolar = CenterPolar;
                            BadaoMissFortuneVariables.CenterEnd = CenterEnd;
                            BadaoMissFortuneVariables.Rcount = Utils.GameTimeTickCount;
                        }
                    }
                }
            }
        }

    }
}
