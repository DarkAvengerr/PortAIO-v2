using EloBuddy; 
using LeagueSharp.SDK; 
namespace Brand.Modes
{
    using System.Linq;

    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;

    using static Brand.Extensions.Config;
    using static Brand.Extensions.Spells;
    using static Brand.Extensions.Other;

    internal class Killsteal
    {
        private static void CastQ()
        {
            if (Q.IsReady())
            {
                var Enemies = GameObjects.EnemyHeroes.Where(x => x != null && x.IsValidTarget());

                foreach (var Enemy in Enemies.Where(x => x.IsValidTarget(Q.Range) && TotalDamage(x, true, false, false, false) > x.Health))
                {
                    var Predinction = Q.GetPrediction(Enemy);
                    if (Predinction.Hitchance >= HitChance.VeryHigh)
                    {
                        Q.Cast(Predinction.CastPosition);
                    }
                }
            }
        }

        private static void CastW()
        {
            if (W.IsReady())
            {
                var Enemies = GameObjects.EnemyHeroes.Where(x => x != null && x.IsValidTarget());

                foreach (var Enemy in Enemies.Where(x => x.IsValidTarget(W.Range) && TotalDamage(x, false, true, false, false) > x.Health))
                {
                    var Predinction = W.GetPrediction(Enemy);
                    if (Predinction.Hitchance >= HitChance.VeryHigh)
                    {
                        W.Cast(Predinction.CastPosition);
                    }
                }
            }
        }

        private static void CastE()
        {
            if (E.IsReady())
            {
                var Enemies = GameObjects.EnemyHeroes.Where(x => x != null && x.IsValidTarget());

                foreach (var Enemy in Enemies.Where(x => x.IsValidTarget(E.Range) && TotalDamage(x, false, false, true, false) > x.Health))
                {
                    E.Cast(Enemy);
                }
            }
        }

        private static void CastR()
        {
            if (R.IsReady())
            {
                var Enemies = GameObjects.EnemyHeroes.Where(x => x != null && x.IsValidTarget());

                foreach (var Enemy in Enemies.Where(x => x.IsValidTarget(R.Range) && TotalDamage(x, false, false, false, true) > x.Health))
                {
                    R.Cast(Enemy);
                }
            }
        }

        public static void Execute()
        {
            if (KM_Q)
            {
                CastQ();
            }
            if (KM_W)
            {
                CastW();
            }
            if (KM_E)
            {
                CastE();
            }
            if (KM_R)
            {
                CastR();
            }
        }
    }
}
