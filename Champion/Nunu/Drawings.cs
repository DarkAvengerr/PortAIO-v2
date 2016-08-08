using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using EloBuddy;

namespace LSharpNunu
{
    public class Drawings
    {
        public static void Drawing_OnDraw(EventArgs args)
        {
            if (Nunu.Player.IsDead)
                return;

            var drawOff = NunuMenu._menu.Item("Nunu.Draw.off").GetValue<bool>();
            var drawQ = NunuMenu._menu.Item("Nunu.Draw.q").GetValue<Circle>();
            var drawW = NunuMenu._menu.Item("Nunu.Draw.W").GetValue<Circle>();
            var drawE = NunuMenu._menu.Item("Nunu.Draw.E").GetValue<Circle>();
            var drawR = NunuMenu._menu.Item("Nunu.Draw.R").GetValue<Circle>();

            if (drawOff)
                return;

            if (drawQ.Active)
                if (Nunu.spells[Spells.Q].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Nunu.spells[Spells.Q].Range, Color.White);

            if (drawW.Active)
                if (Nunu.spells[Spells.W].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Nunu.spells[Spells.W].Range, Color.White);

            if (drawE.Active)
                if (Nunu.spells[Spells.E].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Nunu.spells[Spells.E].Range, Color.White);

            if (drawR.Active)
                if (Nunu.spells[Spells.R].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Nunu.spells[Spells.R].Range, Color.White);
        }

        public static void OnDrawEndScene(EventArgs args)
        {
            if (Nunu.Player.IsDead)
                return;
        }
    }
}