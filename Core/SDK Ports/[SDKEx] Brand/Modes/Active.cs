using LeagueSharp.SDK;

using EloBuddy; 
using LeagueSharp.SDK; 
namespace Brand.Modes
{
    using System.Linq;

    using static Brand.Extensions.Config;
    using static Brand.Extensions.Spells;
    using static Brand.Extensions.Other;

    internal class Active
    {
        private static void CastQImmobile()
        {
            if (Q.IsReady())
            {
                var Enemies = GameObjects.EnemyHeroes;
                foreach (var Enemy in Enemies.Where(x => x.IsValidTarget(Q.Range) && Immobile(x)))
                {
                    Q.Cast(Enemy);
                }
            }
        }

        private static void CastWImmobile()
        {
            if (W.IsReady())
            {
                var Enemies = GameObjects.EnemyHeroes;
                foreach (var Enemy in Enemies.Where(x => x.IsValidTarget(W.Range) && Immobile(x)))
                {
                    W.Cast(Enemy);
                }
            }
        }

        public static void Execute()
        {
            if (IMM_Q)
            {
                CastQImmobile();
            }
            if (IMM_W)
            {
                CastWImmobile();
            }
        }
    }
}
