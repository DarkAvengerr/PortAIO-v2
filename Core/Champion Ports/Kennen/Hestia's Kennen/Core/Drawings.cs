using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Kennen.Core
{
    internal class Drawings
    {
        public static void Drawing_OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || Configs.config.Item("disableDraw").GetValue<bool>())
                return;

            var width = Configs.config.Item("width").GetValue<Slider>().Value;

            if (Configs.config.Item("drawQ").GetValue<Circle>().Active && Spells.Q.Level > 0)
            {
                var circle = Configs.config.Item("drawQ").GetValue<Circle>();
                Render.Circle.DrawCircle(ObjectManager.Player.Position, circle.Radius, circle.Color, width);
            }

            if (Configs.config.Item("drawW").GetValue<Circle>().Active && Spells.W.Level > 0)
            {
                var circle = Configs.config.Item("drawW").GetValue<Circle>();
                Render.Circle.DrawCircle(ObjectManager.Player.Position, circle.Radius, circle.Color, width);
            }

            if (Configs.config.Item("drawR").GetValue<Circle>().Active && Spells.R.Level > 0)
            {
                var circle = Configs.config.Item("drawR").GetValue<Circle>();
                Render.Circle.DrawCircle(ObjectManager.Player.Position, circle.Radius, circle.Color, width);
            }
        }
    }
}
