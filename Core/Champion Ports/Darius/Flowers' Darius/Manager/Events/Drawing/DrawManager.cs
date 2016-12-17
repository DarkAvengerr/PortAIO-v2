using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Darius.Manager.Events
{
    using System;
    using FlowersDariusCommon;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Color = System.Drawing.Color;

    internal class DrawManager : Logic
    {
        internal static void Init(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (Menu.GetBool("DrawQ") && (Q.IsReady() || Me.HasBuff("dariusqcast")))
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.FromArgb(253, 164, 17), 1);
                }

                if (Menu.GetBool("DrawE") && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(143, 16, 146), 1);
                }

                if (Menu.GetBool("DrawR") && R.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, R.Range, Color.FromArgb(25, 213, 255), 1);
                }

                if (Menu.GetBool("DrawRStatus") && R.Level > 0)
                {
                    var useRCombo = Menu.Item("ComboR", true).GetValue<KeyBind>();
                    var MePos = Drawing.WorldToScreen(Me.Position);

                    Drawing.DrawText(MePos[0] - 40, MePos[1] + 25, Color.FromArgb(0, 168, 255),
                        "Use R(" + new string(System.Text.Encoding.Default.GetChars(BitConverter.GetBytes(useRCombo.Key))));
                    Drawing.DrawText(MePos[0] + 17, MePos[1] + 25, Color.FromArgb(0, 168, 255),
                        "): " + (useRCombo.Active ? "On" : "Off"));
                }
            }
        }
    }
}