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
    public static class Clear
    {
        public static AIHeroClient Player { get{ return ObjectManager.Player; } }
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
        }

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;

            if (Player.Mana < 5 || 
                (Player.Mana == 5 && 
                ((!Variables.LaneSave.GetValue<bool>() && target.IsValidTarget() && target.Team != GameObjectTeam.Neutral) 
                || (!Variables.JungSave.GetValue<bool>() && target.IsValidTarget() && target.Team == GameObjectTeam.Neutral)))
                )
            {
                if (Variables.Q.IsReady() 
                    && ((Variables.LaneQ.GetValue<bool>() && target.IsValidTarget() && target.Team != GameObjectTeam.Neutral) 
                    || (Variables.JungQ.GetValue<bool>() && target.IsValidTarget() && target.Team == GameObjectTeam.Neutral)))
                {
                    Variables.Q.Cast();
                }
                else
                {
                    if (Helper.HasItem() &&
                        (Variables.LaneTiamat.GetValue<bool>() && target.IsValidTarget() && target.Team != GameObjectTeam.Neutral)
                        ||(Variables.JungTiamat.GetValue<bool>() && target.IsValidTarget() && target.Team == GameObjectTeam.Neutral))
                        Helper.CastItem();
                }
            }
            else
            {
                if (Helper.HasItem() &&
                    (Variables.LaneTiamat.GetValue<bool>() && target.IsValidTarget() && target.Team != GameObjectTeam.Neutral)
                    || (Variables.JungTiamat.GetValue<bool>() && target.IsValidTarget() && target.Team == GameObjectTeam.Neutral))
                    Helper.CastItem();
            }
       
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;

            if ((Player.Mana < 5))
            {
                var targetW1 = MinionManager.GetMinions(Player.Position, 500, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health).FirstOrDefault();
                var targetE1 = MinionManager.GetMinions(Player.Position, Variables.E.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health).FirstOrDefault();
                var targetW2 = MinionManager.GetMinions(Player.Position, 500, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
                var targetE2 = MinionManager.GetMinions(Player.Position, Variables.E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
                if (Variables.W.IsReady() && targetW1 != null && Variables.LaneW.GetValue<bool>())
                {
                    Variables.W.Cast(targetW1);
                }
                if (Variables.W.IsReady() && targetW2 != null && Variables.JungW.GetValue<bool>())
                {
                    Variables.W.Cast(targetW2);
                }
                if (Variables.E.IsReady() && targetE1 != null && Variables.LaneE.GetValue<bool>())
                {
                    Helper.CastE(targetE1);
                }
                if (Variables.E.IsReady() && targetE2 != null && Variables.JungE.GetValue<bool>())
                {
                    Helper.CastE(targetE2);
                }
            }
        }
    }
}
