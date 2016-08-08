using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HeavenStrikeShyvana
{
    using static Program;
    public static class Drawing
    {
        public static void OnDraw_Draw()
        {
            if (DrawE)
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Green);
            if (DrawR)
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.Green);
        }
    }
}
