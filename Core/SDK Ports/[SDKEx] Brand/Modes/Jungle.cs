using SharpDX;

using EloBuddy; 
using LeagueSharp.SDK; 
namespace Brand.Modes
{
    using System.Linq;

    using LeagueSharp.SDK;

    using static Brand.Extensions.Config;
    using static Brand.Extensions.Spells;
    using static Brand.Extensions.Other;

    internal class Jungle
    {
        private static void CastQ()
        {
            var Minions = GameObjects.JungleLarge.Where(x => x.IsValidTarget(Q.Range)).ToList();

            if (Q.IsReady())
            {
                foreach (var Minion in Minions)
                {
                    Q.Cast(Minion);
                }
            }
        }

        private static void CastW()
        {
            var Minions = GameObjects.JungleLarge.Where(x => x.IsValidTarget(W.Range)).ToList();

            if (W.IsReady())
            {
                foreach (var Minion in Minions)
                {
                    W.Cast(Minion);
                }
            }
        }

        private static void CastE()
        {
            var Minions = GameObjects.JungleLarge.Where(x => x.IsValidTarget(E.Range) && x.HasBuff("brandablaze")).ToList();

            if (E.IsReady())
            {
                foreach (var Minion in Minions)
                {
                    E.Cast(Minion);
                }
            }
        }

        public static void Execute()
        {
            if (MyHero.ManaPercent >= JCM_M_SB_S && JCM_M_SB_B)
            {
                if (JCM_E)
                {
                    CastE();
                }
                if (JCM_Q)
                {
                    CastQ();
                }
                if (JCM_W)
                {
                    CastW();
                }
            }
            else if (!JCM_M_SB_B)
            {
                if (JCM_E)
                {
                    CastE();
                }
                if (JCM_Q)
                {
                    CastQ();
                }
                if (JCM_W)
                {
                    CastW();
                }
            }
        }
    }
}
