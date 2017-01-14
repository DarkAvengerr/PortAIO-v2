using System;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
using LeagueSharp.Common; 
namespace MoonRiven
{
    internal class OnDrawEvents : Logic
    {
        internal static void Init()
        {
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (MenuInit.DrawW && W.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, W.Range, Color.FromArgb(3, 136, 253), 3);
                }

                if (MenuInit.DrawE && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(249, 21, 237), 3);
                }

                if (MenuInit.DrawRStatus && R.Level > 0)
                {
                    var useRCombo = Menu.Item("ComboR1", true).GetValue<KeyBind>();
                    var MePos = Drawing.WorldToScreen(Me.Position);

                    Drawing.DrawText(MePos[0] - 40, MePos[1] + 25, Color.FromArgb(0, 168, 255),
                        "Use R(" + new string(System.Text.Encoding.Default.GetChars(BitConverter.GetBytes(useRCombo.Key))));
                    Drawing.DrawText(MePos[0] + 17, MePos[1] + 25, Color.FromArgb(0, 168, 255),
                        "): " + (useRCombo.Active ? "On" : "Off"));
                }

                if (MenuInit.DrawBurst && R.Level > 0)
                {
                    var switchKey = Menu.Item("BurstSwitch", true).GetValue<KeyBind>();
                    var burstMode = Menu.Item("BurstMode", true).GetValue<StringList>().SelectedValue;
                    var MePos = Drawing.WorldToScreen(Me.Position);

                    Drawing.DrawText(MePos[0] - 78, MePos[1] + 45, Color.FromArgb(0, 168, 255),
                        "Burst Mode(" + new string(System.Text.Encoding.Default.GetChars(BitConverter.GetBytes(switchKey.Key))));
                    Drawing.DrawText(MePos[0] + 22, MePos[1] + 45, Color.FromArgb(0, 168, 255),
                        "): " + burstMode);
                }
            }
        }
    }
}