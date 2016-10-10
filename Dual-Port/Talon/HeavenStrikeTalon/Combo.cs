using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HeavenStrikeTalon
{
    using static Extension;
    using static Program;
    using static Gettarget;
    class Combo
    {
        public static void UpdateCombo()
        {
            if(WCasted)
            {
                if (R1IsReady() && RCombo)
                    R.Cast();
                else if (Q.IsReady() && Player.HasBuff("talonshadowassaultbuff") && !Orbwalking.CanAttack()
                    && !HasItem() && !OutOfAA(TargetSelect(Player.BoundingRadius + Player.AttackRange + 70)))
                    Q.Cast();
                else if (HasItem() && WCasted && (!R1IsReady() || !RCombo))
                    CastItem();
            }
            if (W.IsReady() && WCombo)
            {
                var targetW = TargetSelect(W.Range);
                if (targetW.IsValidTarget() && !targetW.IsZombie && !OutOfAA(targetW) && !Q.IsReady()
                    && Orbwalking.CanMove(80) && !Orbwalking.CanAttack())
                {
                    W.Cast(targetW);
                }
                else if (targetW.IsValidTarget() && !targetW.IsZombie && OutOfAA(targetW)
                    && Orbwalking.CanMove(80) && (!E.IsReady() || !ECombo))
                {
                    W.Cast(targetW);
                }
            }
            if (E.IsReady() && ECombo)
            {
                var targetE = TargetSelect(E.Range);
                if (targetE.IsValidTarget() && !targetE.IsZombie && OutOfAA(targetE) && Orbwalking.CanAttack())
                {
                    E.Cast(targetE);
                }
                else if (targetE.IsValidTarget() && !targetE.IsZombie && OutOfAA(targetE) && !Orbwalking.CanAttack() && Orbwalking.CanMove(80))
                {
                    var x = Prediction.GetPrediction(targetE, Player.AttackDelay - (Utils.GameTimeTickCount - Orbwalking.LastAATick));
                    var y = Prediction.GetPrediction(Player, Player.AttackDelay - (Utils.GameTimeTickCount - Orbwalking.LastAATick));
                    if (x.UnitPosition.To2D().Distance(y.UnitPosition.To2D()) > E.Range)
                        E.Cast(targetE);
                }
            }
            if (R1IsReady() && RCombo)
            {
                var targetR = TargetSelect(R.Range);
                if (targetR.IsValidTarget() && !targetR.IsZombie && OutOfAA(targetR) && (!E.IsReady() || !ECombo) && (!W.IsReady() || !WCombo))
                {
                    R.Cast();
                    LastUltPos = Player.Position.To2D();
                }
                else if (targetR.IsValidTarget() && !targetR.IsZombie && !OutOfAA(targetR)
                    && (!W.IsReady() || !WCombo) && !Orbwalking.CanAttack() && Orbwalking.CanMove(80))
                {
                    R.Cast();
                    LastUltPos = Player.Position.To2D();
                }
            }
            if (Player.HasBuff("talonshadowassaultbuff") && RCombo)
            {
                var targetR2 = TargetSelect(R.Range, LastUltPos.IsValid() ? LastUltPos.To3D() : (Vector3?)null);
                if (targetR2.IsValidTarget() && !targetR2.IsZombie && targetR2.Position.To2D().Distance(LastUltPos) >= 450 &&
                    (OutOfAA(targetR2) || (!OutOfAA(targetR2) && !Orbwalking.CanAttack() && Orbwalking.CanMove(80))))
                {
                    if (R2IsReady())
                        R.Cast();
                    else if (HasItem())
                        CastItem();
                }
            }
            if (ItemData.Blade_of_the_Ruined_King.GetItem().IsReady() || ItemData.Bilgewater_Cutlass.GetItem().IsReady() ||
                ItemData.Youmuus_Ghostblade.GetItem().IsReady())
            {
                var targetA = TargetSelect(550);
                if (targetA.IsValidTarget() && !targetA.IsZombie)
                {
                    if (BotrkCombo)
                    {
                        ItemData.Blade_of_the_Ruined_King.GetItem().Cast(targetA);
                        ItemData.Bilgewater_Cutlass.GetItem().Cast(targetA);
                    }
                    if (YoumuuCombo)
                        ItemData.Youmuus_Ghostblade.GetItem().Cast(targetA);
                }
            }
        }
        public static void AfterAttackCombo(AttackableUnit unit, AttackableUnit target)
        {
            if (W.IsReady() && WCombo && R1IsReady() && RCombo)
            {
                W.Cast(target as Obj_AI_Base);
                WCasted = true;
                WCount = Utils.GameTimeTickCount;
            }
            else if (Q.IsReady())
            {
                Q.Cast();
            }
            else if (W.IsReady() && WCombo)
            {
                W.Cast(target as Obj_AI_Base);
                WCasted = true;
                WCount = Utils.GameTimeTickCount;
            }
            else if (HasItem())
            {
                CastItem();
            }
        }
        public static void BeforeAttackCombo(Orbwalking.BeforeAttackEventArgs args)
        {
            var target = args.Target as Obj_AI_Base;
            var x = Prediction.GetPrediction(args.Target as Obj_AI_Base, Player.AttackCastDelay);
            var chasedis = (Player.Position.To2D().Distance(x.UnitPosition.To2D())
                - Player.BoundingRadius - target.BoundingRadius - Player.AttackRange);
            var chasespeed = (Player.MoveSpeed - target.MoveSpeed);
            var chasetime = chasedis / chasespeed;
            if (Q.IsReady() && (!E.IsReady() || !ECombo) && OutOfAA(x.UnitPosition.To2D(),target)
                && (chasetime >= Player.AttackDelay || chasetime < 0))
            {
                args.Process = false;
                Q.Cast();
            }
        }
    }
}

