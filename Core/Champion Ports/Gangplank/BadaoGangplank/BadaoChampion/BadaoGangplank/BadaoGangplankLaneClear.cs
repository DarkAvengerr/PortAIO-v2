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
namespace BadaoKingdom.BadaoChampion.BadaoGangplank
{
    public static class BadaoGangplankLaneClear
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;
            if (!BadaoGangplankVariables.LaneQ.GetValue<bool>())
                return;
            foreach (Obj_AI_Minion minion in MinionManager.GetMinions(BadaoMainVariables.Q.Range).OrderBy(x => x.Health))
            {
                if (minion.BadaoIsValidTarget() && BadaoMainVariables.Q.GetDamage(minion) >= minion.Health && !(Orbwalking.InAutoAttackRange(minion)))
                {
                    BadaoMainVariables.Q.Cast(minion);
                }
            }
        }
    }
}
