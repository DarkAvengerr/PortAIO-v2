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
    public static class BadaoEliseJungleClear
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
            if (BadaoEliseSpellsManager.IsHuman)
            {
                if (BadaoEliseHelper.UseQJungleClear())
                {
                    foreach (Obj_AI_Minion minion in MinionManager.GetMinions(BadaoMainVariables.Q.Range,
                                                                 MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.Health))
                    {
                        BadaoMainVariables.Q.Cast(minion);
                    }
                }
                if (BadaoEliseHelper.UseWJungleClear())
                {
                    foreach (Obj_AI_Minion minion in MinionManager.GetMinions(BadaoMainVariables.W.Range,
                                                                 MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.Health))
                    {
                        BadaoMainVariables.W.Cast(minion);
                    }
                }
            }
            if (!BadaoEliseSpellsManager.IsHuman)
            {
                if (BadaoEliseHelper.CanUseQSpider())
                {
                    foreach (Obj_AI_Minion minion in MinionManager.GetMinions(BadaoMainVariables.Q2.Range,
                                                                 MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.Health))
                    {
                        BadaoMainVariables.Q.Cast(minion);
                    }
                }
            }
            if (BadaoEliseHelper.UseRJungleClear())
            {
                if (BadaoEliseSpellsManager.IsHuman && !BadaoEliseHelper.UseWJungleClear() && (!BadaoMainVariables.Q.IsReady() ||
                    !BadaoEliseVariables.JungleQ.GetValue<bool>()))
                {
                    BadaoMainVariables.R.Cast();
                }
            }
        }
    }
}
