using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Prediction
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class xcsoftPrediction
    {
        public static float PredHealth(Obj_AI_Base Target, Spell spell)
            =>
            HealthPrediction.GetHealthPrediction(Target, (int) (ObjectManager.Player.Distance(Target)/spell.Speed),
                (int) (spell.Delay*1000 + Game.Ping/2));

        public static void CastCircle(this Spell spell, Obj_AI_Base target)
        {
            if (spell.Type == SkillshotType.SkillshotCircle || spell.Type == SkillshotType.SkillshotCone)
            {
                if (target == null)
                {
                    return;
                }

                var pred = Prediction.GetPrediction(target, spell.Delay, spell.Width / 2, spell.Speed);
                var castVec = (pred.UnitPosition.To2D() + target.ServerPosition.To2D()) / 2;
                var castVec2 = ObjectManager.Player.ServerPosition.To2D() +
                               Vector2.Normalize(pred.UnitPosition.To2D() - ObjectManager.Player.Position.To2D())*
                               spell.Range;

                if (target.IsValidTarget(spell.Range))
                {
                    if (
                        !(target.MoveSpeed*
                          (Game.Ping/2000 + spell.Delay +
                           ObjectManager.Player.ServerPosition.Distance(target.ServerPosition)/spell.Speed) <=
                          spell.Width*1/2))
                    {
                        if (pred.Hitchance < HitChance.VeryHigh ||
                            !(pred.UnitPosition.Distance(target.ServerPosition) < Math.Max(spell.Width, 300f)))
                        {
                            return;
                        }

                        if (!(target.MoveSpeed*
                              (Game.Ping/2000 + spell.Delay +
                               ObjectManager.Player.ServerPosition.Distance(target.ServerPosition)/spell.Speed) <=
                              spell.Width*2/3) || !(castVec.Distance(pred.UnitPosition) <= spell.Width*1/2) ||
                            !(castVec.Distance(ObjectManager.Player.ServerPosition) <= spell.Range))
                        {
                            if (castVec.Distance(pred.UnitPosition) > spell.Width*1/2 &&
                                ObjectManager.Player.ServerPosition.Distance(pred.UnitPosition) <= spell.Range)
                            {
                                spell.Cast(pred.UnitPosition);
                            }
                            else
                            {
                                spell.Cast(pred.CastPosition);
                            }
                        }
                        else
                        {
                            spell.Cast(castVec);
                        }
                    }
                    else
                    {
                        spell.Cast(target.ServerPosition);
                    }
                }
                else if (target.IsValidTarget(spell.Range + spell.Width / 2))
                {
                    if (pred.Hitchance < HitChance.VeryHigh ||
                        !(ObjectManager.Player.ServerPosition.Distance(pred.UnitPosition) <=
                          spell.Range + spell.Width*1/2) ||
                        !(pred.UnitPosition.Distance(target.ServerPosition) < Math.Max(spell.Width, 400f)))
                    {
                        return;
                    }

                    if (!(ObjectManager.Player.ServerPosition.Distance(pred.UnitPosition) <= spell.Range))
                    {
                        if (!(ObjectManager.Player.ServerPosition.Distance(pred.UnitPosition) <=
                              spell.Range + spell.Width*1/2) || !(target.MoveSpeed*
                                                                  (Game.Ping/2000 + spell.Delay +
                                                                   ObjectManager.Player.ServerPosition.Distance(
                                                                       target.ServerPosition)/spell.Speed) <=
                                                                  spell.Width/2))
                        {
                            return;
                        }

                        if (ObjectManager.Player.Distance(castVec2) <= spell.Range)
                        {
                            spell.Cast(castVec2);
                        }
                    }
                    else
                    {
                        if (ObjectManager.Player.ServerPosition.Distance(pred.CastPosition) <= spell.Range)
                        {
                            spell.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }

        public static void CastLine(this Spell spell, Obj_AI_Base target, float alpha = 0f,
            float colmini = float.MaxValue, bool HeroOnly = false, float BombRadius = 0f)
        {
            if (spell.Type != SkillshotType.SkillshotLine)
            {
                return;
            }

            if (target == null)
            {
                return;
            }

            var pred = Prediction.GetPrediction(target, spell.Delay, spell.Width / 2, spell.Speed);
            var collision = spell.GetCollision(ObjectManager.Player.ServerPosition.To2D(),
                new List<Vector2> {pred.CastPosition.To2D()});
            var minioncol = collision.Count(x => HeroOnly == false ? x.IsMinion : x is AIHeroClient);
            var EditedVec = pred.UnitPosition.To2D() -
                            Vector2.Normalize(pred.UnitPosition.To2D() - target.ServerPosition.To2D())*(spell.Width*2/5);
            var EditedVec2 = (pred.UnitPosition.To2D() + target.ServerPosition.To2D()) / 2;
            var collision2 = spell.GetCollision(ObjectManager.Player.ServerPosition.To2D(),
                new List<Vector2> {EditedVec});
            var minioncol2 = collision2.Count(x => HeroOnly == false ? x.IsMinion : x is AIHeroClient);
            var collision3 = spell.GetCollision(ObjectManager.Player.ServerPosition.To2D(),
                new List<Vector2> {EditedVec2});
            var minioncol3 = collision3.Count(x => HeroOnly == false ? x.IsMinion : x is AIHeroClient);

            if (pred.Hitchance >= HitChance.VeryHigh)
            {
                if (
                    !target.IsValidTarget(spell.Range -
                                          target.MoveSpeed*
                                          (spell.Delay +
                                           ObjectManager.Player.Distance(target.ServerPosition)/spell.Speed) + alpha) ||
                    !(minioncol2 <= colmini) || !(pred.UnitPosition.Distance(target.ServerPosition) > spell.Width))
                {
                    if (
                        target.IsValidTarget(spell.Range -
                                             target.MoveSpeed*
                                             (spell.Delay +
                                              ObjectManager.Player.Distance(target.ServerPosition)/spell.Speed) + alpha) &&
                        minioncol3 <= colmini && pred.UnitPosition.Distance(target.ServerPosition) > spell.Width/2)
                    {
                        spell.Cast(EditedVec2);
                    }
                    else if (
                        target.IsValidTarget(spell.Range -
                                             target.MoveSpeed*
                                             (spell.Delay +
                                              ObjectManager.Player.Distance(target.ServerPosition)/spell.Speed) + alpha) &&
                        minioncol <= colmini)
                    {
                        spell.Cast(pred.CastPosition);
                    }
                    else if (false == spell.Collision && colmini < 1 && minioncol >= 1)
                    {
                        var FirstMinion =
                            collision.OrderBy(o => o.Distance(ObjectManager.Player.ServerPosition)).FirstOrDefault();

                        if (FirstMinion != null &&
                            FirstMinion.ServerPosition.Distance(pred.UnitPosition) <= BombRadius/4)
                        {
                            spell.Cast(pred.CastPosition);
                        }
                    }
                }
                else
                {
                    spell.Cast(EditedVec);
                }
            }
        }

        public static void CastCone(this Spell spell, Obj_AI_Base target, float alpha = 0f,
            float colmini = float.MaxValue, bool HeroOnly = false)
        {
            if (spell.Type != SkillshotType.SkillshotCone)
            {
                return;
            }

            if (target == null)
            {
                return;
            }

            var pred = Prediction.GetPrediction(target, spell.Delay, spell.Width / 2, spell.Speed);
            var collision = spell.GetCollision(ObjectManager.Player.ServerPosition.To2D(),
                new List<Vector2> {pred.CastPosition.To2D()});
            var minioncol = collision.Count(x => HeroOnly == false ? x.IsMinion : x is AIHeroClient);

            if (
                target.IsValidTarget(spell.Range -
                                     target.MoveSpeed*
                                     (spell.Delay + ObjectManager.Player.Distance(target.ServerPosition)/spell.Speed) +
                                     alpha) && minioncol <= colmini && pred.Hitchance >= HitChance.VeryHigh)
            {
                spell.Cast(pred.CastPosition);
            }
        }

        public static void CastAOE(this Spell spell, Obj_AI_Base target)
        {
            if (spell == null || target == null)
            {
                return;
            }

            var pred = Prediction.GetPrediction(target, spell.Delay > 0 ? spell.Delay : 0.25f, spell.Range);

            if (pred.Hitchance >= HitChance.High &&
                pred.UnitPosition.Distance(ObjectManager.Player.ServerPosition) <= spell.Range)
            {
                spell.Cast();
            }
        }

        public static void RMouse(this Spell spell)
        {
            var ReverseVec = ObjectManager.Player.ServerPosition.To2D() -
                             Vector2.Normalize(Game.CursorPos.To2D() - ObjectManager.Player.Position.To2D())*
                             spell.Range;

            if (!spell.IsReady())
            {
                return;
            }

            spell.Cast(ReverseVec);
        }

        public static void NMouse(this Spell spell)
        {
            var NVec = ObjectManager.Player.ServerPosition.To2D() +
                       Vector2.Normalize(Game.CursorPos.To2D() - ObjectManager.Player.Position.To2D())*spell.Range;

            if (!spell.IsReady())
            {
                return;
            }

            spell.Cast(NVec);
        }

        public static void RTarget(this Spell spell, Obj_AI_Base Target)
        {
            var ReverseVec = ObjectManager.Player.ServerPosition.To2D() -
                             Vector2.Normalize(Target.ServerPosition.To2D() - ObjectManager.Player.Position.To2D())*
                             spell.Range;

            if (!spell.IsReady())
            {
                return;
            }

            spell.Cast(ReverseVec);
        }

        public static void NTarget(this Spell spell, Obj_AI_Base Target)
        {
            var Vec = ObjectManager.Player.ServerPosition.To2D() +
                      Vector2.Normalize(Target.ServerPosition.To2D() - ObjectManager.Player.Position.To2D())*
                      spell.Range;

            if (!spell.IsReady())
            {
                return;
            }

            spell.Cast(Vec);
        }

        public static bool CanHit(this Spell spell, Obj_AI_Base T, float Drag = 0f)
        {
            return
                T.IsValidTarget(spell.Range + Drag -
                                ((T.Distance(ObjectManager.Player.ServerPosition) - spell.Range)/spell.Speed +
                                 spell.Delay)*T.MoveSpeed);
        }
    }
}
