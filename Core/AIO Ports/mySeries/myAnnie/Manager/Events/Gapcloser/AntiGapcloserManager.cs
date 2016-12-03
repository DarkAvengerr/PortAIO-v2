using EloBuddy; 
using LeagueSharp.Common; 
namespace myAnnie.Manager.Events
{
    using Spells;
    using LeagueSharp.Common;
    using myCommon;

    internal class AntiGapcloserManager : Logic
    {
        internal static void Init(ActiveGapcloser Args)
        {
            var sender = Args.Sender;

            if (!SpellManager.HaveStun && SpellManager.BuffCounts == 3 && E.IsReady() && Menu.GetBool("AntiGapcloserE"))
            {
                E.Cast();
            }

            if (SpellManager.HaveStun)
            {
                if (Q.IsReady() && Menu.GetBool("AntiGapcloserQ") && sender.IsValidTarget(300))
                {
                    Q.Cast(sender, true);
                }
                else if (W.IsReady() && Menu.GetBool("AntiGapcloserW") && sender.IsValidTarget(250))
                {
                    W.Cast(sender, true);
                }
            }
        }
    }
}