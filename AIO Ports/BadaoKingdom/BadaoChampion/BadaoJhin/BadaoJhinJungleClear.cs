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
    public static class BadaoJhinJungleClear
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
            if (!BadaoJhinHelper.CanJungleClearMana())
                return;
            if (BadaoJhinHelper.UseQJungle())
            {
                var minion = MinionManager.GetMinions(BadaoMainVariables.Q.Range, MinionTypes.All, MinionTeam.Neutral
                                        , MinionOrderTypes.MaxHealth).FirstOrDefault();
                if (minion != null)
                {
                    BadaoMainVariables.Q.Cast(minion);
                }
            }
            if (BadaoJhinHelper.UseEJungle())
            {
                var minion = MinionManager.GetMinions(BadaoMainVariables.E.Range, MinionTypes.All, MinionTeam.Neutral
                                        , MinionOrderTypes.MaxHealth).FirstOrDefault();
                if (minion != null)
                {
                    BadaoMainVariables.E.Cast(minion.Position);
                }
            }
            if (BadaoJhinHelper.UseWJungle())
            {
                var minion = MinionManager.GetMinions(BadaoMainVariables.W.Range, MinionTypes.All, MinionTeam.Neutral
                                        , MinionOrderTypes.MaxHealth).FirstOrDefault();
                if (minion != null)
                {
                    BadaoMainVariables.W.Cast(minion);
                }
            }
        }
    }
}
