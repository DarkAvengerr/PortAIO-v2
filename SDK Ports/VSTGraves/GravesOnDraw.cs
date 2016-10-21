using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace VST_Auto_Carry_Standalone_Graves
{
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using System;

    internal class GravesOnDraw : Graves
    {
        internal static void Init(EventArgs args)
        {
            if (Me.IsDead)
                return;

            if (Menu["Draw"]["Q"].GetValue<MenuBool>() && Q.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.Red);
            }

            if (Menu["Draw"]["W"].GetValue<MenuBool>() && W.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.Orange);
            }

            if (Menu["Draw"]["E"].GetValue<MenuBool>() && E.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.LightSeaGreen);
            }

            if (Menu["Draw"]["R"].GetValue<MenuBool>() && R.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, R.Range, System.Drawing.Color.YellowGreen);
            }
        }
    }
}