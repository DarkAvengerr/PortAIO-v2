using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LCS_Lucian;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace LCS_Lucian
{

    class LucianDrawing
    {
        public static Random random;

        public static void Init()
        {

            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (LucianMenu.Config.Item("lucian.q.draw").GetValue<Circle>().Active && LucianSpells.Q.LSIsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, LucianSpells.Q.Range, LucianMenu.Config.Item("lucian.q.draw").GetValue<Circle>().Color);
            }
            if (LucianMenu.Config.Item("lucian.q2.draw").GetValue<Circle>().Active && LucianSpells.Q2.LSIsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, LucianSpells.Q2.Range, LucianMenu.Config.Item("lucian.q2.draw").GetValue<Circle>().Color);
            }
            if (LucianMenu.Config.Item("lucian.w.draw").GetValue<Circle>().Active && LucianSpells.W.LSIsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, LucianSpells.W.Range, LucianMenu.Config.Item("lucian.w.draw").GetValue<Circle>().Color);
            }
            if (LucianMenu.Config.Item("lucian.e.draw").GetValue<Circle>().Active && LucianSpells.E.LSIsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, LucianSpells.E.Range, LucianMenu.Config.Item("lucian.e.draw").GetValue<Circle>().Color);
            }
            if (LucianMenu.Config.Item("lucian.r.draw").GetValue<Circle>().Active && LucianSpells.R.LSIsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, LucianSpells.R.Range, LucianMenu.Config.Item("lucian.r.draw").GetValue<Circle>().Color);
            }
        }
    }
}