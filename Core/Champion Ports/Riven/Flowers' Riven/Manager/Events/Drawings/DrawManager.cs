using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Events
{
    using System;
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Color = System.Drawing.Color;

    internal class DrawManager : Logic
    {
        internal static void Init(EventArgs Args)
        {
            if (Menu.GetBool("DrawW") && W.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, W.Range, Color.FromArgb(3, 136, 253), 3);
            }

            if (Menu.GetBool("DrawBurst") && R.Level > 0)
            {
                Render.Circle.DrawCircle(Me.Position, 425 + E.Range, Color.FromArgb(3, 136, 253), 3);
            }

            if (Menu.GetBool("DrawRStatus") && R.Level > 0)
            {
                var useRCombo = Menu.Item("R1Combo", true).GetValue<KeyBind>();
                var MePos = Drawing.WorldToScreen(Me.Position);

                Drawing.DrawText(MePos[0] - 40, MePos[1] + 25, Color.FromArgb(0, 168, 255),
                    "Use R(" + new string(System.Text.Encoding.Default.GetChars(BitConverter.GetBytes(useRCombo.Key))));
                Drawing.DrawText(MePos[0] + 17, MePos[1] + 25, Color.FromArgb(0, 168, 255),
                    "): " + (useRCombo.Active ? "On" : "Off"));
            }
        }
    }
}