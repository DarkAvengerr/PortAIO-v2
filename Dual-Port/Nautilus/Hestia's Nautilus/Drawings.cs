using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HestiaNautilus
{
    internal class Drawings : Spells
    {
        public static void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || ConfigMenu.config.Item("disableDraw").GetValue<bool>())
                return;

            var width = ConfigMenu.config.Item("width").GetValue<Slider>().Value;

            if (ConfigMenu.config.Item("drawQ").GetValue<Circle>().Active && q.Level > 0)
            {
                var circle = ConfigMenu.config.Item("drawQ").GetValue<Circle>();
                Render.Circle.DrawCircle(ObjectManager.Player.Position, circle.Radius, circle.Color, width);
            }

            if (ConfigMenu.config.Item("drawE").GetValue<Circle>().Active && e.Level > 0)
            {
                var circle = ConfigMenu.config.Item("drawE").GetValue<Circle>();
                Render.Circle.DrawCircle(ObjectManager.Player.Position, circle.Radius, circle.Color, width);
            }

            if (ConfigMenu.config.Item("drawR").GetValue<Circle>().Active && r.Level > 0)
            {
                var circle = ConfigMenu.config.Item("drawR").GetValue<Circle>();
                Render.Circle.DrawCircle(ObjectManager.Player.Position, circle.Radius, circle.Color, width);
            }
        }
    }
}
