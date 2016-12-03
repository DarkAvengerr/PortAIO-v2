using EloBuddy; 
using LeagueSharp.Common; 
namespace myAurelionSol.Manager.Events.Games.Mode
{
    using myCommon;
    using System.Linq;
    using LeagueSharp.Common;

    internal class KillSteal : Logic
    {
        internal static void Init()
        {
            foreach (var e in HeroManager.Enemies.Where(em => em.IsValidTarget() && !em.IsZombie && !em.IsDead))
            {
                if (Q.IsReady() && Menu.GetBool("KillStealQ"))
                {
                    if (e.Health + e.MagicShield + 50 < Q.GetDamage(e))
                    {
                        var qPred = Q.GetPrediction(e, true);

                        if (qPred.Hitchance >= HitChance.VeryHigh || qPred.Hitchance == HitChance.Immobile)
                        {
                            Q.Cast(qPred.CastPosition, true);
                        }
                    }
                }

                if (R.IsReady() && Menu.GetBool("KillStealR"))
                {
                    if (e.Health + e.MagicShield + 50 < R.GetDamage(e))
                    {
                        var rPred = R.GetPrediction(e, true);

                        if (rPred.Hitchance >= HitChance.VeryHigh || rPred.Hitchance == HitChance.Immobile)
                        {
                            R.Cast(rPred.CastPosition, true);
                        }
                    }
                }
            }
        }
    }
}
