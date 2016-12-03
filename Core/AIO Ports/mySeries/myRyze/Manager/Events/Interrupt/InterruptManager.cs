using EloBuddy; 
using LeagueSharp.Common; 
namespace myRyze.Manager.Events
{
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class InterruptManager : Logic
    {
        internal static void Init(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (Menu.GetBool("Interrupt") && E.IsReady() && sender.IsEnemy && sender.IsValidTarget(W.Range))
            {
                if (Args.DangerLevel >= Interrupter2.DangerLevel.High)
                {
                    W.CastOnUnit(sender, true);
                }
            }
        }
    }
}