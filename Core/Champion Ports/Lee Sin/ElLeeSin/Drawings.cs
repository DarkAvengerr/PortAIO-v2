using EloBuddy; 
using LeagueSharp.Common; 
namespace ElLeeSin
{
    using System;

    using ElLeeSin.Components;
    using ElLeeSin.Components.SpellManagers;
    using ElLeeSin.Utilities;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    public class Drawings
    {
        #region Public Methods and Operators

        public static void OnDraw(EventArgs args)
        {
            var newTarget = Misc.GetMenuItem("insecMode")
                                ? (TargetSelector.GetSelectedTarget()
                                   ?? TargetSelector.GetTarget(
                                       LeeSin.spells[LeeSin.Spells.Q].Range,
                                       TargetSelector.DamageType.Physical))
                                : TargetSelector.GetTarget(
                                    LeeSin.spells[LeeSin.Spells.Q].Range,
                                    TargetSelector.DamageType.Physical);

            if (InsecManager.ClicksecEnabled && Misc.GetMenuItem("clickInsec"))
            {
                Render.Circle.DrawCircle(InsecManager.InsecClickPos, 100, Color.DeepSkyBlue);
            }

            var playerPos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            if (Misc.GetMenuItem("ElLeeSin.Draw.Insec.Text"))
            {
                Drawing.DrawText(playerPos.X, playerPos.Y + 40, Color.White, "Flash Insec enabled");
            }

            if (Misc.GetMenuItem("Draw.Insec.Lines"))
            {
                if ((newTarget != null) && newTarget.IsVisible && newTarget.IsValidTarget() && !newTarget.IsDead
                    && (ObjectManager.Player.Distance(newTarget) < 3000))
                {
                    Vector2 targetPos = Drawing.WorldToScreen(newTarget.Position);
                    Drawing.DrawLine(
                        InsecManager.InsecLinePos.X,
                        InsecManager.InsecLinePos.Y,
                        targetPos.X,
                        targetPos.Y,
                        3,
                        Color.Gold);

                    Drawing.DrawText(
                        Drawing.WorldToScreen(newTarget.Position).X - 40,
                        Drawing.WorldToScreen(newTarget.Position).Y + 10,
                        Color.White,
                        "Selected Target");

                    Drawing.DrawCircle(InsecManager.GetInsecPos(newTarget), 100, Color.DeepSkyBlue);
                }
            }

            if (!Misc.GetMenuItem("DrawEnabled"))
            {
                return;
            }

            foreach (var t in ObjectManager.Get<AIHeroClient>())
            {
                if (t.HasBuff("BlindMonkQOne") || t.HasBuff("blindmonkqonechaos"))
                {
                    Drawing.DrawCircle(t.Position, 200, Color.Red);
                }
            }

            if (MyMenu.Menu.Item("ElLeeSin.Wardjump").GetValue<KeyBind>().Active
                && Misc.GetMenuItem("ElLeeSin.Draw.WJDraw"))
            {
                Render.Circle.DrawCircle(Wardmanager.JumpPos.To3D(), 20, Color.Red);
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 600, Color.Red);
            }
            if (Misc.GetMenuItem("ElLeeSin.Draw.Q"))
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    LeeSin.spells[LeeSin.Spells.Q].Range - 80,
                    LeeSin.spells[LeeSin.Spells.Q].IsReady() ? Color.LightSkyBlue : Color.Tomato);
            }
            if (Misc.GetMenuItem("ElLeeSin.Draw.W"))
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    LeeSin.spells[LeeSin.Spells.W].Range - 80,
                    LeeSin.spells[LeeSin.Spells.W].IsReady() ? Color.LightSkyBlue : Color.Tomato);
            }
            if (Misc.GetMenuItem("ElLeeSin.Draw.E"))
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    LeeSin.spells[LeeSin.Spells.E].Range - 80,
                    LeeSin.spells[LeeSin.Spells.E].IsReady() ? Color.LightSkyBlue : Color.Tomato);
            }
            if (Misc.GetMenuItem("ElLeeSin.Draw.R"))
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    LeeSin.spells[LeeSin.Spells.R].Range - 80,
                    LeeSin.spells[LeeSin.Spells.R].IsReady() ? Color.LightSkyBlue : Color.Tomato);
            }
        }

        #endregion
    }
}