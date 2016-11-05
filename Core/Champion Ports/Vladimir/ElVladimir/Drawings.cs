using EloBuddy; namespace ElVladimirReborn
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal class Drawings
    {
        #region Public Methods and Operators

        public static void OnDraw(EventArgs args)
        {
            var drawOff = ElVladimirMenu.Menu.Item("ElVladimir.Draw.off").GetValue<bool>();
            var drawQ = ElVladimirMenu.Menu.Item("ElVladimir.Draw.Q").GetValue<Circle>();
            var drawW = ElVladimirMenu.Menu.Item("ElVladimir.Draw.W").GetValue<Circle>();
            var drawE = ElVladimirMenu.Menu.Item("ElVladimir.Draw.E").GetValue<Circle>();
            var drawR = ElVladimirMenu.Menu.Item("ElVladimir.Draw.R").GetValue<Circle>();
            //var drawText = ElVladimirMenu._menu.Item("ElVladimir.Draw.Text").GetValue<bool>();
            //var rBool = ElVladimirMenu._menu.Item("ElVladimir.AutoHarass.Activated").GetValue<KeyBind>().Active;

            if (drawOff)
            {
                return;
            }

            var playerPos = Drawing.WorldToScreen(ObjectManager.Player.Position);

            if (drawQ.Active)
            {
                if (Vladimir.spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Vladimir.spells[Spells.Q].Range,
                        Color.White);
                }
            }

            if (drawE.Active)
            {
                if (Vladimir.spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Vladimir.spells[Spells.E].Range,
                        Color.White);
                }
            }

            if (drawW.Active)
            {
                if (Vladimir.spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Vladimir.spells[Spells.W].Range,
                        Color.White);
                }
            }

            if (drawR.Active)
            {
                if (Vladimir.spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Vladimir.spells[Spells.R].Range,
                        Color.White);
                }
            }

            //if (drawText)
            //Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, (rBool ? Color.Green : Color.Red), "{0}", (rBool ? "Auto harass Enabled" : "Auto harass Disabled"));
        }

        #endregion
    }
}
 