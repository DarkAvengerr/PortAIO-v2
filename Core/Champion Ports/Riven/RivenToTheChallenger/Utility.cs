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
 namespace RivenToTheChallenger
{
    class Utility
    {
        private Config config;
        public Utility(Config config)
        {
            this.config = config;
            Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;
        }

        private void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {

            if (sender.IsMe && args.Animation == "Spell1a" || args.Animation == "Spell1b" || args.Animation == "Spell1c")
            {
                if (args.Animation == "Spell1c")
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(
                        (int) (0.4 + 1/(ObjectManager.Player.AttackSpeedMod*3.75)*1000),
                        () =>
                        {
                            EloBuddy.Player.DoEmote(Emote.Laugh);
                            var target = TargetSelector.GetTarget(ObjectManager.Player.AttackRange,
                                TargetSelector.DamageType.Physical);
                            Orbwalking.ResetAutoAttackTimer();
                            config.Orbwalker.SetOrbwalkingPoint(target?.ServerPosition ?? Game.CursorPos);
                            if (target != null)
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                            }
                            Console.WriteLine("Joke cause funny");
                        });
                }

                /* var target = this.Q.GetTarget();
                 if (target.IsValidTarget() && ObjectManager.Player.Distance(target) < 500 + target.BoundingRadius)
                 {

                 }*/
            }
        }
    }
}
