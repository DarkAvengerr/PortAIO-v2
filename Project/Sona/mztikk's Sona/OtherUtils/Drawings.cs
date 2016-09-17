using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkSona.OtherUtils
{
    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using mztikkSona.Extensions;

    using Config = mztikkSona.Config;

    internal static class Drawings
    {
        #region Public Methods and Operators

        public static void OnDraw(EventArgs args)
        {
            if (Config.IsChecked("drawQ") && (!Config.IsChecked("onlyRdy") || Spells.Q.CanCast()))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.Q.Range, Color.AliceBlue);
            }

            if (Config.IsChecked("drawW") && (!Config.IsChecked("onlyRdy") || Spells.W.CanCast()))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.W.Range, Color.AliceBlue);
            }

            if (Config.IsChecked("drawE") && (!Config.IsChecked("onlyRdy") || Spells.E.CanCast()))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.E.Range, Color.AliceBlue);
            }

            if (Config.IsChecked("drawR") && (!Config.IsChecked("onlyRdy") || Spells.R.CanCast()))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.R.Range, Color.AliceBlue);
            }

            if (Config.IsChecked("drawFR")
                && (!Config.IsChecked("onlyRdy") || (Spells.R.CanCast() && Spells.Flash.CanCast())))
            {
                var rFlashTarget = TargetSelector.GetTarget(Spells.R.Range + 425, TargetSelector.DamageType.Magical);
                if (rFlashTarget != null)
                {
                    var flashPos = ObjectManager.Player.Position.Extend(rFlashTarget.Position, 425);

                    // Drawing.DrawCircle(flashPos, 75, Color.AliceBlue);
                    var flashUltRectangle = new Geometry.Polygon.Rectangle(
                        flashPos, 
                        flashPos.Extend(ObjectManager.Player.Position, -Spells.R.Range), 
                        Spells.R.Width);
                    foreach (var enemy in
                        HeroManager.Enemies.Where(
                            e => !e.HasBuffOfType(BuffType.Invulnerability) && !e.IsDead && e.IsValid &&
                            flashUltRectangle.IsInside(e.Position)
                            ))
                    {
                        Drawing.DrawCircle(enemy.Position, 100, Color.Red);
                        var w2s = Drawing.WorldToScreen(enemy.Position);
                        Drawing.DrawText(w2s.X, w2s.Y, Color.Red, "Flash Ult", 5);
                    }
                }
            }
        }

        #endregion
    }
}