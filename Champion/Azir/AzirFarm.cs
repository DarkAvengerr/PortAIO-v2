using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using System.Text.RegularExpressions;
using Color = System.Drawing.Color;
using EloBuddy;

namespace HeavenStrikeAzir
{
    public static class AzirFarm
    {
        public static void Initialize()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Program._orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LastHit &&
                Program._orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;
            if (OrbwalkCommands.CanDoAttack())
            {
                var minion = OrbwalkCommands.GetClearMinionsAndBuildings();
                if (minion.LSIsValidTarget())
                {
                    OrbwalkCommands.AttackTarget(minion);
                }
            }
        }
    }
}
