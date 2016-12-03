using EloBuddy; 
using LeagueSharp.Common; 
namespace myRyze.Manager.Events.Drawings
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
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (Menu.GetBool("DrawQ") && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.Green, 1);
                }

                if (Menu.GetBool("DrawW") && W.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, W.Range, Color.FromArgb(9, 253, 242), 1);
                }

                if (Menu.GetBool("DrawE") && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(188, 6, 248), 1);
                }

                if (Menu.GetBool("DrawR") && R.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, R.Range, Color.FromArgb(19, 130, 234), 1);
                }

                if (Menu.GetBool("DrawMode"))
                {
                    var comboSwitchKey = Menu.Item("ComboModeSwitch", true).GetValue<KeyBind>();
                    var comboMode = Menu.Item("ComboMode", true).GetValue<StringList>().SelectedValue;
                    var MePos = Drawing.WorldToScreen(Me.Position);

                    Drawing.DrawText(MePos[0] - 80, MePos[1] + 65, Color.Pink,
                        "Combo Mode(" +
                        new string(System.Text.Encoding.Default.GetChars(BitConverter.GetBytes(comboSwitchKey.Key))));
                    Drawing.DrawText(MePos[0] + 29, MePos[1] + 65, Color.Pink, "): " + comboMode);
                }
            }
        }

        internal static void InitMinMap(EventArgs args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
#pragma warning disable 618
                if (Menu.GetBool("DrawRMin") && R.IsReady())
                {
                    LeagueSharp.Common.Utility.DrawCircle(Me.Position, R.Range, Color.FromArgb(14, 194, 255), 1, 30, true);
                }
#pragma warning restore 618
            }
        }
    }
}
