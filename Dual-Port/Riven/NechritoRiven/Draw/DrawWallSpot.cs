using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Draw
{
    #region

    using System;
    using System.Drawing;

    using Core;

    using Event;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Menus;

    #endregion

    internal class DrawWallSpot : Core
    {
        #region Public Methods and Operators

        public static void WallDraw(EventArgs args)
        {
            if (!MenuConfig.FleeSpot)
            {
                return;
            }

            var end = Player.ServerPosition.Extend(Game.CursorPos, 350);
            var isWallDash = FleeLogic.IsWallDash(end, 350);

            var wallPoint = FleeLogic.GetFirstWallPoint(Player.ServerPosition, end);

            if (isWallDash)
            {
                if (wallPoint.Distance(Player.ServerPosition) <= 600)
                {
                    Render.Circle.DrawCircle(wallPoint, 60, Color.DarkSlateGray);
                    Render.Circle.DrawCircle(end, 60, Color.White);
                }
            }
        }

        #endregion
    }
}