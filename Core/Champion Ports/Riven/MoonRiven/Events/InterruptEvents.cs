using EloBuddy; 
using LeagueSharp.Common; 
namespace MoonRiven
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class InterruptEvents : Logic
    {
        internal static void Init()
        {
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
        }

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (!MenuInit.InterruptW || sender == null || !sender.IsEnemy || Args == null || !W.IsReady())
            {
                return;
            }

            if (sender.IsValidTarget(W.Range) && Args.DangerLevel == Interrupter2.DangerLevel.High)
            {
                W.Cast(true);
            }
        }
    }
}