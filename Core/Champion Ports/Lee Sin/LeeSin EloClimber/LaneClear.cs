using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace LeeSin_EloClimber
{

    internal class LaneClear
    {
        public static List<Obj_AI_Base> enemyMinion;

        internal static void Load()
        {
            // Callback
            Game.OnUpdate += Update;
        }

        private static void Update(EventArgs args)
        {
            if (LeeSin.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;

            enemyMinion = MinionManager.GetMinions(LeeSin.Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.None);
            if (enemyMinion.Count() != 0 && enemyMinion[0].IsValidTarget(LeeSin.Q.Range, true, LeeSin.myHero.Position))
            {
                var target = enemyMinion[0];
                double Q1_dmg = 0;
                double Q2_dmg = 0;

                if (LeeSin.Q.IsReady())
                {
                    Q1_dmg = LeeSin.GetDamage_Q1(target);
                    Q2_dmg = LeeSin.GetDamage_Q2(target, Q1_dmg);
                }

                if (MenuManager.myMenu.Item("lane.useE").GetValue<Boolean>() && LeeSin.E.IsReady() && LeeSin.IsSecondCast(LeeSin.E)) // Second E
                {
                    if (LeeSin.PassiveStack < 1)
                    {
                        LeeSin.E.Cast();
                        return;
                    }
                }
                if (MenuManager.myMenu.Item("lane.useW").GetValue<Boolean>() && LeeSin.W.IsReady() && LeeSin.IsSecondCast(LeeSin.W)) // Second W
                {
                    if (LeeSin.PassiveStack < 1)
                    {
                        LeeSin.W.Cast();
                        return;
                    }
                }
                if (MenuManager.myMenu.Item("lane.useQ").GetValue<Boolean>() && LeeSin.Q.IsReady() && LeeSin.IsSecondCast(LeeSin.Q)) // Second Q
                {
                    if (LeeSin.PassiveStack < 1 || Q2_dmg > target.Health || LeeSin.myHero.Distance(target) > 500)
                    {
                        LeeSin.Q.Cast();
                        return;
                    }

                }
                if (MenuManager.myMenu.Item("lane.useE").GetValue<Boolean>() && LeeSin.E.IsReady() && !LeeSin.IsSecondCast(LeeSin.E) && LeeSin.myHero.Distance(target) < LeeSin.E.Range) // First E
                {
                    if (LeeSin.PassiveStack < 1 || LeeSin.E.CountHits(enemyMinion, LeeSin.myHero.Position) > MenuManager.myMenu.Item("lane.countE").GetValue<Slider>().Value)
                    {
                        LeeSin.E.Cast();
                        return;
                    }
                }
                if (MenuManager.myMenu.Item("lane.useW").GetValue<Boolean>() && LeeSin.W.IsReady() && !LeeSin.IsSecondCast(LeeSin.W) && LeeSin.myHero.Distance(target) < LeeSin.E.Range) // First W
                {
                    if (LeeSin.PassiveStack < 1)
                    {
                        LeeSin.W.Cast();
                        return;
                    }
                }
                if (MenuManager.myMenu.Item("lane.useQ").GetValue<Boolean>() && LeeSin.Q.IsReady() && !LeeSin.IsSecondCast(LeeSin.Q)) // First Q
                {
                    if (LeeSin.PassiveStack < 1 || (Q1_dmg + Q2_dmg) > target.Health)
                    {
                        LeeSin.Q.Cast(target.Position);
                        return;
                    }
                }
            }
        }
    }
}
