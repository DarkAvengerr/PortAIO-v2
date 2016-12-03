using EloBuddy; 
using LeagueSharp.Common; 
namespace myDiana.Manager.Events
{
    using myCommon;
    using LeagueSharp.Common;

    internal class AntiGapcloserManager : Logic
    {
        internal static void Init(ActiveGapcloser Args)
        {
            if (Menu.GetBool("EGap") && Args.Sender.IsValidTarget(E.Range))
            {
                if (E.IsReady())
                {
                    E.Cast(true);
                }
            }
        }
    }
}