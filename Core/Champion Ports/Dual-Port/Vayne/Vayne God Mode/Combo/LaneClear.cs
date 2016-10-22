
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GodModeOn_Vayne.Combo
{
    class LaneClear
    {
        public static void Do()
        {
            var MinionN =
                  MinionManager.GetMinions(800, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health)
                      .FirstOrDefault();
            var QLane = Program.menu.Item("QL").GetValue<bool>();
            if (QLane)
            {
                if (MinionN != null)
                {
                    Program.Q.Cast(Game.CursorPos, false);
                }
            }
        }
    }
}
