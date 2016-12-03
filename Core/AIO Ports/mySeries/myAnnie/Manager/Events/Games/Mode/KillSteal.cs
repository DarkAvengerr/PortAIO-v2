using EloBuddy; 
using LeagueSharp.Common; 
namespace myAnnie.Manager.Events.Games.Mode
{
    using myCommon;
    using System.Linq;
    using LeagueSharp.Common;

    internal class KillSteal : Logic
    {
        internal static void Init()
        {
            foreach (var e in HeroManager.Enemies.Where(e => !e.IsZombie && !e.IsDead && e.IsValidTarget()))
            {
                if (Q.IsReady() && Menu.GetBool("KillStealQ") && e.Health + e.MagicShield < Q.GetDamage(e) &&
                    e.IsValidTarget(Q.Range))
                {
                    Q.Cast(e, true);
                    return;
                }

                if (W.IsReady() && Menu.GetBool("KillStealW") && e.Health + e.MagicShield < W.GetDamage(e) &&
                    e.IsValidTarget(W.Range))
                {
                    W.Cast(e, true);
                    return;
                }
            }
        }
    }
}
