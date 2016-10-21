#region

using System;
using LeagueSharp;
using LeagueSharp.SDK.Utils;
using Spirit_Karma.Core;
using Spirit_Karma.Menus;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Spirit_Karma.Draw 
{
    internal class DrawMantra : Core.Core
    {

        public static void SelectedMantra(EventArgs args)
        {
            if(Player.IsDead || !MenuConfig.MantraDraw || !MenuConfig.UseDrawings) return;
            var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);

            if (MenuConfig.QRange && MenuConfig.MantraMode.Index == 0)
            {
                Render.Circle.DrawCircle(Player.Position, Spells.Q.Range,
                   Spells.R.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            }

            if (MenuConfig.QRange && MenuConfig.MantraMode.Index == 1)
            {
                Render.Circle.DrawCircle(Player.Position, Spells.W.Range,
                   Spells.R.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            }

            if (MenuConfig.QRange && MenuConfig.MantraMode.Index == 2)
            {
                Render.Circle.DrawCircle(Player.Position, Spells.E.Range,
                   Spells.R.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            }

            if (MenuConfig.QRange && MenuConfig.MantraMode.Index == 3)
            {
                Render.Circle.DrawCircle(Player.Position, Spells.Q.Range,
                   Spells.R.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            }

            if (MenuConfig.MantraMode.SelectedValue == "Q")
            {
                Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.White, "Selected Prio: Q");
            }
            if (MenuConfig.MantraMode.SelectedValue == "W")
            {
                Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.White, "Selected Prio: W");
            }
            if (MenuConfig.MantraMode.SelectedValue == "E")
            {
                Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.White, "Selected Prio: E");
            }
            if (MenuConfig.MantraMode.SelectedValue == "Auto")
            {
                Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.White, "Selected Prio: Auto");
            }
        }
    }
}
