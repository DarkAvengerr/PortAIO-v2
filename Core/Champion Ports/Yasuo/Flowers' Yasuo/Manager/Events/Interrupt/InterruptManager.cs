using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Yasuo.Manager.Events
{
    using Spells;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class InterruptManager : Logic
    {
        internal static void Init(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (Menu.Item("Q3Int", true).GetValue<bool>() && Q3.IsReady() && SpellManager.HaveQ3)
            {
                if (sender != null && sender.IsValidTarget(Q3.Range) && Args.DangerLevel >= Interrupter2.DangerLevel.Medium)
                {
                    Q3.Cast(sender);
                }
            }
        }
    }
}