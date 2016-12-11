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
    internal class Insec
    {
        private static AIHeroClient insecAlly;
        private static AIHeroClient insecTarget;
        private static Obj_AI_Turret insecTurret;
        private static Vector3 insecPos;
        private static Obj_AI_Base qHit;

        internal static void Load()
        {
            Game.OnWndProc += Game_OnWndProc;
            Game.OnUpdate += Update;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnBuffGain += OnBuffAdd;
        }

        private static void OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs Args)
        {
            if (sender.IsEnemy && sender.IsValid && Args.Buff.Name == "BlindMonkQOne")
                qHit = sender;
        }

        private static void OnDraw(EventArgs args)
        {
            if(insecTarget != null && insecTarget.IsValidTarget(50000))
            {
                Vector3 endPos = new Vector3();
                if (insecAlly != default(AIHeroClient))
                    endPos = insecAlly.Position;
                else if (insecTurret != default(Obj_AI_Turret))
                    endPos = insecTurret.Position;
                else
                    endPos = insecPos;

                if (endPos != default(Vector3))
                {
                    Vector2 posA = Drawing.WorldToScreen(insecTarget.Position);
                    Vector2 posB = Drawing.WorldToScreen(endPos);                                     
                    Drawing.DrawLine(posA, posB, 5, Color.LightGreen);
                    Render.Circle.DrawCircle(insecTarget.Position, 70, Color.LightGreen);
                    Render.Circle.DrawCircle(endPos, 70, Color.LightGreen);

                    if (MenuManager.myMenu.Item("insec.PredictPos").GetValue<Boolean>())
                    {
                        Vector3 predictPos = insecTarget.Position + (endPos - insecTarget.Position).Normalized() * 1100;
                        Vector2 textPredictPos = Drawing.WorldToScreen(predictPos);
                        Render.Circle.DrawCircle(predictPos, 70, Color.LawnGreen);
                        Drawing.DrawText(textPredictPos.X, textPredictPos.Y, Color.LimeGreen, "Predict Pos");
                    }
                    if (MenuManager.myMenu.Item("insec.WardPos").GetValue<Boolean>())
                    {
                        Vector3 WardPos = insecTarget.Position + (insecTarget.Position - endPos).Normalized() * 250;
                        Vector2 textWardPos = Drawing.WorldToScreen(WardPos);
                        Render.Circle.DrawCircle(WardPos, 70, Color.LawnGreen);
                        Drawing.DrawText(textWardPos.X, textWardPos.Y, Color.LimeGreen, "Ward Pos");
                    }
                }
            }
        }

        private static void Update(EventArgs args)
        {
            if(!LeeSin.R.IsReady())
            {
                insecAlly = default(AIHeroClient);
                insecTarget = default(AIHeroClient);
                insecTurret = default(Obj_AI_Turret);
                insecPos = default(Vector3);
            }

            if (MenuManager.myMenu.Item("insec.key").GetValue<KeyBind>().Active)
                startInsec();
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowsMessages.WM_LBUTTONDOWN || !LeeSin.R.IsReady())
            {
                return;
            }

            var target = HeroManager.Enemies.Where(unit => (unit.Distance(Game.CursorPos) < 150) && unit.IsValidTarget(50000));
            if (target.Count() > 0)
            {
                insecTarget = target.First();
                return;
            }

            var ally = HeroManager.Allies.Where(unit => (unit.Distance(Game.CursorPos) < 150) && unit.IsValid);
            if(ally.Count() > 0)
            {
                insecAlly = ally.First();
                insecTurret = default(Obj_AI_Turret);
                insecPos = default(Vector3);
                return;
            }

            var turret = Game.CursorPos.GetObjects<Obj_AI_Turret>(150);
            if(turret.Count() > 0)
            {
                insecTurret = turret.First();
                insecAlly = default(AIHeroClient);
                insecPos = default(Vector3);
                return;
            }

            insecPos = Game.CursorPos;
            insecAlly = default(AIHeroClient);
            insecTurret = default(Obj_AI_Turret);
        }

        private static void startInsec()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (insecTarget == null || !insecTarget.IsValidTarget() || !LeeSin.R.IsReady())
                return;

            Vector3 endPos = new Vector3();
            if (insecAlly != default(AIHeroClient))
                endPos = insecAlly.Position;
            else if (insecTurret != default(Obj_AI_Turret))
                endPos = insecTurret.Position;
            else
                endPos = insecPos;

            if (endPos == default(Vector3))
                return;

            Vector3 WardPos = insecTarget.Position + (insecTarget.Position - endPos).Normalized() * 300;

            if (LeeSin.myHero.Distance(WardPos) < 100)
            {
                LeeSin.R.Cast(insecTarget);
                return;
            }

            Spell wardSpell = LeeSin.FindWard();

            if (LeeSin.Q.IsReady() && !LeeSin.IsSecondCast(LeeSin.Q) && LeeSin.W.IsReady() && !LeeSin.IsSecondCast(LeeSin.W) && wardSpell != null)
            {
                int HitChance = 0;
                Vector3 pos = new Vector3();

                if (MenuManager.myMenu.Item("pred.list2").GetValue<StringList>().SelectedIndex == 0)
                {
                    PredictionOutput pred = LeeSin.Q.GetPrediction(insecTarget);
                    HitChance = (int)pred.Hitchance;
                    pos = pred.CastPosition;
                }
                else
                {
                    resultPred pred = myPred.GetPrediction(insecTarget, LeeSin.Q);
                    HitChance = pred.Hitchance;
                    pos = pred.predPos;
                }

                if (HitChance >= MenuManager.myMenu.Item("insec.qHitChance").GetValue<Slider>().Value)
                {
                    LeeSin.Q.Cast(pos);
                    return;
                }
                else
                {
                    var enemyMinion = MinionManager.GetMinions(LeeSin.Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.None);
                    var isMinion = enemyMinion.Where(unit => unit.Position.Distance(WardPos) < 500 && unit.Health > LeeSin.GetDamage_Q1(unit));
                    if(isMinion.Count() > 0)
                    {
                        PredictionOutput qPred = LeeSin.Q.GetPrediction(isMinion.First());
                        if ((int)qPred.Hitchance >= MenuManager.myMenu.Item("combo.qHitChance").GetValue<Slider>().Value)
                            LeeSin.Q.Cast(qPred.CastPosition);
                    }

                    var secondUnit = HeroManager.Enemies.Where(unit => unit.Position.Distance(WardPos) < 500);
                    if (secondUnit.Count() > 0)
                    {
                        if (MenuManager.myMenu.Item("pred.list2").GetValue<StringList>().SelectedIndex == 0)
                        {
                            PredictionOutput qPred = LeeSin.Q.GetPrediction(secondUnit.First());
                            if ((int)qPred.Hitchance >= MenuManager.myMenu.Item("combo.qHitChance").GetValue<Slider>().Value)
                                LeeSin.Q.Cast(qPred.CastPosition);
                        }
                        else
                        {
                            resultPred qPred = myPred.GetPrediction(secondUnit.First(), LeeSin.Q);
                            if (qPred.Hitchance >= MenuManager.myMenu.Item("combo.qHitChance").GetValue<Slider>().Value)
                                LeeSin.Q.Cast(qPred.predPos);
                        }
                    }
                }
            }

            if (LeeSin.Q.IsReady() && LeeSin.IsSecondCast(LeeSin.Q) && qHit != null && qHit.Position.Distance(WardPos) < 600)
            {
                LeeSin.Q.Cast();
                return;
            }

            if (LeeSin.myHero.Distance(WardPos) < 600 && LeeSin.W.IsReady() && !LeeSin.IsSecondCast(LeeSin.W))
            {
                LeeSin.WardJump_Position(WardPos);
                return;
            }
        }

    }
}
