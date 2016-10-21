#region

using System;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Utils;
using Swiftly_Teemo.Menu;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Swiftly_Teemo.Draw
{
    internal class Drawings
    {
        public static void OnDraw(EventArgs args)
        {
            if (GameObjects.Player.IsDead)
            {
                return;
            }

            if (MenuConfig.EngageDraw)
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, Spells.Q.Range,
                    Spells.Q.IsReady()
                        ? System.Drawing.Color.DarkSlateGray
                        : System.Drawing.Color.LightGray);
            }

            if (!MenuConfig.DrawR) return;

            var target = Variables.TargetSelector.GetSelectedTarget();

            if (!target.IsValidTarget() || target == null || target.IsDead) return;
            if (!Spells.R.IsReady()) return;

            var rPrediction = Spells.R.GetPrediction(target).UnitPosition;

            Render.Circle.DrawCircle(rPrediction, 75, System.Drawing.Color.GhostWhite);
        }
    }
}
