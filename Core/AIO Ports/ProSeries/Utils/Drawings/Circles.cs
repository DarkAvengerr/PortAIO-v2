using System;
using System.Collections.Generic;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ProSeries.Utils.Drawings
{
    public static class Circles
    {
        private static readonly Dictionary<string, object> RangeCircles = new Dictionary<string, object>();

        static Circles()
        {
            Drawing.OnDraw += DrawingOnOnDraw;
        }

        private static void DrawingOnOnDraw(EventArgs args)
        {
            foreach (var circle in RangeCircles)
            {
                var c = ProSeries.Config.SubMenu("Drawings").Item(circle.Key, true).GetValue<Circle>();
                var range = 0f;

                if (circle.Value is Spell)
                {
                    range = ((Spell) circle.Value).Range;
                }

                if (c.Active)
                {
                    Render.Circle.DrawCircle(ProSeries.Player.Position, range, c.Color, 2);
                }
            }
        }

        internal static void Add(string name, object spellOrCallBack)
        {
            ProSeries.Config.SubMenu("Drawings")
                .AddItem(new MenuItem(name, name, true).SetValue(new Circle(true, Color.White)));

            RangeCircles.Add(name, spellOrCallBack);
        }
    }
}