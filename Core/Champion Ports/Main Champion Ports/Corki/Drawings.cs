namespace ElCorki
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class Drawings
    {
        #region Public Methods and Operators

        public static void Drawing_OnDraw(EventArgs args)
        {
            var drawOff = ElCorkiMenu._menu.Item("ElCorki.Draw.off").GetValue<bool>();
            var drawQ = ElCorkiMenu._menu.Item("ElCorki.Draw.Q").GetValue<Circle>();
            var drawW = ElCorkiMenu._menu.Item("ElCorki.Draw.W").GetValue<Circle>();
            var drawE = ElCorkiMenu._menu.Item("ElCorki.Draw.E").GetValue<Circle>();
            var drawR = ElCorkiMenu._menu.Item("ElCorki.Draw.R").GetValue<Circle>();
            var drawText = ElCorkiMenu._menu.Item("ElCorki.Draw.Text").GetValue<bool>();
            var rBool = ElCorkiMenu._menu.Item("ElCorki.AutoHarass").GetValue<KeyBind>().Active;

            if (drawOff)
            {
                return;
            }

            var playerPos = Drawing.WorldToScreen(ObjectManager.Player.Position);

            if (drawQ.Active)
            {
                if (Corki.spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Corki.spells[Spells.Q].Range, Color.White);
                }
            }

            if (drawE.Active)
            {
                if (Corki.spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Corki.spells[Spells.E].Range, Color.White);
                }
            }

            if (drawW.Active)
            {
                if (Corki.spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Corki.spells[Spells.W].Range, Color.White);
                }
            }

            if (drawR.Active)
            {
                if (Corki.spells[Spells.R1].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Corki.spells[Spells.R1].Range, Color.White);
                }
            }

            if (drawText)
            {
                Drawing.DrawText(
                    playerPos.X - 70,
                    playerPos.Y + 40,
                    (rBool ? Color.Green : Color.Red),
                    (rBool ? "Auto harass Enabled" : "Auto harass Disabled"));
            }
        }

        #endregion
    }
}