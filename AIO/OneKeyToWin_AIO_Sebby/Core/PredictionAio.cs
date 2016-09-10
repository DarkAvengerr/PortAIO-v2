using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace OneKeyToWin_AIO_Sebby.Core
{
    static class PredictionAio
    {

        static AIHeroClient Player { get { return ObjectManager.Player; } }


        internal static void CCast(this Spell spell, Obj_AI_Base target, HitChance SelectedHitchance) //for Circular spells
        {
            if (spell.Type == SkillshotType.SkillshotCircle || spell.Type == SkillshotType.SkillshotCone) // Cone 스킬은 임시로
            {
                if (spell != null && target != null)
                {
                    var pred = Prediction.GetPrediction(target, spell.Delay, spell.Width / 2, spell.Speed);
                    SharpDX.Vector2 castVec = (pred.UnitPosition.To2D() + target.ServerPosition.To2D()) / 2;
                    SharpDX.Vector2 castVec2 = Player.ServerPosition.To2D() +
                                               SharpDX.Vector2.Normalize(pred.UnitPosition.To2D() - Player.Position.To2D()) * (spell.Range);

                    if (target.IsValidTarget(spell.Range))
                    {
                        if (target.MoveSpeed * (Game.Ping / 2000 + spell.Delay + Player.ServerPosition.Distance(target.ServerPosition) / spell.Speed) <= spell.Width * 1 / 2)
                            spell.Cast(target.ServerPosition); //Game.Ping/2000  추가함.
                        else if (pred.Hitchance >= SelectedHitchance && pred.UnitPosition.Distance(target.ServerPosition) < Math.Max(spell.Width, 300f))
                        {
                            if (target.MoveSpeed * (Game.Ping / 2000 + spell.Delay + Player.ServerPosition.Distance(target.ServerPosition) / spell.Speed) <= spell.Width * 2 / 3 && castVec.Distance(pred.UnitPosition) <= spell.Width * 1 / 2 && castVec.Distance(Player.ServerPosition) <= spell.Range)
                            {
                                spell.Cast(castVec);
                            }
                            else if (castVec.Distance(pred.UnitPosition) > spell.Width * 1 / 2 && Player.ServerPosition.Distance(pred.UnitPosition) <= spell.Range)
                            {
                                spell.Cast(pred.UnitPosition);
                            }
                            else
                                spell.Cast(pred.CastPosition); // <- 별로 좋은 선택은 아니지만.. 
                        }
                    }
                    else if (target.IsValidTarget(spell.Range + spell.Width / 2)) //사거리 밖 대상에 대해서
                    {
                        if (pred.Hitchance >= SelectedHitchance && Player.ServerPosition.Distance(pred.UnitPosition) <= spell.Range + spell.Width * 1 / 2 && pred.UnitPosition.Distance(target.ServerPosition) < Math.Max(spell.Width, 400f))
                        {
                            if (Player.ServerPosition.Distance(pred.UnitPosition) <= spell.Range)
                            {
                                if (Player.ServerPosition.Distance(pred.CastPosition) <= spell.Range)
                                    spell.Cast(pred.CastPosition);
                            }
                            else if (Player.ServerPosition.Distance(pred.UnitPosition) <= spell.Range + spell.Width * 1 / 2 && target.MoveSpeed * (Game.Ping / 2000 + spell.Delay + Player.ServerPosition.Distance(target.ServerPosition) / spell.Speed) <= spell.Width / 2)
                            {
                                if (Player.Distance(castVec2) <= spell.Range)
                                    spell.Cast(castVec2);
                            }
                        }
                    }
                }
            }
        }

        internal static void LCast(this Spell spell, Obj_AI_Base target, HitChance SelectedHitchance, float alpha = 0f, float colmini = float.MaxValue, bool HeroOnly = false, float BombRadius = 0f) //for Linar spells  사용예시 AIO_Func.LCast(Q,Qtarget,50,0)  
        {                            //        AIO_Func.LCast(E,Etarget,Menu.Item("Misc.Etg").GetValue<Slider>().Value,float.MaxValue); <- 이런식으로 사용.
            if (spell.Type == SkillshotType.SkillshotLine)
            {
                if (spell != null && target != null)
                {
                    var pred = Prediction.GetPrediction(target, spell.Delay, spell.Width / 2, spell.Speed); //spell.Width/2
                    var collision = spell.GetCollision(Player.ServerPosition.To2D(), new List<SharpDX.Vector2> { pred.CastPosition.To2D() });
                    //var minioncol = collision.Where(x => !(x is AIHeroClient)).Count(x => x.IsMinion);
                    var minioncol = collision.Count(x => (HeroOnly == false ? x.IsMinion : (x is AIHeroClient)));
                    SharpDX.Vector2 EditedVec = pred.UnitPosition.To2D() -
                                               SharpDX.Vector2.Normalize(pred.UnitPosition.To2D() - target.ServerPosition.To2D()) * (spell.Width * 2 / 5);
                    SharpDX.Vector2 EditedVec2 = (pred.UnitPosition.To2D() + target.ServerPosition.To2D()) / 2;

                    var collision2 = spell.GetCollision(Player.ServerPosition.To2D(), new List<SharpDX.Vector2> { EditedVec });
                    var minioncol2 = collision2.Count(x => (HeroOnly == false ? x.IsMinion : (x is AIHeroClient)));
                    var collision3 = spell.GetCollision(Player.ServerPosition.To2D(), new List<SharpDX.Vector2> { EditedVec2 });
                    var minioncol3 = collision3.Count(x => (HeroOnly == false ? x.IsMinion : (x is AIHeroClient)));
                    if (pred.Hitchance >= SelectedHitchance)
                    {
                        if (target.IsValidTarget(spell.Range - target.MoveSpeed * (spell.Delay + Player.Distance(target.ServerPosition) / spell.Speed) + alpha) && minioncol2 <= colmini && pred.UnitPosition.Distance(target.ServerPosition) > spell.Width)
                        {
                            spell.Cast(EditedVec);
                        }
                        else if (target.IsValidTarget(spell.Range - target.MoveSpeed * (spell.Delay + Player.Distance(target.ServerPosition) / spell.Speed) + alpha) && minioncol3 <= colmini && pred.UnitPosition.Distance(target.ServerPosition) > spell.Width / 2)
                        {
                            spell.Cast(EditedVec2);
                        }
                        else if (target.IsValidTarget(spell.Range - target.MoveSpeed * (spell.Delay + Player.Distance(target.ServerPosition) / spell.Speed) + alpha) && minioncol <= colmini)
                        {
                            spell.Cast(pred.CastPosition);
                        }
                        else if (false == spell.Collision && colmini < 1 && minioncol >= 1)
                        {
                            var FirstMinion = collision.OrderBy(o => o.Distance(Player.ServerPosition)).FirstOrDefault();
                            if (FirstMinion.ServerPosition.Distance(pred.UnitPosition) <= BombRadius / 4)
                                spell.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }

        internal static void ConeCast(this Spell spell, Obj_AI_Base target, HitChance SelectedHitchance, float alpha = 0f, float colmini = float.MaxValue, bool HeroOnly = false)
        {
            if (spell.Type == SkillshotType.SkillshotCone)
            {
                if (spell != null && target != null)
                {
                    var pred = Prediction.GetPrediction(target, spell.Delay, spell.Width / 2, spell.Speed); //spell.Width/2
                    var collision = spell.GetCollision(Player.ServerPosition.To2D(), new List<SharpDX.Vector2> { pred.CastPosition.To2D() });
                    var minioncol = collision.Count(x => (HeroOnly == false ? x.IsMinion : (x is AIHeroClient)));
                    if (target.IsValidTarget(spell.Range - target.MoveSpeed * (spell.Delay + Player.Distance(target.ServerPosition) / spell.Speed) + alpha) && minioncol <= colmini && pred.Hitchance >= SelectedHitchance)
                    {
                        spell.Cast(pred.CastPosition);
                    }
                }
            }
        }

        internal static void AOECast(this Spell spell, Obj_AI_Base target)
        {
            if (spell != null && target != null)
            {
                var pred = Prediction.GetPrediction(target, spell.Delay > 0 ? spell.Delay : 0.25f, spell.Range);
                if (pred.Hitchance >= HitChance.High && pred.UnitPosition.Distance(Player.ServerPosition) <= spell.Range)
                {
                    spell.Cast();
                }
            }
        }



    }
}