using EloBuddy; 
using LeagueSharp.Common; 
namespace myAurelionSol.Manager.Events
{
    using LeagueSharp.Common;
    using myCommon;

    internal class AntiGapcloserManager : Logic
    {
        internal static void Init(ActiveGapcloser Args)
        {
            var sender = Args.Sender;

            if (Q.IsReady() && Menu.GetBool("GapCloserQ"))
            {
                var QPred = Q.GetPrediction(sender);

                if (sender.IsValidTarget(Q.Range))
                {
                    if (QPred.Hitchance >= HitChance.VeryHigh)
                    {
                        Q.Cast(QPred.CastPosition);
                    }
                }
            }

            if (R.IsReady() && Menu.GetBool("GapCloserR"))
            {
                var RPred = R.GetPrediction(sender);

                if (sender.IsValidTarget(R.Range))
                {
                    if (RPred.Hitchance >= HitChance.VeryHigh)
                    {
                        R.Cast(RPred.CastPosition);
                    }
                }
            }
        }
    }
}