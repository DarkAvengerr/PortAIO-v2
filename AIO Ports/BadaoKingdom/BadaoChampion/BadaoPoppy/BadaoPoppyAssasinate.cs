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
 namespace BadaoKingdom.BadaoChampion.BadaoPoppy
{
    public static class BadaoPoppyAssasinate
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!BadaoPoppyHelper.AssasinateActive())
                return;
            var x = EloBuddy.Drawing.WorldToScreen(ObjectManager.Player.Position);
            var head = EloBuddy.Drawing.WorldToScreen((ObjectManager.Player.Position.To2D() + new Vector2(0, 250)).To3D());
            var selected = TargetSelector.SelectedTarget;
            if (selected.BadaoIsValidTarget())
            {
                var y = EloBuddy.Drawing.WorldToScreen(selected.Position);
                EloBuddy.Drawing.DrawLine(x,y,2,Color.Pink);
                EloBuddy.Drawing.DrawText(head[0], head[1], Color.Yellow, "selected target is: " + selected.ChampionName);
            }
            else
            {
                EloBuddy.Drawing.DrawText(head[0], head[1], Color.Yellow, "please select target");
            }
        }

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!BadaoPoppyHelper.AssasinateActive() || !unit.IsMe)
                return;
            if (target.Position.Distance(ObjectManager.Player.Position) <= 200 + 125 + 140)
                BadaoChecker.BadaoUseTiamat();
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!BadaoPoppyHelper.AssasinateActive())
                return;
            var selectedTarget = TargetSelector.GetSelectedTarget();
            Orbwalking.Orbwalk((selectedTarget.BadaoIsValidTarget() && Orbwalking.InAutoAttackRange(selectedTarget))
                                ?  selectedTarget : null
                                , Game.CursorPos,90,50,false,false);
            if (!selectedTarget.BadaoIsValidTarget())
                return;
            if (BadaoPoppyHelper.UseRComboKillable())
            {
                if (!BadaoMainVariables.R.IsCharging)
                {
                    if (selectedTarget.BadaoIsValidTarget(500) &&
                        BadaoMainVariables.R.GetDamage(selectedTarget) >= selectedTarget.Health)
                        BadaoMainVariables.R.StartCharging();
                }
                else
                {
                    if (selectedTarget.BadaoIsValidTarget(500) &&
                        BadaoMainVariables.R.GetDamage(selectedTarget) >= selectedTarget.Health)
                        BadaoMainVariables.R.Cast(selectedTarget.Position);
                    else
                    {
                        if (selectedTarget.BadaoIsValidTarget(BadaoMainVariables.R.Range))
                            BadaoMainVariables.R.Cast(selectedTarget);
                    }
                }
            }
            if (BadaoMainVariables.E.IsReady() && Environment.TickCount - BadaoPoppyVariables.QCastTick >= 1250)
            {
                if (selectedTarget.BadaoIsValidTarget(BadaoMainVariables.E.Range))
                {
                    var predPos = BadaoMainVariables.E.GetPrediction(selectedTarget).UnitPosition.To2D();
                    if (BadaoMath.GetFirstWallPoint(predPos, predPos.Extend(ObjectManager.Player.Position.To2D(), -300 - selectedTarget.BoundingRadius)) != null)
                    {
                        BadaoMainVariables.E.Cast(selectedTarget);
                        goto nextStep;
                    }
                }
            }
            if (BadaoPoppyHelper.UseEComboGap() && Environment.TickCount - BadaoPoppyVariables.QCastTick >= 1250)
            {
                if (selectedTarget.BadaoIsValidTarget(BadaoMainVariables.E.Range)
                    && !Orbwalking.InAutoAttackRange(selectedTarget)
                    && Prediction.GetPrediction(selectedTarget, 0.5f).UnitPosition.Distance(ObjectManager.Player.Position)
                    > selectedTarget.Distance(ObjectManager.Player.Position) + 20)
                {
                    BadaoMainVariables.E.Cast(selectedTarget);
                }
            }

        nextStep:
            if (BadaoPoppyHelper.UseQCombo())
            {
                if (selectedTarget.BadaoIsValidTarget(BadaoMainVariables.Q.Range))
                {
                    if (BadaoMainVariables.Q.Cast(selectedTarget) == Spell.CastStates.SuccessfullyCasted)
                        BadaoPoppyVariables.QCastTick = Environment.TickCount;
                }
            }
            if (BadaoPoppyHelper.UseWCombo())
            {
                if (selectedTarget.BadaoIsValidTarget(600))
                    BadaoMainVariables.W.Cast();
            }

        }
    }
}
