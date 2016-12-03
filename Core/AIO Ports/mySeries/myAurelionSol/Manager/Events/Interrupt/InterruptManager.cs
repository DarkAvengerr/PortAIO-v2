using EloBuddy; 
using LeagueSharp.Common; 
namespace myAurelionSol.Manager.Events
{
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class InterruptManager : Logic
    {
        internal static void Init(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (Args.DangerLevel >= Interrupter2.DangerLevel.Medium)
            {
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

                if (Args.DangerLevel == Interrupter2.DangerLevel.High)
                {
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
    }
}