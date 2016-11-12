using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpAI.SummonersRift.Data;
using SharpAI.Utility;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Utils;
using TreeSharp;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace SharpAI.SummonersRift.Logic
{
    public static class Fight
    {
        static bool ShouldTakeAction()
        {
            if (
                ObjectManager.Get<AIHeroClient>()
                    .Count(e => e.IsEnemy && !e.IsDead && e.Distance(ObjectManager.Player) < 1400) >= 2)
            {
                return false;
            }
            if (ObjectManager.Get<AIHeroClient>()
                .Any(h => h.IsEnemy && !h.IsDead && h.IsVisible && h.Distance(ObjectManager.Player) < 1650) &&
                ObjectManager.Get<AIHeroClient>()
                    .Count(h => h.IsAlly && !h.IsDead && h.Distance(ObjectManager.Player) < 1650) >
                ObjectManager.Get<AIHeroClient>()
                    .Count(h => h.IsEnemy && !h.IsDead && h.IsVisible && h.Distance(ObjectManager.Player) < 1650))
            {
                return true;
            }
            var orbwalkerTarget = Variables.Orbwalker.GetTarget();
            if (orbwalkerTarget != null)
            {
                if (

                    orbwalkerTarget is AIHeroClient)
                {
                    var target = Variables.Orbwalker.GetTarget() as AIHeroClient;
                    return (target != null && ObjectManager.Player.HealthPercent >= target.HealthPercent*2 ||
                            target.HealthPercent < 10) &&
                           !target.IsUnderEnemyTurret();
                }
            }
            return false;
        }

        static TreeSharp.Action TakeAction()
        {
            return new TreeSharp.Action(a =>
            {
                Logging.Log("SWITCHED MODE TO FIGHT");
                Variables.Orbwalker.GetTarget()
                        .Position.Extend(ObjectManager.Player.Position,
                            ObjectManager.Player.GetRealAutoAttackRange() - 250)
                        .WalkToPoint(OrbwalkingMode.Combo);
            });
        }

        public static Composite BehaviorComposite => new Decorator(t => ShouldTakeAction(), TakeAction());
    }
}
