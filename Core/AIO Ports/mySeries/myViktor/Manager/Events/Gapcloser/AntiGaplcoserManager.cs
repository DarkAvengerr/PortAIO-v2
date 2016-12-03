using EloBuddy; 
using LeagueSharp.Common; 
namespace myViktor.Manager.Events
{
    using LeagueSharp.Common;
    using myCommon;

    internal class AntiGaplcoserManager : Logic
    {
        internal static void Init(ActiveGapcloser Args)
        {
            if (Menu.GetBool("GapW") && W.IsReady() && Me.Distance(Args.End) <= 250)
            {
                W.Cast(Me.Position, true);
            }
        }
    }
}