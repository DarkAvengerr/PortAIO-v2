using System;
using System.Linq;
using LeagueSharp;
using VayneHunter_Reborn.Utility.MenuUtility;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace VayneHunter_Reborn.Utility.TrapAvoid
{
    class TrapAvoider
    {
        internal static void OnLoad()
        {
            Drawing.OnDraw += OnDraw;
        }

        internal static bool IsEnabled()
        {
            return MenuExtensions.GetItemValue<bool>("dz191.vhr.draw.trapDraw");
        }

        private static void OnDraw(EventArgs args)
        {
            if (!IsEnabled())
            {
                return;
            }

            if ((int) (Game.Time*10f) % 2 == 0)
            {
                return;
            }

            foreach (var trap in  ObjectManager.Get<Obj_GeneralParticleEmitter>().Where(trap => trap != null && trap.IsValid))
            {
                var distanceToPlayer = ObjectManager.Player.ServerPosition.Distance(trap.Position);
                if (distanceToPlayer > 1200f)
                {
                    continue;
                }

                if (trap.Name.ToLower().Contains("yordleTrap_idle_red.troy".ToLower()))
                {
                   Render.Circle.DrawCircle(trap.Position, 110f, System.Drawing.Color.OrangeRed, 1);
                }
            }

            foreach (var minionObject in ObjectManager.Get<Obj_AI_Minion>().Where(trapObj => trapObj != null && trapObj.IsValid && trapObj.IsEnemy))
            {
                var distanceToPlayer = ObjectManager.Player.ServerPosition.Distance(minionObject.Position);
                if (distanceToPlayer > 1200f)
                {
                    continue;
                }

                if (minionObject.Name.Equals("Noxious Trap"))
                {
                    Render.Circle.DrawCircle(minionObject.Position, 110f, System.Drawing.Color.OrangeRed, 1);
                }

                if (minionObject.Name.Equals("k"))
                {
                     Render.Circle.DrawCircle(minionObject.Position, 110f, System.Drawing.Color.OrangeRed, 1);
                }
            }
        }
    }
}
