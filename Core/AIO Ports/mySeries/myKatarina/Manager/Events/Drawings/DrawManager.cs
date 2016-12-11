using EloBuddy; 
using LeagueSharp.Common; 
namespace myKatarina.Manager.Events.Drawings
{
    using System;
    using System.Drawing;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class DrawManager : Logic
    {
        internal static void Init(EventArgs args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (Menu.Item("DrawQ", true).GetValue<bool>() && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.FromArgb(253, 164, 17), 1);
                }

                if (Menu.Item("DrawE", true).GetValue<bool>() && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(143, 16, 146), 1);
                }

                if (Menu.Item("DrawR", true).GetValue<bool>() && R.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, R.Range, Color.FromArgb(25, 213, 255), 1);
                }
            }
        }
    }
}
