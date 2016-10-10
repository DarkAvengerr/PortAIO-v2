using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElEkko
{
    using SharpDX;

    using Color = System.Drawing.Color;

    internal class Drawings
    {
        public static void OnDraw(EventArgs args)
        {
            var drawOff = ElEkkoMenu._menu.Item("ElEkko.Draw.off").GetValue<bool>();
            var drawQ = ElEkkoMenu._menu.Item("ElEkko.Draw.Q").GetValue<Circle>();
            var drawW = ElEkkoMenu._menu.Item("ElEkko.Draw.W").GetValue<Circle>();
            var drawE = ElEkkoMenu._menu.Item("ElEkko.Draw.E").GetValue<Circle>();
            var drawR = ElEkkoMenu._menu.Item("ElEkko.Draw.R").GetValue<Circle>();

            if (drawOff)
                return;

            if (drawQ.Active)
                if (ElEkko.spells[Spells.Q].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, ElEkko.spells[Spells.Q].Range, System.Drawing.Color.White);

            if (drawE.Active)
                if (ElEkko.spells[Spells.E].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, ElEkko.spells[Spells.E].Range, System.Drawing.Color.White);

            if (drawW.Active)
                if (ElEkko.spells[Spells.W].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, ElEkko.spells[Spells.W].Range, System.Drawing.Color.White);

            if (drawR.Active)
            {
                if (ElEkko.spells[Spells.R].Level > 0)
                {
                    if (ElEkko.Troy != null && ElEkko.Troy.IsValid)
                    {
                        Render.Circle.DrawCircle(ElEkko.Troy.Position, ElEkko.spells[Spells.R].Range, Color.Orange);
                    }
                }
            }
        }
    }
}