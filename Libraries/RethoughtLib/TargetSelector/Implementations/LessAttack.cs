using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.TargetSelector.Implementations
{
    using global::RethoughtLib.TargetSelector.Interfaces;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class LessAttack : ITargetSelectionMode
    {
        public string Name { get; set; } = "Less Attack";

        public AIHeroClient GetTarget(List<AIHeroClient> targets, AIHeroClient requester)
        {
            var results = new Dictionary<AIHeroClient, double>();

            foreach (var target in targets)
            {
                var targetHealth = target.Health;

                while (targetHealth > 0)
                {
                    targetHealth -= (float) requester.GetAutoAttackDamage(target);
                }
            }
            return targets.MinOrDefault(x => x.Health / requester.GetAutoAttackDamage(x, true));
        }
    }
}
