using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Nidalee.Prediction
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
        {
            return HealthPrediction.GetHealthPrediction(Target, (int)(ObjectManager.Player.Distance(Target, false) / spell.Speed), (int)(spell.Delay * 1000 + Game.Ping / 2));
        }

        public static void CastCircle(this Spell spell, Obj_AI_Base target)
        {
            if (spell.Type == SkillshotType.SkillshotCircle || spell.Type == SkillshotType.SkillshotCone)
            {
                if (spell != null && target != null)
                {
                    var pred = Prediction.GetPrediction(target, spell.Delay, spell.Width / 2, spell.Speed);

                    Vector2 castVec = (pred.UnitPosition.To2D() + target.ServerPosition.To2D()) / 2;
                    Vector2 castVec2 = ObjectManager.Player.ServerPosition.To2D() + Vector2.Normalize(pred.UnitPosition.To2D() - ObjectManager.Player.Position.To2D()) * (spell.Range);

                    if (target.IsValidTarget(spell.Range))
                    {
                        if (target.MoveSpeed * (Game.Ping / 2000 + spell.Delay + ObjectManager.Player.ServerPosition.Distance(target.ServerPosition) / spell.Speed) <= spell.Width * 1 / 2)
                        {
                            spell.Cast(target.ServerPosition);
                        }
                        else if (pred.Hitchance >= HitChance.VeryHigh && pred.UnitPosition.Distance(target.ServerPosition) < Math.Max(spell.Width, 300f))
                        {
                            if (target.MoveSpeed * (Game.Ping / 2000 + spell.Delay + ObjectManager.Player.ServerPosition.Distance(target.ServerPosition) / spell.Speed) <= spell.Width * 2 / 3 && castVec.Distance(pred.UnitPosition) <= spell.Width * 1 / 2 && castVec.Distance(ObjectManager.Player.ServerPosition) <= spell.Range)
                            {
                                spell.Cast(castVec);
                            }
                            else if (castVec.Distance(pred.UnitPosition) > spell.Width * 1 / 2 && ObjectManager.Player.ServerPosition.Distance(pred.UnitPosition) <= spell.Range)
                            {
                                spell.Cast(pred.UnitPosition);
                            }
                            else
                            {
                                spell.Cast(pred.CastPosition);
                            }
                        }
                    }
                    else if (target.IsValidTarget(spell.Range + spell.Width / 2))
                    {
                        if (pred.Hitchance >= HitChance.VeryHigh && ObjectManager.Player.ServerPosition.Distance(pred.UnitPosition) <= spell.Range + spell.Width * 1 / 2 && pred.UnitPosition.Distance(target.ServerPosition) < Math.Max(spell.Width, 400f))
                        {
                            if (ObjectManager.Player.ServerPosition.Distance(pred.UnitPosition) <= spell.Range)
                            {
                                if (ObjectManager.Player.ServerPosition.Distance(pred.CastPosition) <= spell.Range)
                                {
                                    spell.Cast(pred.CastPosition);
                                }
                            }
                            else if (ObjectManager.Player.ServerPosition.Distance(pred.UnitPosition) <= spell.Range + spell.Width * 1 / 2 && target.MoveSpeed * (Game.Ping / 2000 + spell.Delay + ObjectManager.Player.ServerPosition.Distance(target.ServerPosition) / spell.Speed) <= spell.Width / 2)
                            {
                                if (ObjectManager.Player.Distance(castVec2) <= spell.Range)
                                {
                                    spell.Cast(castVec2);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void CastLine(this Spell spell, Obj_AI_Base target, float alpha = 0f, float colmini = float.MaxValue, bool HeroOnly = false, float BombRadius = 0f)
        {
            if (spell.Type == SkillshotType.SkillshotLine)
            {
                if (spell != null && target != null)
                {
                    var pred = Prediction.GetPrediction(target, spell.Delay, spell.Width / 2, spell.Speed);
                    var collision = spell.GetCollision(ObjectManager.Player.ServerPosition.To2D(), new List<Vector2> { pred.CastPosition.To2D() });
                    var minioncol = collision.Count(x => (HeroOnly == false ? x.IsMinion : (x is AIHeroClient)));

                    Vector2 EditedVec = pred.UnitPosition.To2D() - Vector2.Normalize(pred.UnitPosition.To2D() - target.ServerPosition.To2D()) * (spell.Width * 2 / 5);
                    Vector2 EditedVec2 = (pred.UnitPosition.To2D() + target.ServerPosition.To2D()) / 2;

                    var collision2 = spell.GetCollision(ObjectManager.Player.ServerPosition.To2D(), new List<SharpDX.Vector2> { EditedVec });
                    var minioncol2 = collision2.Count(x => (HeroOnly == false ? x.IsMinion : (x is AIHeroClient)));
                    var collision3 = spell.GetCollision(ObjectManager.Player.ServerPosition.To2D(), new List<SharpDX.Vector2> { EditedVec2 });
                    var minioncol3 = collision3.Count(x => (HeroOnly == false ? x.IsMinion : (x is AIHeroClient)));

                    if (pred.Hitchance >= HitChance.VeryHigh)
                    {
                        if (target.IsValidTarget(spell.Range - target.MoveSpeed * (spell.Delay + ObjectManager.Player.Distance(target.ServerPosition) / spell.Speed) + alpha) && minioncol2 <= colmini && pred.UnitPosition.Distance(target.ServerPosition) > spell.Width)
                        {
                            spell.Cast(EditedVec);
                        }
                        else if (target.IsValidTarget(spell.Range - target.MoveSpeed * (spell.Delay + ObjectManager.Player.Distance(target.ServerPosition) / spell.Speed) + alpha) && minioncol3 <= colmini && pred.UnitPosition.Distance(target.ServerPosition) > spell.Width / 2)
                        {
                            spell.Cast(EditedVec2);
                        }
                        else if (target.IsValidTarget(spell.Range - target.MoveSpeed * (spell.Delay + ObjectManager.Player.Distance(target.ServerPosition) / spell.Speed) + alpha) && minioncol <= colmini)
                        {
                            spell.Cast(pred.CastPosition);
                        }
                        else if (false == spell.Collision && colmini < 1 && minioncol >= 1)
                        {
                            var FirstMinion = collision.OrderBy(o => o.Distance(ObjectManager.Player.ServerPosition)).FirstOrDefault();

                            if (FirstMinion.ServerPosition.Distance(pred.UnitPosition) <= BombRadius / 4)
                            {
                                spell.Cast(pred.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        public static void CastCone(this Spell spell, Obj_AI_Base target, float alpha = 0f, float colmini = float.MaxValue, bool HeroOnly = false)
        {
            if (spell.Type == SkillshotType.SkillshotCone)
            {
                if (spell != null && target != null)
                {
                    var pred = Prediction.GetPrediction(target, spell.Delay, spell.Width / 2, spell.Speed);
                    var collision = spell.GetCollision(ObjectManager.Player.ServerPosition.To2D(), new List<Vector2> { pred.CastPosition.To2D() });
                    var minioncol = collision.Count(x => (HeroOnly == false ? x.IsMinion : (x is AIHeroClient)));

                    if (target.IsValidTarget(spell.Range - target.MoveSpeed * (spell.Delay + ObjectManager.Player.Distance(target.ServerPosition) / spell.Speed) + alpha) && minioncol <= colmini && pred.Hitchance >= HitChance.VeryHigh)
                    {
                        spell.Cast(pred.CastPosition);
                    }
                }
            }
        }

        public static void CastAOE(this Spell spell, Obj_AI_Base target)
        {
            if (spell != null && target != null)
            {
                var pred = Prediction.GetPrediction(target, spell.Delay > 0 ? spell.Delay : 0.25f, spell.Range);

                if (pred.Hitchance >= HitChance.High && pred.UnitPosition.Distance(ObjectManager.Player.ServerPosition) <= spell.Range)
                {
                    spell.Cast();
                }
            }
        }

        internal static void RL244AtoBCast(this Spell spell, Obj_AI_Base T, float Drag = 700f)
        {
            if (T != null)
            {
                var TH = T as AIHeroClient;
                var TM = T as Obj_AI_Minion;

                if (TH != null)
                {
                    var TH2 = HeroManager.Enemies.Where(x => x != TH && CanHit(spell, x, Drag)).FirstOrDefault();
                    var THdelay = (ObjectManager.Player.Distance(TH.ServerPosition) > spell.Range ? (ObjectManager.Player.Distance(TH.ServerPosition) - spell.Range) / spell.Speed : 100f / spell.Speed);
                    var pred = Prediction.GetPrediction(TH, THdelay + spell.Delay);
                    var TH2delay = (TH2 != null ? (ObjectManager.Player.Distance(TH.ServerPosition) > spell.Range ? (ObjectManager.Player.Distance(TH2.ServerPosition) - spell.Range) / spell.Speed : TH2.ServerPosition.Distance(TH.ServerPosition) / spell.Speed) : 0f);
                    var TH2pred = (TH2 != null ? Prediction.GetPrediction(TH2, TH2delay + spell.Delay) : null);

                    Vector2 castVec = (pred.UnitPosition.To2D() + TH.ServerPosition.To2D()) / 2;
                    Vector2 castVec2 = ObjectManager.Player.ServerPosition.To2D() + Vector2.Normalize(pred.UnitPosition.To2D() - ObjectManager.Player.Position.To2D()) * (spell.Range);
                    Vector2 castVec3 = TH.ServerPosition.To2D() - Vector2.Normalize(pred.UnitPosition.To2D() - ObjectManager.Player.Position.To2D()) * (100f);
                    Vector2 EditedVec = pred.UnitPosition.To2D() - Vector2.Normalize(pred.UnitPosition.To2D() - TH.ServerPosition.To2D()) * (spell.Width * 2 / 5);
                    Vector2 EditedCV2Vec = ObjectManager.Player.ServerPosition.To2D() + Vector2.Normalize(EditedVec - ObjectManager.Player.Position.To2D()) * (spell.Range);

                    if (pred.Hitchance >= HitChance.VeryHigh)
                    {
                        if (TH.Distance(ObjectManager.Player.ServerPosition) >= spell.Range)
                        {
                            if (CanHit(spell, TH, Drag) && (pred.UnitPosition.Distance(TH.ServerPosition) <= spell.Width / 2 || TH.MoveSpeed * THdelay <= spell.Width / 2))
                            {
                                spell.Cast(EditedCV2Vec, TH.ServerPosition.To2D());
                            }
                            else if (CanHit(spell, TH, Drag) && pred.UnitPosition.Distance(TH.ServerPosition) < 350)
                            {
                                if (pred.UnitPosition.Distance(ObjectManager.Player.ServerPosition) > spell.Range)
                                {
                                    spell.Cast(castVec2, EditedVec);
                                }
                            }
                        }
                        else
                        {
                            if (TH2 == null || !CanHit(spell, TH2, Drag))
                            {
                                if (castVec3.Distance(ObjectManager.Player.ServerPosition) < TH.ServerPosition.Distance(ObjectManager.Player.ServerPosition))
                                {
                                    spell.Cast(castVec3, TH.ServerPosition.To2D());
                                }
                                else
                                {
                                    spell.Cast(TH.ServerPosition.To2D(), castVec3);
                                }
                            }
                            else if (TH2 != null && CanHit(spell, TH2, Drag) && TH2pred.Hitchance >= HitChance.VeryHigh)
                            {
                                Vector2 castVec4 = TH.ServerPosition.To2D() - Vector2.Normalize(TH2pred.UnitPosition.To2D() - TH.ServerPosition.To2D()) * (80f);

                                if (castVec4.Distance(ObjectManager.Player.ServerPosition) < TH2pred.UnitPosition.Distance(ObjectManager.Player.ServerPosition))
                                {
                                    spell.Cast(castVec4, TH2pred.UnitPosition.To2D());
                                }
                                else
                                {
                                    spell.Cast(TH2pred.UnitPosition.To2D(), castVec4);
                                }
                            }
                        }
                    }
                }

                if (TM != null)
                {
                    var Minions = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsValidTarget(spell.Range + Drag) && m.Team != ObjectManager.Player.Team).Cast<Obj_AI_Base>().ToList();

                    if (Minions.Count > 0)
                    {
                        var FM = Minions.OrderBy(o => o.Distance(ObjectManager.Player.ServerPosition)).FirstOrDefault().ServerPosition;
                        var FFM = Minions.OrderBy(o => o.Distance(ObjectManager.Player.ServerPosition)).Reverse().FirstOrDefault().ServerPosition;
                        var P = MinionManager.GetMinionsPredictedPositions(Minions, spell.Delay, spell.Width, spell.Speed, FM, Drag, true, SkillshotType.SkillshotLine);
                        var PP = MinionManager.GetBestLineFarmLocation(P, spell.Width, spell.Range + Drag);

                        if (FM != null && FM.Distance(ObjectManager.Player.ServerPosition) <= spell.Range)
                        {
                            spell.Cast(FM.To2D(), FFM.To2D());
                        }
                    }
                }
            }

            if (Program.Orbwalker.ActiveMode == Flowers_Nidalee.Orbwalking.OrbwalkingMode.LaneClear)
            {
                var Minions = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsValidTarget(spell.Range + Drag) && m.Team != ObjectManager.Player.Team).Cast<Obj_AI_Base>().ToList();

                if (Minions.Count > 0)
                {
                    var FM = Minions.OrderBy(o => o.Distance(ObjectManager.Player.ServerPosition)).FirstOrDefault().ServerPosition;
                    var FFM = Minions.OrderBy(o => o.Distance(ObjectManager.Player.ServerPosition)).Reverse().FirstOrDefault().ServerPosition;
                    var P = MinionManager.GetMinionsPredictedPositions(Minions, spell.Delay, spell.Width, spell.Speed, FM, Drag, true, SkillshotType.SkillshotLine);
                    var PP = MinionManager.GetBestLineFarmLocation(P, spell.Width, spell.Range + Drag);

                    if (FM != null && FM.Distance(ObjectManager.Player.ServerPosition) <= spell.Range)
                    {
                        spell.Cast(FM.To2D(), FFM.To2D());
                    }
                }
            }
        }

        public static void RMouse(this Spell spell)
        {
            Vector2 ReverseVec = ObjectManager.Player.ServerPosition.To2D() - Vector2.Normalize(Game.CursorPos.To2D() - ObjectManager.Player.Position.To2D()) * (spell.Range);

            if (spell.IsReady())
            {
                spell.Cast(ReverseVec);
            }
        }

        public static void NMouse(this Spell spell)
        {
            Vector2 NVec = ObjectManager.Player.ServerPosition.To2D() + Vector2.Normalize(Game.CursorPos.To2D() - ObjectManager.Player.Position.To2D()) * (spell.Range);

            if (spell.IsReady())
            {
                spell.Cast(NVec);
            }
        }

        public static void RTarget(this Spell spell, Obj_AI_Base Target)
        {
            Vector2 ReverseVec = ObjectManager.Player.ServerPosition.To2D() - Vector2.Normalize(Target.ServerPosition.To2D() - ObjectManager.Player.Position.To2D()) * (spell.Range);

            if (spell.IsReady())
            {
                spell.Cast(ReverseVec);
            }
        }

        public static void NTarget(this Spell spell, Obj_AI_Base Target)
        {
            Vector2 Vec = ObjectManager.Player.ServerPosition.To2D() + Vector2.Normalize(Target.ServerPosition.To2D() - ObjectManager.Player.Position.To2D()) * (spell.Range);

            if (spell.IsReady())
            {
                spell.Cast(Vec);
            }
        }

        public static bool CanHit(this Spell spell, Obj_AI_Base T, float Drag = 0f)
        {
            return T.IsValidTarget(spell.Range + Drag - ((T.Distance(ObjectManager.Player.ServerPosition) - spell.Range) / spell.Speed + spell.Delay) * T.MoveSpeed);
        }
    }
}
