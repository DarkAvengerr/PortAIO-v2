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
namespace BadaoKingdom.BadaoChampion.BadaoKatarina
{
    using static BadaoKatarinaVariables;
    using static BadaoMainVariables;
    public static class BadaoKatarinaLaneClear
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;

            if (Q.IsReady() && LaneClearQ.GetValue<bool>())
            {
                var minionQ = MinionManager.GetMinions(BadaoMainVariables.Q.Range).Where(x => Q.GetDamage(x) >= x.Health).FirstOrDefault();
                if (minionQ != null)
                {
                    Q.Cast(minionQ);
                }
            }
            if (W.IsReady() && LaneClearW.GetValue<bool>())
            {
                var minionWs = MinionManager.GetMinions(300);
                if (minionWs.Count() >= 2)
                {
                    W.Cast();
                }
            }
        }
    }
}
