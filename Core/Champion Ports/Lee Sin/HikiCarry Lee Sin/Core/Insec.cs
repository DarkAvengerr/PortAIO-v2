using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using HikiCarry_Lee_Sin.Core;
using HikiCarry_Lee_Sin.Plugins;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Geometry = HikiCarry_Lee_Sin.Plugins.Geometry;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Lee_Sin.Core
{
    class Insec
    {
        /// <summary>
        /// returns Slider Value
        /// </summary>
        /// <param name="menuName">"slider" menu name for value</param>
        /// <returns></returns>
        public static int SliderCheck(string menuName)
        {
            return Program.Config.Item(menuName).GetValue<Slider>().Value;
        }
        /// <summary>
        /// returns insec to ally position 
        /// </summary>
        /// <param name="distance">ward place distance for insec</param>
        /// <param name="enemy">insec enemy</param>
        /// <returns></returns>
        public static Vector3 AllyInsec(float distance, AIHeroClient enemy)
        {
            var ally = ObjectManager.Get<AIHeroClient>().Where(x => !x.IsMe && !x.IsDead && x.IsHPBarRendered && x.IsAlly).OrderBy(o => o.Distance(ObjectManager.Player.Position)).FirstOrDefault();
            return ally != null ? enemy.Position.To2D().Extend(ally.Position.To2D(), -distance).To3D() : new Vector3(0,0,0);
        }
        /// <summary>
        /// returns insec to cursor position
        /// </summary>
        /// <param name="distance">ward place distance for insec</param>
        /// <param name="enemy">insec enemy</param>
        /// <returns></returns>
        public static Vector3 CursorInsec(float distance, AIHeroClient enemy)
        {
            return enemy != null ? enemy.Position.To2D().Extend(ObjectManager.Player.Position.To2D(), -distance).To3D() : new Vector3(0, 0, 0);
        }
        /// <summary>
        /// returns insec to tower position
        /// </summary>
        /// <param name="distance">ward place distance for insec</param>
        /// <param name="enemy">insec enemy</param>
        /// <returns></returns>
        public static Vector3 TowerInsec(int distance, AIHeroClient enemy)
        {
            var tower = ObjectManager.Get<Obj_AI_Turret>().Where(x => x.IsAlly && !x.IsEnemy).OrderBy(o => o.Distance(ObjectManager.Player.Position)).FirstOrDefault();
            return tower != null ? enemy.Position.To2D().Extend(tower.Position.To2D(), -distance).To3D() : new Vector3(0, 0, 0);
        }
        public static void ClickInsec()
        {
            if (Program.Config.Item("obj.usage").GetValue<bool>())
            {
                if (Program.Config.Item("flash.usage").GetValue<bool>())
                {
                    AIHeroClient selectedTarget = TargetSelector.GetSelectedTarget();
                    var wardjumpflashrange = Spells.W.Range + 424;

                    var insecpos = InsecPositions(TargetSelector.GetSelectedTarget());

                    var insecposex = InsecPositions(TargetSelector.GetSelectedTarget())
                        .Extend(ObjectManager.Player.Position, wardjumpflashrange);


                    var obj = ObjectManager.Get<Obj_AI_Base>().Where(x => x.IsEnemy && x.IsValidTarget(Spells.Q.Range) && x.Distance(insecpos) < wardjumpflashrange && x.Distance(insecpos) > 20 && Spells.Q.GetPrediction(x).Hitchance >=
                        HitChance.High && !x.Name.ToLower().Contains("turret") && x.Name != TargetSelector.GetSelectedTarget().Name)
                        .OrderBy(o => o.Distance(insecposex)).FirstOrDefault(x => x.Health > Spells.Q.GetDamage(x));

                    if (obj != null)
                    {
                        if (Spells.Q.CanCast(obj))
                        {
                            Spells.Q.Cast(obj);
                        }
                        if (Spells.Q.Instance.Name != "BlindMonkQOne" && Spells.Q2.IsReady())
                        {
                            Spells.Q2.Cast();
                        }
                        if (!Spells.Q.IsReady() && !Spells.Q2.IsReady() && Spells.W.IsReady() && Spells.W.Instance.Name == "BlindMonkWOne")
                        {
                            WardJump.HikiJump(obj.Position.Extend(selectedTarget.Position, 424));
                        }
                        if (!Spells.Q.IsReady() && !Spells.Q2.IsReady() && Spells.W.Instance.Name != "BlindMonkWOne" && Spells.R.CanCast(selectedTarget))
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add(30, () => ObjectManager.Player.Spellbook.CastSpell(ObjectManager.Player.GetSpellSlot("SummonerFlash"), insecpos));
                        }
                        if (!Spells.Q.IsReady() && !Spells.Q2.IsReady() && Spells.W.Instance.Name != "BlindMonkWOne" && Spells.R.CanCast(selectedTarget))
                        {
                            Spells.R.CastOnUnit(selectedTarget);
                        }
                    }
                    else
                    {
                        AIHeroClient selectedTargetx = TargetSelector.GetSelectedTarget();

                        if (selectedTargetx != null && Spells.Q.GetPrediction(selectedTargetx).Hitchance >= HitChance.High)
                        {
                            if (Spells.Q.CanCast(selectedTargetx) && Spells.Q.GetPrediction(selectedTargetx).Hitchance > HitChance.High)
                            {
                                Spells.Q.Cast(selectedTargetx);
                            }
                            if (Spells.Q.Instance.Name != "BlindMonkQOne" && Spells.Q2.IsReady())
                            {
                                Spells.Q2.Cast();
                            }
                            if (!Spells.Q.IsReady() && !Spells.Q2.IsReady() && Spells.W.IsReady() && Spells.W.Instance.Name == "BlindMonkWOne")
                            {
                                InsecTo(selectedTargetx);
                            }
                            if (!Spells.Q.IsReady() && !Spells.Q2.IsReady() && Spells.W.Instance.Name != "BlindMonkWOne" && Spells.R.CanCast(selectedTargetx))
                            {
                                Spells.R.CastOnUnit(selectedTargetx);
                            }
                        }
                    }
                }
                else
                {
                    AIHeroClient selectedTarget = TargetSelector.GetSelectedTarget();

                    var insecpos = InsecPositions(TargetSelector.GetSelectedTarget());


                    var obj = ObjectManager.Get<Obj_AI_Base>().Where(x => x.IsEnemy && x.IsValidTarget(Spells.Q.Range) && x.Distance(insecpos) < Spells.W.Range && x.Distance(insecpos) > 20 && Spells.Q.GetPrediction(x).Hitchance >=
                        HitChance.High && x.Name != TargetSelector.GetSelectedTarget().Name)
                        .OrderBy(o => o.Distance(insecpos)).FirstOrDefault(x => x.Health > Spells.Q.GetDamage(x));

                    if (obj != null)
                    {
                        if (Spells.Q.CanCast(obj))
                        {
                            Spells.Q.Cast(obj);
                        }
                        if (Spells.Q.Instance.Name != "BlindMonkQOne" && Spells.Q2.IsReady())
                        {
                            Spells.Q2.Cast();
                        }
                        if (!Spells.Q.IsReady() && !Spells.Q2.IsReady() && Spells.W.IsReady() && Spells.W.Instance.Name == "BlindMonkWOne")
                        {
                            WardJump.HikiJump(insecpos);
                        }
                        if (!Spells.Q.IsReady() && !Spells.Q2.IsReady() && Spells.W.Instance.Name != "BlindMonkWOne" && Spells.R.CanCast(selectedTarget))
                        {
                            Spells.R.CastOnUnit(selectedTarget);
                        }
                    }
                    else
                    {
                        AIHeroClient selectedTargetx = TargetSelector.GetSelectedTarget();

                        if (selectedTargetx != null && Spells.Q.GetPrediction(selectedTargetx).Hitchance >= HitChance.High)
                        {
                            if (Spells.Q.CanCast(selectedTargetx) && Spells.Q.GetPrediction(selectedTargetx).Hitchance > HitChance.High)
                            {
                                Spells.Q.Cast(selectedTargetx);
                            }
                            if (Spells.Q.Instance.Name != "BlindMonkQOne" && Spells.Q2.IsReady())
                            {
                                Spells.Q2.Cast();
                            }
                            if (!Spells.Q.IsReady() && !Spells.Q2.IsReady() && Spells.W.IsReady() && Spells.W.Instance.Name == "BlindMonkWOne")
                            {
                                InsecTo(selectedTargetx);
                            }
                            if (!Spells.Q.IsReady() && !Spells.Q2.IsReady() && Spells.W.Instance.Name != "BlindMonkWOne" && Spells.R.CanCast(selectedTargetx))
                            {
                                Spells.R.CastOnUnit(selectedTargetx);
                            }
                        }
                    }
                }
            }
            else
            {
                AIHeroClient selectedTarget = TargetSelector.GetSelectedTarget();
                var insecpos = InsecPositions(TargetSelector.GetSelectedTarget());

                var obj = ObjectManager.Get<Obj_AI_Base>().Where(x => x.IsEnemy && x.IsValidTarget(Spells.Q.Range) && x.Distance(insecpos) < 400 && x.Distance(insecpos) > 20 && Spells.Q.GetPrediction(x).Hitchance >=
                   HitChance.High && x.Name != TargetSelector.GetSelectedTarget().Name)
                   .OrderBy(o => o.Distance(insecpos)).FirstOrDefault();

                if (selectedTarget != null && Spells.Q.GetPrediction(selectedTarget).Hitchance >= HitChance.High)
                {
                    if (Spells.Q.CanCast(selectedTarget) && Spells.Q.GetPrediction(selectedTarget).Hitchance > HitChance.High)
                    {
                        Spells.Q.Cast(selectedTarget);
                    }
                    if (Spells.Q.Instance.Name != "BlindMonkQOne" && Spells.Q2.IsReady())
                    {
                        Spells.Q2.Cast();
                    }
                    if (!Spells.Q.IsReady() && !Spells.Q2.IsReady() && Spells.W.IsReady() && Spells.W.Instance.Name == "BlindMonkWOne")
                    {
                        InsecTo(selectedTarget);
                    }
                    if (!Spells.Q.IsReady() && !Spells.Q2.IsReady() && Spells.W.Instance.Name != "BlindMonkWOne" && Spells.R.CanCast(selectedTarget))
                    {
                        Spells.R.CastOnUnit(selectedTarget);
                    }
                }
            }
            
            
        }
        /// <summary>
        /// returns current insec positions
        /// </summary>
        /// <param name="enemy">insec enemy</param>
        public static void InsecTo(AIHeroClient enemy)
        {
            switch (Program.Config.Item("insec.to").GetValue<StringList>().SelectedIndex)
            {
                case 0:

                    if (ObjectManager.Player.Distance(AllyInsec(SliderCheck("insec.distance"), enemy)) < Spells.W.Range && AllyInsec(SliderCheck("insec.distance"), enemy) != new Vector3(0,0,0))
                    {
                        WardJump.HikiJump(AllyInsec(SliderCheck("insec.distance"), enemy));
                    }
                    break;
                case 1:
                    if (ObjectManager.Player.Distance(TowerInsec(SliderCheck("insec.distance"), enemy)) < Spells.W.Range && TowerInsec(SliderCheck("insec.distance"), enemy) != new Vector3(0,0,0))
                    {
                        WardJump.HikiJump(TowerInsec(SliderCheck("insec.distance"), enemy));
                    }
                    break;
                case 2:
                    if (ObjectManager.Player.Distance(CursorInsec(SliderCheck("insec.distance"), enemy)) < Spells.W.Range && CursorInsec(SliderCheck("insec.distance"), enemy) != new Vector3(0,0,0))
                    {
                        WardJump.HikiJump(CursorInsec(SliderCheck("insec.distance"),enemy));
                    }
                    break;
            }
        }
        /// <summary>
        /// returns current insec positions 
        /// </summary>
        /// <param name="enemy">insec enemy</param>
        /// <returns></returns>
        public static Vector3 InsecPositions(AIHeroClient enemy)
        {
            switch (Program.Config.Item("insec.to").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return AllyInsec(SliderCheck("insec.distance"), enemy);
                case 1:
                    return TowerInsec(SliderCheck("insec.distance"), enemy);
                case 2:
                    return CursorInsec(SliderCheck("insec.distance"), enemy);
            }
            return new Vector3(0,0,0);
        }
        /// <summary>
        /// Object usage debug (cirlces)
        /// </summary>
        public static void ObjectUsageDebug()
        {
            if (TargetSelector.GetSelectedTarget() == null)
            {
                return;
            }

            AIHeroClient selectedTarget = TargetSelector.GetSelectedTarget();
            var insecpos = InsecPositions(TargetSelector.GetSelectedTarget());
            var obj = ObjectManager.Get<Obj_AI_Base>().Where(x => x.IsEnemy && x.IsValidTarget(Spells.Q.Range) && x.Distance(insecpos) < 400 && x.Distance(insecpos) > 20 && Spells.Q.GetPrediction(x).Hitchance >= 
                HitChance.High && x.Name != TargetSelector.GetSelectedTarget().Name)
                .OrderBy(o => o.Distance(insecpos)).FirstOrDefault();

            if (selectedTarget != null && obj != null && (obj.Team == GameObjectTeam.Neutral || obj.IsMinion || obj.IsChampion())
                && (Spells.Q.GetPrediction(selectedTarget).Hitchance < HitChance.High || Spells.Q.GetPrediction(selectedTarget).Hitchance == HitChance.Collision || 
                Spells.Q.GetPrediction(selectedTarget).Hitchance == HitChance.Impossible))
            {
               Render.Circle.DrawCircle(obj.Position,100,Color.Gold);
            }
        }
        /// <summary>
        /// ward jump flash insec
        /// </summary>
        public static void WardJumpFlashInsec()
        {
            AIHeroClient selectedTarget = TargetSelector.GetSelectedTarget();
            var wardjumpflashrange = Spells.W.Range + 424;
            
            var insecpos = InsecPositions(TargetSelector.GetSelectedTarget());
            
            var insecposex = InsecPositions(TargetSelector.GetSelectedTarget())
                .Extend(ObjectManager.Player.Position, wardjumpflashrange);


            var obj = ObjectManager.Get<Obj_AI_Base>().Where(x => x.IsEnemy && x.IsValidTarget(Spells.Q.Range) && x.Distance(insecpos) < wardjumpflashrange && x.Distance(insecpos) > 20 && Spells.Q.GetPrediction(x).Hitchance >=
                HitChance.High && x.Name != TargetSelector.GetSelectedTarget().Name)
                .OrderBy(o => o.Distance(insecposex)).FirstOrDefault(x=> x.Health > Spells.Q.GetDamage(x));

            if (obj != null)
            {
                if (Spells.Q.CanCast(obj))
                {
                    Spells.Q.Cast(obj);
                }
                if (Spells.Q.Instance.Name != "BlindMonkQOne" && Spells.Q2.IsReady())
                {
                    Spells.Q2.Cast();
                }
                if (!Spells.Q.IsReady() && !Spells.Q2.IsReady() && Spells.W.IsReady() && Spells.W.Instance.Name == "BlindMonkWOne")
                {
                    WardJump.HikiJump(obj.Position.Extend(selectedTarget.Position, 424));
                }
                if (!Spells.Q.IsReady() && !Spells.Q2.IsReady() && Spells.W.Instance.Name != "BlindMonkWOne" && Spells.R.CanCast(selectedTarget))
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(30, () => ObjectManager.Player.Spellbook.CastSpell(ObjectManager.Player.GetSpellSlot("SummonerFlash"), insecpos));
                }
                if (!Spells.Q.IsReady() && !Spells.Q2.IsReady() && Spells.W.Instance.Name != "BlindMonkWOne" && Spells.R.CanCast(selectedTarget))
                {
                    Spells.R.CastOnUnit(selectedTarget);
                }
            }
        }
        /// <summary>
        /// insec debug (rectangle alert)
        /// </summary>
        public static void InsecDebug()
        {
            if (TargetSelector.GetSelectedTarget().IsValidTarget(Spells.Q2.Range))
            {
                var xx = new Geometry.Rectangle(ObjectManager.Player.Position.To2D(), TargetSelector.GetSelectedTarget().Position.To2D(), 50);
                xx.ToPolygon().Draw(Color.Gold, 3);
                var xy = new Geometry.Rectangle(TargetSelector.GetSelectedTarget().Position.To2D(), Insec.InsecPositions(TargetSelector.GetSelectedTarget()).To2D(), 50);
                xy.ToPolygon().Draw(Color.Azure, 3);
                Render.Circle.DrawCircle(InsecPositions(TargetSelector.GetSelectedTarget()), 50, Color.Gold, 3);
            }
        }

    }
}
