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
 namespace BadaoKingdom.BadaoChampion.BadaoVeigar
{
    public static class BadaoVeigarLaneClear
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;   
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;
            if (BadaoMainVariables.Q.IsReady())
            {
                var minions = MinionManager.GetMinions(BadaoMainVariables.Q.Range).Where(x => x.IsMinion);
                Obj_AI_Base minion = null;
                int count = 0;
                foreach (var x in minions.Where(x => BadaoVeigarHelper.GetQDamage(x) >= x.Health))
                {
                    var pred = BadaoMainVariables.Q.GetPrediction(x);
                    var pred2 = BadaoMainVariables.Q2.GetPrediction(x);
                    List<Obj_AI_Base> pred3 = new List<Obj_AI_Base>();
                    foreach (var obj in pred.CollisionObjects)
                    {
                        if (!pred3.Any(a => a.NetworkId == obj.NetworkId))
                            pred3.Add(obj);
                    }
                    if (pred2.Hitchance >= HitChance.Medium && pred3.Count() <= 1)
                    {
                        minion = x;
                        count = 1;
                        if (pred3.Count() == 1 && BadaoVeigarHelper.GetQDamage(pred3.First()) > pred3.First().Health)
                        {
                            count = 2;
                            break;
                        }
                    }
                }
                if (minion != null && count >= BadaoVeigarVariables.ClearCount.GetValue<Slider>().Value)
                    BadaoMainVariables.Q2.Cast(minion);
            }
        }
    }
}
