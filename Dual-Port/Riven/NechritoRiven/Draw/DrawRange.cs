#region

using System;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using NechritoRiven.Core;
using NechritoRiven.Menus;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Draw
{
    internal class DrawRange : Core.Core
    {
        public static void RangeDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            var pos = Drawing.WorldToScreen(Player.Position);

            if (MenuConfig.DrawCb)
            {
                Render.Circle.DrawCircle(Player.Position, 250 + Player.AttackRange + 70,
                    Spells.E.IsReady()
                        ? Color.LightBlue
                        : Color.DarkSlateGray);
            }

            if (MenuConfig.DrawBt && Spells.Flash != SpellSlot.Unknown)
            {
                Render.Circle.DrawCircle(Player.Position, 750,
                    Spells.R.IsReady() && Spells.Flash.IsReady()
                        ? Color.LightBlue
                        : Color.DarkSlateGray);
            }

            if (MenuConfig.DrawFh)
            {
                Render.Circle.DrawCircle(Player.Position, 450 + Player.AttackRange + 70,
                 Spells.E.IsReady() && Spells.Q.IsReady()
                      ? Color.LightBlue
                     : Color.DarkSlateGray);
            }
             

            if (MenuConfig.DrawHs)
            {
                Render.Circle.DrawCircle(Player.Position, 400,
                    Spells.Q.IsReady() && Spells.W.IsReady()
                        ? Color.LightBlue
                        : Color.DarkSlateGray);
            }

            if (MenuConfig.DrawAlwaysR)
            {
                Drawing.DrawText(pos.X - 10, pos.Y + 20, Color.Cyan, "Use R1  (     )");
                Drawing.DrawText(pos.X + 53, pos.Y + 20,
                    MenuConfig.AlwaysR ? Color.White : Color.Red, MenuConfig.AlwaysR ? "On" : "Off");
            }

            if (!MenuConfig.ForceFlash) return;

            Drawing.DrawText(pos.X - 15, pos.Y + 40, Color.Cyan, "Force Flash  (     )");
            Drawing.DrawText(pos.X + 83, pos.Y + 40, MenuConfig.AlwaysF 
                ? Color.White
                : Color.Red,
                MenuConfig.AlwaysF ? "On" : "Off");
        }
    }
}
