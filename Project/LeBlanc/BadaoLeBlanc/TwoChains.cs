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
 namespace Leblanc
{
    public static class TwoChains
    {
        public static Spell R2 = new Spell(SpellSlot.R, 950);
        public static int LastChain = 0;
        public static int LastChainM = 0;
        public static void TwoChainsActive(AIHeroClient target)
        {
            //string t = "";
            //if (target.IsValidTarget())
            //{
            //    foreach (var x in target.Buffs)
            //    {
            //        t += ", " + x.Name;
            //    }
            //    Chat.Print(t);
            //}

            R2.SetSkillshot(0.25f, 70, 1600, true, SkillshotType.SkillshotLine);
            Orbwalking.Orbwalk(target, Game.CursorPos, 90, 50);
            if (target != null && target.IsValidTarget(Program.Q.Range) && (Program.Rstate != 3 || !R2.IsReady()))
            {
                Program.Q.Cast(target);
            }
            if (target != null && target.IsValidTarget(Program.E.Range) && Program.E.IsReady() && !ObjectManager.Player.IsDashing()
                 && !target.HasBuff("LeblancSoulShackleM") && Environment.TickCount - LastChainM >= 1500 + Game.Ping)
            {
                Program.E.Cast(target);
                LastChain = Environment.TickCount;
            }
            if (target != null && target.IsValidTarget(Program.E.Range) && Program.R.IsReady() && !ObjectManager.Player.IsDashing() && Program.Rstate == 3
                && !target.HasBuff("LeblancSoulShackle") && Environment.TickCount - LastChain >= 1500 + Game.Ping)
            {
                R2.Cast(target);
                LastChainM = Environment.TickCount;
            }
            if (target.IsValidTarget(Program.W.Range) && Program.Menu.Item("Use W Combo").GetValue<bool>() && (Program.Rstate != 3 || !R2.IsReady()))
            {
                Program.CastW(target);
            }
        }
    }
}
