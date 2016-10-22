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
 namespace BadaoKingdom.BadaoChampion.BadaoElise
{
    public static class BadaoEliseLaneClear
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
        }

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;
            if (!BadaoEliseSpellsManager.IsHuman && BadaoEliseHelper.CanUseWSpider())
            {
                BadaoMainVariables.W2.Cast();
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;
            if (!BadaoEliseSpellsManager.IsHuman && BadaoEliseHelper.CanUseQSpider())
            {
                foreach (Obj_AI_Minion minion in MinionManager.GetMinions(BadaoMainVariables.Q2.Range).OrderBy(x => x.Health))
                {
                    BadaoMainVariables.Q2.Cast(minion);
                }
            }
        }
    }
}
