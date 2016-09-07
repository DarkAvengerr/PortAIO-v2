using System;
using System.Collections.Generic;
using System.Linq;
using System;
using LeagueSharp;
using LeagueSharp.Common;
using Dark_Star_Thresh.Core;
using Dark_Star_Thresh.Update;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Dark_Star_Thresh.Drawings
{
    class DrawRange : Core.Core
    {
        public static void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;

            var pos = Drawing.WorldToScreen(Player.Position);

            if (MenuConfig.DrawQ) { Render.Circle.DrawCircle(Player.Position, Spells.Q.Range,Spells.Q.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed); }

            if (MenuConfig.DrawW) { Render.Circle.DrawCircle(Player.Position, Spells.W.Range, Spells.W.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed); }

            if (MenuConfig.DrawE) { Render.Circle.DrawCircle(Player.Position, Spells.E.Range, Spells.E.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed); }

            if (MenuConfig.DrawR) { Render.Circle.DrawCircle(Player.Position, Spells.R.Range, Spells.R.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed); }

            if(MenuConfig.DrawPred)
            {
                var Target = TargetSelector.GetSelectedTarget();

                if (Target != null && !Target.IsDead && Target.IsValidTarget(Spells.Q.Range))
                {
                    Render.Circle.DrawCircle(Mode.qPred(Target), 50, System.Drawing.Color.GhostWhite);
                }
            }
        }
    }
}
