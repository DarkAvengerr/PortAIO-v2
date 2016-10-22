using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GodModeOn_Vayne.Combo
{
    static class JungleClear
    {
        public static void Do()
        {
            var MinionN =
                  MinionManager.GetMinions(800, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                      .FirstOrDefault();
            var QJungle = Program.menu.Item("QJ").GetValue<bool>();
            var EJungle = Program.menu.Item("EJ").GetValue<bool>();
            if (QJungle)
            {
                if (MinionN != null)
                {
                    Program.Q.Cast(Game.CursorPos, false);
                }
            }
            if (EJungle)
            {
                if (CanCondemn(Program.Player.Position, MinionN))
                    Program.E.Cast(MinionN);
            }
        }
        public static bool CanCondemn(Vector3 fromPosition, Obj_AI_Base target)
        {
            var line = new Geometry.Polygon.Line(target.Position, Program.Efinishpos(target));
            if (line.Points.Any(point => point.To3D().IsWall()))
            {
                return true;
            }
            /*        if (Program.Efinishpos(target).IsWall())
                        return true;*/
            return false;
        }


    }
}
