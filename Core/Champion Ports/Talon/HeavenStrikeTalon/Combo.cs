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
            //Q dash
            if (Q.IsReady() && Q1Combo)
            {
                var target = TargetSelect(Q.Range);
                if (target != null && !Orbwalking.InAutoAttackRange(target) && Orbwalking.CanMove(90))
                {
                    Q.Cast(target);
                }
            }
            //W
            if (W.IsReady() && WCombo && !Player.HasBuff("TalonRStealth"))
            {
                var target = TargetSelect(W.Range);
                if (target != null && Orbwalking.CanMove(90) && !(Orbwalking.CanAttack() && Orbwalking.InAutoAttackRange(target)))
                {
                    W.Cast(target);
                }
            }
            //R1
            if (R1IsReady() && RCombo)
            {
                var target = TargetSelect(R.Range);
                if (target != null && Orbwalking.CanMove(90) && (Orbwalking.InAutoAttackRange(target) || Q.IsReady()))
                {
                    R.Cast();
                }
            }
        }
        public static void AfterAttackCombo(AttackableUnit unit, AttackableUnit target)
        {
            if (Q.IsReady())
            {
                Q.Cast(target as Obj_AI_Base);
            }
            else if (HasItem())
            {
                CastItem();
            }
        }
        public static void BeforeAttackCombo(Orbwalking.BeforeAttackEventArgs args)
        {

            if (args.Unit.IsMe )

            {

                if (ItemData.Youmuus_Ghostblade.GetItem().IsReady())

                    ItemData.Youmuus_Ghostblade.GetItem().Cast();

            }
        }
    }
}

