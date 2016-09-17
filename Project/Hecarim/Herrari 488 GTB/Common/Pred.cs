using System;
using System.Collections.Generic;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Herrari_488_GTB
{
    static class Pred
    {
        internal static Menu Menu { get { return MenuProvider.MenuInstance.SubMenu("Champion"); } }
        static AIHeroClient Player { get { return ObjectManager.Player; } }
        static Orbwalking.Orbwalker Orbwalker { get { return MenuProvider.Orbwalker; } }

        internal static float PredHealth(Obj_AI_Base Target, Spell spell)
        {
            return HealthPrediction.GetHealthPrediction(Target, (int)(Player.Distance(Target, false) / spell.Speed), (int)(spell.Delay * 1000 + Game.Ping / 2));
        }

        internal static void CCast(this Spell spell, Obj_AI_Base target) //for Circular spells
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
                        else if (pred.Hitchance >= MenuProvider.Champion.Misc.SelectedHitchance && pred.UnitPosition.Distance(target.ServerPosition) < Math.Max(spell.Width, 300f))
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
                        if (pred.Hitchance >= MenuProvider.Champion.Misc.SelectedHitchance && Player.ServerPosition.Distance(pred.UnitPosition) <= spell.Range + spell.Width * 1 / 2 && pred.UnitPosition.Distance(target.ServerPosition) < Math.Max(spell.Width, 400f))
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

        internal static void LCast(this Spell spell, Obj_AI_Base target, float alpha = 0f, float colmini = float.MaxValue, bool HeroOnly = false, float BombRadius = 0f) //for Linar spells  사용예시 Func.LCast(Q,Qtarget,50,0)  
        {                            //        Func.LCast(E,Etarget,Menu.Item("Misc.Etg").GetValue<Slider>().Value,float.MaxValue); <- 이런식으로 사용.
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
                    if (pred.Hitchance >= MenuProvider.Champion.Misc.SelectedHitchance)
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

        internal static void ConeCast(this Spell spell, Obj_AI_Base target, float alpha = 0f, float colmini = float.MaxValue, bool HeroOnly = false)
        {
            if (spell.Type == SkillshotType.SkillshotCone)
            {
                if (spell != null && target != null)
                {
                    var pred = Prediction.GetPrediction(target, spell.Delay, spell.Width / 2, spell.Speed); //spell.Width/2
                    var collision = spell.GetCollision(Player.ServerPosition.To2D(), new List<SharpDX.Vector2> { pred.CastPosition.To2D() });
                    var minioncol = collision.Count(x => (HeroOnly == false ? x.IsMinion : (x is AIHeroClient)));
                    if (target.IsValidTarget(spell.Range - target.MoveSpeed * (spell.Delay + Player.Distance(target.ServerPosition) / spell.Speed) + alpha) && minioncol <= colmini && pred.Hitchance >= MenuProvider.Champion.Misc.SelectedHitchance)
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

        internal static void AtoB(this Spell spell, Obj_AI_Base T, float Drag = 700f) //Coded By RL244 AtoB Drag 기본값 700f는 빅토르를 위한 것임.
        {
            if (T != null)
            {
                var TH = T as AIHeroClient;
                var TM = T as Obj_AI_Minion;
                if (TH != null)
                {
                    var TH2 = HeroManager.Enemies.Where(x => x != TH && Func.CanHit(spell, x, Drag)).FirstOrDefault();
                    var THdelay = (Player.Distance(TH.ServerPosition) > spell.Range ? (Player.Distance(TH.ServerPosition) - spell.Range) / spell.Speed : 100f / spell.Speed);
                    var pred = Prediction.GetPrediction(TH, THdelay + spell.Delay);
                    var TH2delay = (TH2 != null ? (Player.Distance(TH.ServerPosition) > spell.Range ? (Player.Distance(TH2.ServerPosition) - spell.Range) / spell.Speed : TH2.ServerPosition.Distance(TH.ServerPosition) / spell.Speed) : 0f);
                    var TH2pred = (TH2 != null ? Prediction.GetPrediction(TH2, TH2delay + spell.Delay) : null);
                    SharpDX.Vector2 castVec = (pred.UnitPosition.To2D() + TH.ServerPosition.To2D()) / 2;
                    SharpDX.Vector2 castVec2 = Player.ServerPosition.To2D() +
                                               SharpDX.Vector2.Normalize(pred.UnitPosition.To2D() - Player.Position.To2D()) * (spell.Range);
                    SharpDX.Vector2 castVec3 = TH.ServerPosition.To2D() -
                                               SharpDX.Vector2.Normalize(pred.UnitPosition.To2D() - Player.Position.To2D()) * (100f);
                    SharpDX.Vector2 EditedVec = pred.UnitPosition.To2D() -
                                               SharpDX.Vector2.Normalize(pred.UnitPosition.To2D() - TH.ServerPosition.To2D()) * (spell.Width * 2 / 5);
                    SharpDX.Vector2 EditedCV2Vec = Player.ServerPosition.To2D() +
                                               SharpDX.Vector2.Normalize(EditedVec - Player.Position.To2D()) * (spell.Range);

                    if (pred.Hitchance >= MenuProvider.Champion.Misc.SelectedHitchance)
                    {
                        if (TH.Distance(Player.ServerPosition) >= spell.Range)
                        {
                            if (Func.CanHit(spell, TH, Drag) && (pred.UnitPosition.Distance(TH.ServerPosition) <= spell.Width / 2 || TH.MoveSpeed * THdelay <= spell.Width / 2))//if(Func.CanHit(spell,TH,Drag) && TH2 != null && TH2pred.Hitchance >= MenuProvider.Champion.Misc.SelectedHitchance)//별로 좋은 생각이 더 안나고 피곤해서 걍관둠.
                            {
                                spell.Cast(EditedCV2Vec, TH.ServerPosition.To2D());//별로 좋은 생각이 더 안나고 피곤해서 걍관둠.
                            }
                            else if (Func.CanHit(spell, TH, Drag) && pred.UnitPosition.Distance(TH.ServerPosition) < 350)
                            {
                                if (pred.UnitPosition.Distance(Player.ServerPosition) > spell.Range)
                                    spell.Cast(castVec2, EditedVec);//pred.UnitPosition.To2D());
                            }
                        }
                        else
                        {
                            if (TH2 == null || !Func.CanHit(spell, TH2, Drag))
                            {
                                if (castVec3.Distance(Player.ServerPosition) < TH.ServerPosition.Distance(Player.ServerPosition))
                                    spell.Cast(castVec3, TH.ServerPosition.To2D());
                                else
                                    spell.Cast(TH.ServerPosition.To2D(), castVec3);
                            }
                            else if (TH2 != null && Func.CanHit(spell, TH2, Drag) && TH2pred.Hitchance >= MenuProvider.Champion.Misc.SelectedHitchance)
                            {
                                SharpDX.Vector2 castVec4 = TH.ServerPosition.To2D() -
                                                           SharpDX.Vector2.Normalize(TH2pred.UnitPosition.To2D() - TH.ServerPosition.To2D()) * (80f);
                                if (castVec4.Distance(Player.ServerPosition) < TH2pred.UnitPosition.Distance(Player.ServerPosition))
                                    spell.Cast(castVec4, TH2pred.UnitPosition.To2D());
                                else
                                    spell.Cast(TH2pred.UnitPosition.To2D(), castVec4);
                            }
                        }
                    }
                }
                if (TM != null)
                {
                    var Minions = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsValidTarget(spell.Range + Drag) && m.Team != ObjectManager.Player.Team).Cast<Obj_AI_Base>().ToList();
                    if (Minions.Count > 0)
                    { //으....
                        //MinionManager.GetMinions(spell.Range+Drag, MinionTypes.All, MinionTeam.NotAlly);
                        var FM = Minions.OrderBy(o => o.Distance(Player.ServerPosition)).FirstOrDefault().ServerPosition;
                        var FFM = Minions.OrderBy(o => o.Distance(Player.ServerPosition)).Reverse().FirstOrDefault().ServerPosition;
                        var P = MinionManager.GetMinionsPredictedPositions(Minions, spell.Delay, spell.Width, spell.Speed, FM, Drag, true, SkillshotType.SkillshotLine);
                        var PP = MinionManager.GetBestLineFarmLocation(P, spell.Width, spell.Range + Drag);
                        if (FM != null && FM.Distance(Player.ServerPosition) <= spell.Range)// && PP.MinionsHit >= Math.Min(Minions.Count,6))
                                                                                            //spell.Cast(FM.To2D(),PP.Position);
                            spell.Cast(FM.To2D(), FFM.To2D());
                    }
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var Minions = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsValidTarget(spell.Range + Drag) && m.Team != ObjectManager.Player.Team).Cast<Obj_AI_Base>().ToList();
                if (Minions.Count > 0)
                {
                    //MinionManager.GetMinions(spell.Range+Drag, MinionTypes.All, MinionTeam.NotAlly);
                    var FM = Minions.OrderBy(o => o.Distance(Player.ServerPosition)).FirstOrDefault().ServerPosition;
                    var FFM = Minions.OrderBy(o => o.Distance(Player.ServerPosition)).Reverse().FirstOrDefault().ServerPosition;
                    var P = MinionManager.GetMinionsPredictedPositions(Minions, spell.Delay, spell.Width, spell.Speed, FM, Drag, true, SkillshotType.SkillshotLine);
                    var PP = MinionManager.GetBestLineFarmLocation(P, spell.Width, spell.Range + Drag);
                    if (FM != null && FM.Distance(Player.ServerPosition) <= spell.Range)// && PP.MinionsHit >= Math.Min(Minions.Count,6))
                        //spell.Cast(FM.To2D(),PP.Position);
                        spell.Cast(FM.To2D(), FFM.To2D());
                }
            }
        }

        internal static void RMouse(this Spell spell)
        {
            SharpDX.Vector2 ReverseVec = Player.ServerPosition.To2D() -
                                       SharpDX.Vector2.Normalize(Game.CursorPos.To2D() - Player.Position.To2D()) * (spell.Range);
            if (spell.IsReady())
                spell.Cast(ReverseVec);
        }

        internal static void NMouse(this Spell spell)
        {
            SharpDX.Vector2 NVec = Player.ServerPosition.To2D() +
                                       SharpDX.Vector2.Normalize(Game.CursorPos.To2D() - Player.Position.To2D()) * (spell.Range);
            if (spell.IsReady())
                spell.Cast(NVec);
        }

        internal static void RTarget(this Spell spell, Obj_AI_Base Target)
        {
            SharpDX.Vector2 ReverseVec = Player.ServerPosition.To2D() -
                                       SharpDX.Vector2.Normalize(Target.ServerPosition.To2D() - Player.Position.To2D()) * (spell.Range);
            if (spell.IsReady())
                spell.Cast(ReverseVec);
        }

        internal static void NTarget(this Spell spell, Obj_AI_Base Target)
        {
            SharpDX.Vector2 Vec = Player.ServerPosition.To2D() +
                                       SharpDX.Vector2.Normalize(Target.ServerPosition.To2D() - Player.Position.To2D()) * (spell.Range);
            if (spell.IsReady())
                spell.Cast(Vec);
        }

        internal static void FleeToPosition(this Spell spell, string W = "N") // N 정방향, R 역방향.
        {
            bool FM = true;
            if (Menu.Item("Flee.If Mana >" + spell.Slot.ToString(), true) != null)
            {
                FM = Player.ManaPercent > MenuProvider.Champion.Flee.IfMana;
            }
            SharpDX.Vector2 NormalVec = Player.ServerPosition.To2D() +
                                       SharpDX.Vector2.Normalize(Game.CursorPos.To2D() - Player.Position.To2D()) * (spell.Range);
            if (Menu.Item("Flee.Use " + spell.Slot.ToString(), true) != null && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Flee)
            {
                if (Menu.Item("Flee.Use " + spell.Slot.ToString(), true).GetValue<bool>() && spell.IsReady())
                {
                    if (W == "N")
                        spell.Cast(NormalVec);
                    else
                        RMouse(spell);
                }
            }
        }

    }
}
