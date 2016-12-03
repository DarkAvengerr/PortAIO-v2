using EloBuddy; 
using LeagueSharp.Common; 
namespace myDiana.Manager.Events
{
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class InterruptManager : Logic
    {
        internal static void Init(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (Menu.GetBool("EInt") && sender.IsValidTarget(E.Range) && Args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                if (E.IsReady())
                {
                    E.Cast(true);
                }
            }
        }
    }
}