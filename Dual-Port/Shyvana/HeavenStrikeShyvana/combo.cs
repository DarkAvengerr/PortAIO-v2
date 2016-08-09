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
    class combo
    {
        public static void UpdateCombo()
        {
            var targetW = TargetSelector.GetTarget(600, TargetSelector.DamageType.Magical);
            if (WCombo && W.IsReady() && targetW.IsValidTarget() && !targetW.IsZombie)
            {
                W.Cast();
            }

            var targetE = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (ECombo && E.IsReady() && targetE.IsValidTarget() && !targetE.IsZombie
                && Orbwalking.CanMove(80))
            {
                E.Cast(targetE);
            }

            var targetR = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (RCombo && R.IsReady() && targetR.IsValidTarget() && !targetR.IsZombie && Orbwalking.CanMove(80))
            {
                var pred = R.GetPrediction(targetR);
                var chasetime = (targetR.Position.To2D().Distance(Player.Position.To2D()) - Player.AttackRange 
                                - Player.BoundingRadius - targetR.BoundingRadius)
                                / (Player.MoveSpeed - targetR.MoveSpeed);
                if (targetR.Position.To2D().Distance(Player.Position.To2D()) > Player.AttackRange
                    && (Player.Position.To2D().GetFirstWallPoint(targetR.Position.To2D()) != null
                    || chasetime > 1.5 || chasetime < 0)
                    && !pred.CastPosition.IsWall() && pred.Hitchance >= HitChance.Medium)
                {
                    R.Cast(pred.CastPosition);
                }
            }

            if (ItemData.Blade_of_the_Ruined_King.GetItem().IsReady() || ItemData.Bilgewater_Cutlass.GetItem().IsReady() || 
                ItemData.Youmuus_Ghostblade.GetItem().IsReady())
            {
                var targetA = TargetSelector.GetTarget(550, TargetSelector.DamageType.Magical);
                if (targetA.IsValidTarget() && !targetA.IsZombie)
                {
                    if (BotrkCombo)
                    {
                        ItemData.Blade_of_the_Ruined_King.GetItem().Cast(targetA);
                        ItemData.Bilgewater_Cutlass.GetItem().Cast(targetA);
                    }
                    if(YoumuuCombo)
                        ItemData.Youmuus_Ghostblade.GetItem().Cast(targetA);
                }
            }
            if(ItemData.Randuins_Omen.GetItem().IsReady())
            {
                var target = TargetSelector.GetTarget(450, TargetSelector.DamageType.Magical);
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    ItemData.Randuins_Omen.GetItem().Cast();
                }
            }
            if (ComboSmite && Smite.IsReady() && HasSmiteBlue)
            {
                var targetsmiteblue = TargetSelector.GetTarget(Player.BoundingRadius + 620, TargetSelector.DamageType.Magical);
                if (Player.Position.To2D().Distance(targetsmiteblue.Position.To2D()) <= Player.BoundingRadius + targetsmiteblue.BoundingRadius + 500)
                {
                    Player.Spellbook.CastSpell(Smite, targetsmiteblue);
                }
            }
        }
        public static void AfterAttackCombo(AttackableUnit unit, AttackableUnit target)
        {
            if (Q.IsReady())
            {
                Q.Cast();
            }
            else if (HasItem())
            {
                CastItem();
            }
        }
        public static void BeforeAttackCombo(Orbwalking.BeforeAttackEventArgs args)
        {
            var target = args.Target as Obj_AI_Base;
            var x = Prediction.GetPrediction(args.Target as Obj_AI_Base, Player.AttackCastDelay + Game.Ping/1000 + 0.02f);
            var chasetime = (Player.Position.To2D().Distance(x.UnitPosition.To2D())
                - Player.BoundingRadius - target.BoundingRadius - Player.AttackRange)
                / (Player.MoveSpeed - target.MoveSpeed);
            if (Q.IsReady() && Player.Position.To2D().Distance(x.UnitPosition.To2D())
                >= Player.BoundingRadius + Player.AttackRange + args.Target.BoundingRadius
                && (chasetime >= Player.AttackDelay || chasetime < 0))
            {
                args.Process = false;
                Q.Cast();
            }
        }
        public static void OnAttackCombo(AttackableUnit unit, AttackableUnit target)
        {
            if (ComboSmite && Smite.IsReady() && HasSmiteRed && target.IsValidTarget())
            {
                Player.Spellbook.CastSpell(Smite,target);
            }
        }

    }
}
