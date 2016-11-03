using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Draw
{
    #region

    using System;
    using System.Drawing;

    using Core;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class DrawMisc : Core
    {
        #region Public Methods and Operators

        public static void RangeDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (MenuConfig.DrawCb)
            {
                if (Spells.E.IsReady())
                {
                    Render.Circle.DrawCircle(
                        Player.Position,
                        310 + Player.AttackRange,
                        Spells.Q.IsReady()
                        ? Color.DodgerBlue
                        : Color.DarkSlateGray,
                        3,
                        true);
                }
                else
                {
                    Render.Circle.DrawCircle(
                        Player.Position,
                        Player.AttackRange,
                        Spells.Q.IsReady() 
                        ? Color.LightBlue 
                        : Color.DarkSlateGray,
                        3);
                }
            }

            if (MenuConfig.DrawBt && Spells.Flash != SpellSlot.Unknown && Spells.Flash.IsReady())
            {
                Render.Circle.DrawCircle(
                     Player.Position,
                     Player.AttackRange + 625,
                     Color.Orange,
                    3);
            }

            if (MenuConfig.DrawFh)
            {
                Render.Circle.DrawCircle(
                    Player.Position,
                    450 + Player.AttackRange + 70,
                    Spells.E.IsReady() && Spells.Q.IsReady()
                    ? Color.LightBlue 
                    : Color.DarkSlateGray,
                    3);
            }

            if (MenuConfig.DrawHs)
            {
                Render.Circle.DrawCircle(
                    Player.Position,
                    400,
                    Spells.Q.IsReady() && Spells.W.IsReady()
                    ? Color.LightBlue 
                    : Color.DarkSlateGray,
                    3);
            }

            var pos = Drawing.WorldToScreen(Player.Position);

            if (MenuConfig.DrawAlwaysR)
            {
                Drawing.DrawText(pos.X - 20, pos.Y + 20, Color.DodgerBlue, "Use R1  (     )");

                Drawing.DrawText(pos.X + 43, pos.Y + 20, MenuConfig.AlwaysR 
                     ? Color.Yellow
                       : Color.Red,

                    MenuConfig.AlwaysR 
                     ? "On"
                       : "Off");
            }

            if (!MenuConfig.ForceFlash)
            {
                return;
            }

            Drawing.DrawText(pos.X - 20, pos.Y + 40, Color.DodgerBlue, "Use Flash  (     )");

            Drawing.DrawText(pos.X + 64, pos.Y + 40, MenuConfig.AlwaysF 
                ? Color.Yellow
                : Color.Red,
                MenuConfig.AlwaysF
                ? "On"
                : "Off");
        }

        #endregion
    }
}