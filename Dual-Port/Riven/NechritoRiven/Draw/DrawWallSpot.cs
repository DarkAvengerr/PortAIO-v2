#region

using System;
using LeagueSharp;
using LeagueSharp.Common;
using NechritoRiven.Core;
using NechritoRiven.Event;
using NechritoRiven.Menus;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Draw
{
    internal class DrawWallSpot : Core.Core
    {
        public static void WallDraw(EventArgs args)
        {
            var end = Player.ServerPosition.Extend(Game.CursorPos, Spells.Q.Range);
            var isWallDash = FleeLogic.IsWallDash(end, Spells.Q.Range);

            var wallPoint = FleeLogic.GetFirstWallPoint(Player.ServerPosition, end);

            if (isWallDash && MenuConfig.FleeSpot)
            {
                if (wallPoint.Distance(Player.ServerPosition) <= 600)
                {
                    Render.Circle.DrawCircle(wallPoint, 60, System.Drawing.Color.White);
                    Render.Circle.DrawCircle(end, 60, System.Drawing.Color.Green);
                }
            }
        }
    }
}
