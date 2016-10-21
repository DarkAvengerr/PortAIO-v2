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
 namespace ElKalista
{
    internal class Drawings
    {

        public static void Drawing_OnDraw(EventArgs args)
        {
            if (Kalista.Player.IsDead)
                return;

            var drawOff = ElKalistaMenu._menu.Item("ElKalista.Draw.off").GetValue<bool>();
            var drawQ = ElKalistaMenu._menu.Item("ElKalista.Draw.Q").GetValue<Circle>();
            var drawW = ElKalistaMenu._menu.Item("ElKalista.Draw.W").GetValue<Circle>();
            var drawE = ElKalistaMenu._menu.Item("ElKalista.Draw.E").GetValue<Circle>();
            var drawR = ElKalistaMenu._menu.Item("ElKalista.Draw.R").GetValue<Circle>();
            var drawText = ElKalistaMenu._menu.Item("ElKalista.Draw.Text").GetValue<bool>();
            var rBool = ElKalistaMenu._menu.Item("ElKalista.AutoHarass").GetValue<KeyBind>().Active;       

            if (drawOff)
                return;

            var playerPos = Drawing.WorldToScreen(ObjectManager.Player.Position);

            if (drawQ.Active)
                if (Kalista.spells[Spells.Q].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Kalista.spells[Spells.Q].Range, Color.White);

            if (drawW.Active)
                if (Kalista.spells[Spells.W].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Kalista.spells[Spells.W].Range, Color.White);

            if (drawE.Active)
                if (Kalista.spells[Spells.E].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Kalista.spells[Spells.E].Range, Color.White);

            if (drawR.Active)
                if (Kalista.spells[Spells.R].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Kalista.spells[Spells.R].Range, Color.White);

            //if (drawText)
               // Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, (rBool ? Color.Green : Color.Red), "{0}", (rBool ? "Auto harass Enabled" : "Auto harass Disabled"));
        }
    }
}