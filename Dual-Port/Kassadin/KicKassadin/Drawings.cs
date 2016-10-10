using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Drawing;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KicKassadin
{
    class Drawings {
        public static void Drawing_OnDraw(EventArgs args) {
            var drawOff = KassMenu.Config.Item("Drawings.Off").GetValue<bool>();
            var drawQ = KassMenu.Config.Item("Drawings.Q").GetValue<bool>();
            var drawW = KassMenu.Config.Item("Drawings.W").GetValue<bool>();
            var drawE = KassMenu.Config.Item("Drawings.E").GetValue<bool>();
            var drawR = KassMenu.Config.Item("Drawings.R").GetValue<bool>();

            if (drawOff)
                return;

            if (drawQ)
                if (KicKassadin.spells[Spells.Q].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, KicKassadin.spells[Spells.Q].Range, Color.White);

            if (drawE)
                if (KicKassadin.spells[Spells.E].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, KicKassadin.spells[Spells.E].Range, Color.White);

            if (drawW)
                if (KicKassadin.spells[Spells.W].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, KicKassadin.spells[Spells.W].Range, Color.White);

            if (drawR)
                if (KicKassadin.spells[Spells.R].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, KicKassadin.spells[Spells.R].Range, Color.White);
        }
    }
}
