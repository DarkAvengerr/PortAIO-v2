using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Events
{
    using myCommon;
    using LeagueSharp.Common;

    internal class GapcloserManager : Logic
    {
        internal static void Init(ActiveGapcloser Args)
        {
            if (Menu.GetBool("AntiGapCloserW") && W.IsReady() && Args.Sender.IsValidTarget(W.Range) &&
                Me.CountEnemiesInRange(1000) < 3)
            {
                W.Cast(true);
            }
        }
    }
}