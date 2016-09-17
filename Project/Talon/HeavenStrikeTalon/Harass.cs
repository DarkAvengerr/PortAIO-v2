using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HeavenStrikeTalon
{
    using static Program;
    using static Extension;
    public static class Harass
    {
        public static void  UpdateHarass()
        {
            if (HasItem() && WCasted)
            {
                CastItem();
            }
            if (W.IsReady() && WHarass)
            {
                var targetW = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (targetW.IsValidTarget() && !targetW.IsZombie && !OutOfAA(targetW) && !Q.IsReady()
                    && Orbwalking.CanMove(80) && !Orbwalking.CanAttack())
                {
                    W.Cast(targetW);
                }
                else if (targetW.IsValidTarget() && !targetW.IsZombie && OutOfAA(targetW)
                    && Orbwalking.CanMove(80))
                {
                    W.Cast(targetW);
                }
            }
        }
        public static void AfterAttackHarass(AttackableUnit unit, AttackableUnit target)
        {
            if (!(target is AIHeroClient))
                return;
            if (Q.IsReady())
            {
                Q.Cast();
            }
            else if (W.IsReady())
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
        public static void BeforeAttackHarass(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!(args.Target is AIHeroClient))
                return;
            var target = args.Target as Obj_AI_Base;
            var x = Prediction.GetPrediction(args.Target as Obj_AI_Base, Player.AttackCastDelay);
            var chasedis = (Player.Position.To2D().Distance(x.UnitPosition.To2D())
                - Player.BoundingRadius - target.BoundingRadius - Player.AttackRange);
            var chasespeed = (Player.MoveSpeed - target.MoveSpeed);
            var chasetime = chasedis / chasespeed;
            if (Q.IsReady() && OutOfAA(x.UnitPosition.To2D(), target)
                && (chasetime >= Player.AttackDelay || chasetime < 0))
            {
                args.Process = false;
                Q.Cast();
            }
        }
    }
}
