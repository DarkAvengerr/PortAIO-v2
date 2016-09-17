using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Hikigaya_Lux.Core
{
    class Drawings
    {
        public static void Init()
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (LuxMenu.Config.Item("q.draw").GetValue<Circle>().Active && Spells.Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.Q.Range, LuxMenu.Config.Item("q.draw").GetValue<Circle>().Color);
            }
            if (LuxMenu.Config.Item("w.draw").GetValue<Circle>().Active && Spells.W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.W.Range, LuxMenu.Config.Item("w.draw").GetValue<Circle>().Color);
            }
            if (LuxMenu.Config.Item("e.draw").GetValue<Circle>().Active && Spells.E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.E.Range, LuxMenu.Config.Item("e.draw").GetValue<Circle>().Color);
            }
            if (LuxMenu.Config.Item("r.draw").GetValue<Circle>().Active && Spells.R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.R.Range, LuxMenu.Config.Item("r.draw").GetValue<Circle>().Color);
            }
        }
    }
}
