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
 namespace HeavenStrikeShyvana
{
    using static Program;
    using static extension;
    class harass
    {
        public static void UpdateHarass()
        {
            var targetE = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (EHarass && E.LSIsReady() && targetE.LSIsValidTarget() && !targetE.IsZombie
                && Orbwalking.CanMove(80))
            {
                E.Cast(targetE);
            }
        }
        public static void AfterAttackHarass(AttackableUnit unit, AttackableUnit target)
        {
            if (!(target is AIHeroClient))
                return;
            if (Q.LSIsReady())
            {
                Q.Cast();
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
            var x = Prediction.GetPrediction(args.Target as Obj_AI_Base, Player.AttackCastDelay + Game.Ping / 1000 + 0.02f);
            var chasetime = (Player.Position.LSTo2D().LSDistance(x.UnitPosition.LSTo2D())
                - Player.BoundingRadius - target.BoundingRadius - Player.AttackRange)
                / (Player.MoveSpeed - target.MoveSpeed);
            if (Q.LSIsReady() && Player.Position.LSTo2D().LSDistance(x.UnitPosition.LSTo2D())
                >= Player.BoundingRadius + Player.AttackRange + args.Target.BoundingRadius
                && (chasetime >= Player.AttackDelay || chasetime < 0))
            {
                args.Process = false;
                Q.Cast();
            }
        }
    }
}
