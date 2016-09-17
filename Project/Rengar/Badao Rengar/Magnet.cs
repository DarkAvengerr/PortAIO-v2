using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoRengar
{
    public static class Magnet
    {
        public static AIHeroClient MagnetTarget = null;
        public static bool IsDoingMagnet = false;
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (MagnetTarget != null && IsDoingMagnet)
            {
                var x = Drawing.WorldToScreen(ObjectManager.Player.Position);
                var y = Drawing.WorldToScreen(MagnetTarget.Position);
                Drawing.DrawLine(x, y, 1, Color.Yellow);
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!Variables.MagnetEnable.GetValue<bool>())
            {
                MagnetTarget = null;
                IsDoingMagnet = false;
                Variables.Orbwalker._orbwalkingPoint = Game.CursorPos;
                return;
            }

            if (Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo 
                && TargetSelector.GetSelectedTarget().IsValidTarget(Variables.MagnetRange.GetValue<Slider>().Value))
            {
                MagnetTarget = TargetSelector.GetSelectedTarget();
                IsDoingMagnet = true;
                Variables.Orbwalker._orbwalkingPoint = TargetSelector.GetSelectedTarget().Position.Extend(ObjectManager.Player.Position, - 200);
            }
            else if (Variables.AssassinateKey.GetValue<KeyBind>().Active && Assasinate.AssassinateTarget.IsValidTarget(Variables.MagnetRange.GetValue<Slider>().Value))
            {
                MagnetTarget = Assasinate.AssassinateTarget;
                IsDoingMagnet = true;
                Variables.Orbwalker._orbwalkingPoint = Assasinate.AssassinateTarget.Position.Extend(ObjectManager.Player.Position, -200);
            }
            else
            {
                MagnetTarget = null;
                IsDoingMagnet = false;
                Variables.Orbwalker._orbwalkingPoint = Game.CursorPos;
            }
        }
    }
}
