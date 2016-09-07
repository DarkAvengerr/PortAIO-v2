using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Utils;
using Reforged_Riven.Menu;
using Reforged_Riven.Update;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Reforged_Riven.Draw
{
    internal class SpellRange : Core
    {
        public static void Draw(EventArgs args)
        {
            if (Player.IsDead) return;

            if (MenuConfig.QMinionDraw)
            {
                var minions = GameObjects.EnemyMinions.Where(m => m.IsValidTarget(Player.AttackRange + 350));

                foreach (var m in minions)
                {
                    if (m.Health <= Spells.Q.GetDamage(m))
                    {
                        Render.Circle.DrawCircle(m.Position, m.BoundingRadius, Color.LightSeaGreen);
                    }
                }
            }

            if (MenuConfig.DrawFlee)
            {
                var end = Player.Position.Extend(Game.CursorPos, 200);
                var isWallDash = FleeLogic.IsWallDash(end, 200);

                var wallPoint = FleeLogic.GetFirstWallPoint(Player.ServerPosition, end);
               
                if (isWallDash && wallPoint.Distance(Player.ServerPosition) < 260)
                {
                    Render.Circle.DrawCircle(wallPoint, 60, Color.LightGreen);
                    Render.Circle.DrawCircle(end, 60, Color.White);
                }
               else
                {
                    Render.Circle.DrawCircle(wallPoint, 60, Color.Red);
                }
            }

            if (MenuConfig.BurstKeyBind.Active)
            {
                var textPos = Drawing.WorldToScreen(Player.Position);

                Drawing.DrawText(textPos.X - 27, textPos.Y + 15, Spells.Flash.IsReady()
                    ? Color.LightCyan
                    : Color.DarkSlateGray,
                    "Flash Burst");
            }

            if (!MenuConfig.DrawCombo) return;

            if (Spells.Flash.IsReady() && MenuConfig.BurstKeyBind.Active)
            {
                if (Spells.R.IsReady() || MenuConfig.FnoR)
                {
                    Render.Circle.DrawCircle(Player.Position, 425 + Spells.W.Range - 35, Color.Yellow, 4, true);
                }
            }

           if (Spells.E.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, 310 + Player.AttackRange,
                   Spells.Q.IsReady()
                        ? Color.LightBlue
                        : Color.DarkSlateGray);
            }
            else
            {
                Render.Circle.DrawCircle(Player.Position, Player.GetRealAutoAttackRange(Player),
                    Spells.Q.IsReady()
                        ? Color.LightBlue
                        : Color.DarkSlateGray);
            }
        }
    }
}