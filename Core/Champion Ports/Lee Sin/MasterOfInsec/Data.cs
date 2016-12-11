using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace MasterOfInsec
{
    internal static class Data
    {
        public static void castSpell(Spell p, String menu)
        {
            var Passive = Program.menu.Item(menu).GetValue<bool>();
            if (!Program.Passive())
            {
                Program.passive = false;
            }
            if (Passive == false && Program.passive)
            {
                p.Cast();
            }
            else
            {
                if (!Program.Passive())
                {
                    p.Cast();
                }
            }
        }

        public static void castSpell(Spell p, String menu, Obj_AI_Base target)
        {
            var Passive = Program.menu.Item(menu).GetValue<bool>();
            if (!Program.Passive())
            {
                Program.passive = false;
            }
            if (Passive == false && Program.passive)
            {
                p.Cast(target);
            }
            else
            {
                if (!Program.Passive())
                {
                    p.Cast(target);
                }
            }
        }
    }
}