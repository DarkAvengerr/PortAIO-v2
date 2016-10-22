using LeagueSharp.SDK.Utils;
using Preserved_Kassadin.Cores;
using System;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Preserved_Kassadin.Update.Draw
{
    class DrawSpells : Core
    {
        public static void OnDraw(EventArgs args)
        {
            if (MenuConfig.DisableDraw) return;

            if(MenuConfig.DrawQ) Render.Circle.DrawCircle(Player.Position, Spells.Q.Range, 
                Spells.Q.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);

            if (MenuConfig.DrawE) Render.Circle.DrawCircle(Player.Position, Spells.E.Range,
                Spells.E.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);

            if (MenuConfig.DrawR) Render.Circle.DrawCircle(Player.Position, Spells.R.Range,
                Spells.R.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
        }
    }
}
