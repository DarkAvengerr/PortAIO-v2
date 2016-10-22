using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using BadaoShen;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoShen
{
    public static class BadaoShenLaneClear
    {
        public static void BadaoActivate()
        {
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
        }

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;
            if (args.Target.Team == GameObjectTeam.Neutral)
                return;
            if (BadaoShenHelper.UseQLaneClear())
            {
                BadaoMainVariables.Q.Cast();
            }
        }
    }
}
