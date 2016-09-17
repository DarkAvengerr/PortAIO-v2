using LeagueSharp;
using LeagueSharp.Common;
using Nechrito_Rengar;
using Nechrito_Rengar.Main;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Rengar.Drawings
{
    class DRAWING : Core
    {
        public static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);

            if (MenuConfig.Passive)
            {
                Drawing.DrawText(heropos.X - 15, heropos.Y + 20, System.Drawing.Color.DodgerBlue, "Passive  (     )");
                Drawing.DrawText(heropos.X + 53, heropos.Y + 20,
                    MenuConfig.Passive ? System.Drawing.Color.LimeGreen : System.Drawing.Color.Red, MenuConfig.Passive ? "On" : "Off");
            }

            if (MenuConfig.EngageDraw)
            {
                Render.Circle.DrawCircle(Player.Position, Champion.E.Range,
                   Champion.E.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            }
        }
    }

}