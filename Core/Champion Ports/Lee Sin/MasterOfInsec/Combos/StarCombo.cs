using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace MasterOfInsec.Combos
{
    internal static class StarCombo
    {
        public static string steps = "One";

        private static bool star;

        public static void CastQ()
        {
            Program.Q.Cast();
            steps = "One";
        }

        public static void Combo()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Program.Player.Position.Extend(Game.CursorPos, 150));
            var target = TargetSelector.GetTarget(1300, TargetSelector.DamageType.Physical);
            if (star == false)
            {
                if (Program.Q.IsReady() && Program.R.IsReady() && Program.Player.Mana >= 150)
                {
                    star = true;
                    if (!Program.E.IsInRange(target) && Program.W.IsReady())
                    {
                        WardJump.JumpTo(target.Position);

                    }
                    else
                    {

                        star = false;

                    }
                }
            }
            if (star == false) return;



        }
    }
}
