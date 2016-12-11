using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Yasuo.Manager.Events
{
    using Spells;
    using LeagueSharp.Common;
    using Orbwalking = Orbwalking;

    internal class GapcloserManager : Logic
    {
        internal static void Init(ActiveGapcloser Args)
        {
            if (Menu.Item("Q3Anti", true).GetValue<bool>() && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                var target = Args.Sender;

                if (target.IsValidTarget(400) && Q3.IsReady() && SpellManager.HaveQ3)
                {
                    Q3.Cast(target);
                }
            }
        }
    }
}