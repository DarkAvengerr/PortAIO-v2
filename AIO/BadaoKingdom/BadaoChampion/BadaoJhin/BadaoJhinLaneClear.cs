using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoJhin
{
    public static class BadaoJhinLaneClear
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;
            if (BadaoMainVariables.R.Instance.SData.Name == "JhinRShot")
                return;
            if (!BadaoJhinHelper.CanLaneClearMana())
                return;
            if (BadaoJhinHelper.UseQLane())
            {
                var info = BadaoJhinHelper.GetQInfo();
                var target = info.Where(x => x.BounceTargets.Count() >= 3)
                    .OrderByDescending(x => x.DeathCount).FirstOrDefault();
                if (target != null)
                {
                    BadaoMainVariables.Q.Cast(target.QTarget);
                }
            }
        }
    }
}
