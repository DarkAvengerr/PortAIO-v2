using System;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace Mundo
{
    internal class Drawings : Spells
    {
        public static void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || ConfigMenu.config.Item("disableDraw").GetValue<bool>())
                return;

            var heroPosition = Drawing.WorldToScreen(ObjectManager.Player.Position);
            var textDimension = 70;
            var drawQstatus = ConfigMenu.config.Item("drawAutoQ").GetValue<bool>();
            var autoQ = ConfigMenu.config.Item("autoQ").GetValue<KeyBind>().Active;

            if (drawQstatus && autoQ)
            {
                Drawing.DrawText(heroPosition.X - textDimension, heroPosition.Y - textDimension, Color.DarkOrange, "AutoQ: ON");
            }

            var width = ConfigMenu.config.Item("width").GetValue<Slider>().Value;

            if (ConfigMenu.config.Item("drawQ").GetValue<Circle>().Active && q.Level > 0)
            {
                var circle = ConfigMenu.config.Item("drawQ").GetValue<Circle>();
                Render.Circle.DrawCircle(ObjectManager.Player.Position, circle.Radius, circle.Color, width);
            }

            if (ConfigMenu.config.Item("drawW").GetValue<Circle>().Active && w.Level > 0)
            {
                var circle = ConfigMenu.config.Item("drawW").GetValue<Circle>();
                Render.Circle.DrawCircle(ObjectManager.Player.Position, circle.Radius, circle.Color, width);
            }
        }
    }
}
