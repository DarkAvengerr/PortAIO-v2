using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using EloBuddy;

namespace ElSejuani
{
    internal class Drawings
    {
        public static void OnDraw(EventArgs args)
        {
            var drawOff = ElSejuaniMenu.Menu.Item("ElSejuani.Draw.off").GetValue<bool>();
            var drawQ = ElSejuaniMenu.Menu.Item("ElSejuani.Draw.Q").GetValue<Circle>();
            var drawW = ElSejuaniMenu.Menu.Item("ElSejuani.Draw.W").GetValue<Circle>();
            var drawE = ElSejuaniMenu.Menu.Item("ElSejuani.Draw.E").GetValue<Circle>();
            var drawR = ElSejuaniMenu.Menu.Item("ElSejuani.Draw.R").GetValue<Circle>();


            if (drawOff)
                return;

            var playerPos = Drawing.WorldToScreen(ObjectManager.Player.Position);


            if (drawQ.Active)
                if (Sejuani.spells[Spells.Q].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Sejuani.spells[Spells.Q].Range, Color.White);

            if (drawW.Active)
                if (Sejuani.spells[Spells.W].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Sejuani.spells[Spells.W].Range, Color.White);

            if (drawE.Active)
                if (Sejuani.spells[Spells.E].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Sejuani.spells[Spells.E].Range, Color.White);

            if (drawR.Active)
                if (Sejuani.spells[Spells.R].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Sejuani.spells[Spells.R].Range, Color.White);
        }
    }
}