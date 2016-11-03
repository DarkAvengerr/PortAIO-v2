using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Dark_Star_Thresh.Drawings
{
    using System;
    using System.Drawing;

    using Dark_Star_Thresh.Core;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class DrawRange : Core
    {
        public static void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;

            var pos = Drawing.WorldToScreen(Player.Position);

            Drawing.DrawText(pos.X - 20, pos.Y + 40, Color.DodgerBlue, "Use Q2 (     )");

            Drawing.DrawText(pos.X + 42, pos.Y + 40, MenuConfig.Q2 
                ? Color.White
                : Color.Red,
                MenuConfig.Q2 ? "On" : "Off");

            if (MenuConfig.DrawQ)
            {
                Render.Circle.DrawCircle(
                    Player.Position,
                    MenuConfig.ComboQ,
                    Spells.Q.IsReady() 
                    ? Color.White 
                    : Color.DarkSlateGray);
            }

            if (MenuConfig.DrawW)
            {
                Render.Circle.DrawCircle(
                    Player.Position,
                    Spells.W.Range,
                    Spells.W.IsReady()
                    ? Color.White 
                    : Color.DarkSlateGray);
            }

            if (MenuConfig.DrawE)
            {
                Render.Circle.DrawCircle(
                    Player.Position,
                    Spells.E.Range,
                    Spells.E.IsReady() 
                    ? Color.White
                    : Color.DarkSlateGray);
            }

            if (MenuConfig.DrawR)
            {
                Render.Circle.DrawCircle(
                    Player.Position,
                    Spells.R.Range,
                    Spells.R.IsReady()
                    ? Color.White 
                    : Color.DarkSlateGray);
            }
        }
    }
}
