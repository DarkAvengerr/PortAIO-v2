using EloBuddy; 
using LeagueSharp.Common; 
namespace myAnnie.Manager.Events
{
    using Spells;
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class InterruptManager : Logic
    {
        internal static void Init(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (Args.DangerLevel >= Interrupter2.DangerLevel.Medium)
            {
                if (!SpellManager.HaveStun && SpellManager.BuffCounts == 3 && E.IsReady() && Menu.GetBool("InterruptE"))
                {
                    E.Cast();
                }

                if (SpellManager.HaveStun)
                {
                    if (Q.IsReady() && Menu.GetBool("InterruptQ") && sender.IsValidTarget(Q.Range))
                    {
                        Q.Cast(sender, true);
                    }
                    else if (W.IsReady() && Menu.GetBool("InterruptW") && sender.IsValidTarget(W.Range))
                    {
                        W.Cast(sender, true);
                    }
                }
            }
        }
    }
}