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
 namespace ElSinged
{
    internal class Drawings
    {
        public static void Drawing_OnDraw(EventArgs args)
        {
            var drawOff = ElSingedMenu.Menu.Item("ElSinged.Draw.off").GetValue<bool>();
            var drawQ = ElSingedMenu.Menu.Item("ElSinged.Draw.Q").GetValue<Circle>();
            var drawW = ElSingedMenu.Menu.Item("ElSinged.Draw.W").GetValue<Circle>();
            var drawE = ElSingedMenu.Menu.Item("ElSinged.Draw.E").GetValue<Circle>();

            if (drawOff)
                return;

            if (drawQ.Active)
                if (Singed.spells[Spells.Q].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Singed.spells[Spells.Q].Range, Singed.spells[Spells.Q].LSIsReady() ? Color.Green : Color.Red);

            if (drawW.Active)
                if (Singed.spells[Spells.W].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Singed.spells[Spells.W].Range, Singed.spells[Spells.W].LSIsReady() ? Color.Green : Color.Red);

            if (drawE.Active)
                if (Singed.spells[Spells.E].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Singed.spells[Spells.E].Range, Singed.spells[Spells.E].LSIsReady() ? Color.Green : Color.Red);
        }
    }
}