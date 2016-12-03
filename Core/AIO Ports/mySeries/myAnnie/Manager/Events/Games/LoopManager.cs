using EloBuddy; 
using LeagueSharp.Common; 
namespace myAnnie.Manager.Events.Games
{
    using Mode;
    using System;
    using LeagueSharp.Common;
    

    internal class LoopManager : Logic
    {
        internal static void Init(EventArgs args)
        {
            AutoFollow.Init();

            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            KillSteal.Init();
 
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo.Init();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass.Init();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear.Init();
                    JungleClear.Init();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    FlashR.Init();
                    break;
            }
        }
    }
}
