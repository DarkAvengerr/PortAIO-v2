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

    internal class Clear
    {
        private static void CastW()
        {
            var Minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(W.Range)).ToList();
            var FarmPos = W.GetCircularFarmLocation(Minions, W.Width);
            var MinHit = FarmPos.MinionsHit;

            if (W.IsReady())
            {
                if (MinHit >= LCM_W_H)
                {
                    W.Cast(FarmPos.Position);
                }
            }
        }

        private static void CastE()
        {
            var Minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(E.Range)).ToList();

            if (E.IsReady())
            {
                foreach (var Minion in Minions.Where(x => x.HasBuff("brandablaze") && CountMinionsInRange(400, x.Position) >= 3))
                {
                    E.Cast(Minion);
                }
            }
        }

        public static void Execute()
        {
            if (MyHero.ManaPercent >= LCM_M_SB_S && LCM_M_SB_B)
            {
                if (LCM_W)
                {
                    CastW();
                }
                if (LCM_E)
                {
                    CastE();
                }
            }
            else if (!LCM_M_SB_B)
            {
                if (LCM_W)
                {
                    CastW();
                }
                if (LCM_E)
                {
                    CastE();
                }
            }
        }
    }
}
