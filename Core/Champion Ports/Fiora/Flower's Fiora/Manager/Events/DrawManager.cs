using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Fiora.Manager.Events
{
    using Common;
    using Spells;
    using SharpDX;
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Color = System.Drawing.Color;
    using static Common.Common;

    internal class DrawManager : Logic
    {
        internal static void Init(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu.Item("DrawingQ", true).GetValue<bool>() && Q.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, Q.Range, Color.BlueViolet);
            }

            if (Menu.Item("DrawingW", true).GetValue<bool>() && W.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, W.Range, Color.BlueViolet);
            }

            if (Menu.Item("DrawingR", true).GetValue<bool>() && R.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, 450f, Color.BlueViolet);
            }

            if (Menu.Item("DrawingDamage", true).GetValue<bool>())
            {
                foreach (
                    var e in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(e => e.IsValidTarget() && e.IsValid && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = e;
                    HpBarDraw.DrawDmg((float) (ComboDamage(e) + SpellManager.GetPassiveDamage(e)),
                        new ColorBGRA(255, 204, 0, 170));
                }
            }
        }
    }
}