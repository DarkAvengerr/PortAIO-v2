namespace ElLeeSin
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using EloBuddy;

    public class Drawings
    {
        #region Public Methods and Operators

        public static void OnDraw(EventArgs args)
        {
            var newTarget = Program.ParamBool("insecMode")
                                ? TargetSelector.GetSelectedTarget()
                                : TargetSelector.GetTarget(
                                    Program.spells[Program.Spells.Q].Range + 200,
                                    TargetSelector.DamageType.Physical);


            if (Program.ClicksecEnabled && Program.ParamBool("clickInsec"))
            {
                Render.Circle.DrawCircle(Program.InsecClickPos, 100, Color.DeepSkyBlue);
            }

            var playerPos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            if (Program.ParamBool("ElLeeSin.Draw.Insec.Text"))
            {
                Drawing.DrawText(playerPos.X, playerPos.Y + 40, Color.White, "Flash Insec enabled");
            }

            if (Program.ParamBool("Draw.Insec.Lines")) //&& Program.spells[Program.Spells.R].IsReady()
            {
                if (newTarget != null && newTarget.IsVisible && newTarget.LSIsValidTarget() && !newTarget.IsDead && Program.Player.Distance(newTarget) < 3000)
                {
                    Vector2 targetPos = Drawing.WorldToScreen(newTarget.Position);
                    Drawing.DrawLine(
                        Program.InsecLinePos.X,
                        Program.InsecLinePos.Y,
                        targetPos.X,
                        targetPos.Y,
                        3,
                        Color.Gold);

                    Drawing.DrawText(
                        Drawing.WorldToScreen(newTarget.Position).X - 40,
                        Drawing.WorldToScreen(newTarget.Position).Y + 10,
                        Color.White,
                        "Selected Target");

                    Drawing.DrawCircle(Program.GetInsecPos(newTarget), 100, Color.DeepSkyBlue);

                }
            }
    
            if (!Program.ParamBool("DrawEnabled"))
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

            if (InitMenu.Menu.Item("ElLeeSin.Wardjump").GetValue<KeyBind>().Active
                && Program.ParamBool("ElLeeSin.Draw.WJDraw"))
            {
                Render.Circle.DrawCircle(Program.JumpPos.To3D(), 20, Color.Red);
                Render.Circle.DrawCircle(Program.Player.Position, 600, Color.Red);
            }
            if (Program.ParamBool("ElLeeSin.Draw.Q"))
            {
                Render.Circle.DrawCircle(
                    Program.Player.Position,
                    Program.spells[Program.Spells.Q].Range - 80,
                    Program.spells[Program.Spells.Q].LSIsReady() ? Color.LightSkyBlue : Color.Tomato);
            }
            if (Program.ParamBool("ElLeeSin.Draw.W"))
            {
                Render.Circle.DrawCircle(
                    Program.Player.Position,
                    Program.spells[Program.Spells.W].Range - 80,
                    Program.spells[Program.Spells.W].LSIsReady() ? Color.LightSkyBlue : Color.Tomato);
            }
            if (Program.ParamBool("ElLeeSin.Draw.E"))
            {
                Render.Circle.DrawCircle(
                    Program.Player.Position,
                    Program.spells[Program.Spells.E].Range - 80,
                    Program.spells[Program.Spells.E].LSIsReady() ? Color.LightSkyBlue : Color.Tomato);
            }
            if (Program.ParamBool("ElLeeSin.Draw.R"))
            {
                Render.Circle.DrawCircle(
                    Program.Player.Position,
                    Program.spells[Program.Spells.R].Range - 80,
                    Program.spells[Program.Spells.R].LSIsReady() ? Color.LightSkyBlue : Color.Tomato);
            }
        }

        #endregion
    }
}