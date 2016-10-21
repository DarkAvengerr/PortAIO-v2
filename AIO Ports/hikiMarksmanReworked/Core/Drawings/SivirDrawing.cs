using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hikiMarksmanRework.Core.Menus;
using hikiMarksmanRework.Core.Spells;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework.Core.Drawings
{
    class SivirDrawing
    {
        public static void Init()
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (SivirMenu.Config.Item("sivir.q.draw").GetValue<Circle>().Active && SivirSpells.Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, SivirSpells.Q.Range, SivirMenu.Config.Item("sivir.q.draw").GetValue<Circle>().Color);
            }
            if (SivirMenu.Config.Item("sivir.w.draw").GetValue<Circle>().Active && SivirSpells.W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, SivirSpells.W.Range, SivirMenu.Config.Item("sivir.w.draw").GetValue<Circle>().Color);
            }
            if (SivirMenu.Config.Item("sivir.e.draw").GetValue<Circle>().Active && SivirSpells.E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, SivirSpells.E.Range, SivirMenu.Config.Item("sivir.e.draw").GetValue<Circle>().Color);
            }
            if (SivirMenu.Config.Item("sivir.r.draw").GetValue<Circle>().Active && SivirSpells.R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, SivirSpells.R.Range, SivirMenu.Config.Item("sivir.r.draw").GetValue<Circle>().Color);
            }
        }
    }
}
