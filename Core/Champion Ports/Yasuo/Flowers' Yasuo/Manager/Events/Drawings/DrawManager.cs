using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Yasuo.Manager.Events
{
    using System;
    using System.Linq;
    using Spells;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Color = System.Drawing.Color;
    using Games.Mode;

    internal class DrawManager : Logic
    {
        internal static void Init(EventArgs args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (Menu.Item("DrawQ", true).GetValue<bool>() && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.FromArgb(17, 245, 224), 3);
                }

                if (Menu.Item("DrawQ3", true).GetValue<bool>() && SpellManager.HaveQ3)
                {
                    Render.Circle.DrawCircle(Me.Position, Q3.Range, Color.FromArgb(0, 149, 255), 3);
                }

                if (Menu.Item("DrawW", true).GetValue<bool>() && W.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, W.Range, Color.FromArgb(249, 21, 237), 3);
                }

                if (Menu.Item("DrawE", true).GetValue<bool>() && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(51, 254, 216), 3);
                }

                if (Menu.Item("DrawR", true).GetValue<bool>() && R.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, R.Range, Color.FromArgb(247, 10, 10), 3);
                }

                if (Menu.Item("DrawSpots", true).GetValue<bool>())
                {
                    foreach (var pos in WallJump.WallJumpPos.Where(x => x.Distance(Me) <= 1200))
                    {
                        Render.Circle.DrawCircle(pos.To3D(), 150, Color.FromArgb(251, 209, 0), 1);
                    }
                }

                if (Menu.Item("DrawStackQ", true).GetValue<bool>() && Q.Level > 0)
                {
                    var stackQ = Menu.Item("StackQ", true).GetValue<KeyBind>();
                    var MePos = Drawing.WorldToScreen(Me.Position);

                    Drawing.DrawText(MePos[0] - 40, MePos[1] + 25, Color.MediumSlateBlue,
                        "Stack Q(" + new string(System.Text.Encoding.Default.GetChars(BitConverter.GetBytes(stackQ.Key))));
                    Drawing.DrawText(MePos[0] + 29, MePos[1] + 25, Color.MediumSlateBlue, "): " + (stackQ.Active ? "On" : "Off"));
                }

                if (Menu.Item("DrawAutoQ", true).GetValue<bool>() && Q.Level > 0)
                {
                    var autoQ = Menu.Item("AutoQ", true).GetValue<KeyBind>();
                    var MePos = Drawing.WorldToScreen(Me.Position);

                    Drawing.DrawText(MePos[0] - 35, MePos[1] + 45, Color.Orange,
                        "Auto Q(" + new string(System.Text.Encoding.Default.GetChars(BitConverter.GetBytes(autoQ.Key))));
                    Drawing.DrawText(MePos[0] + 29, MePos[1] + 45, Color.Orange, "): " + (autoQ.Active ? "On" : "Off"));
                }

                if (Menu.Item("DrawRStatus", true).GetValue<bool>() && R.Level > 0)
                {
                    var comboR = Menu.Item("ComboR", true).GetValue<KeyBind>();
                    var MePos = Drawing.WorldToScreen(Me.Position);

                    Drawing.DrawText(MePos[0] - 50, MePos[1] + 65, Color.PowderBlue,
                        "Combo R(" + new string(System.Text.Encoding.Default.GetChars(BitConverter.GetBytes(comboR.Key))));
                    Drawing.DrawText(MePos[0] + 29, MePos[1] + 65, Color.PowderBlue, "): " + (comboR.Active ? "On" : "Off"));
                }
            }
        }
    }
}
