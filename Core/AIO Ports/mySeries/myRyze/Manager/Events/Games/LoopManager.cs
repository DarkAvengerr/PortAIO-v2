using EloBuddy; 
using LeagueSharp.Common; 
namespace myRyze.Manager.Events.Games
{
    using Mode;
    using myCommon;
    using System;
    using LeagueSharp.Common;
    

    internal class LoopManager : Logic
    {
        internal static void Init(EventArgs Args)
        {
            R.Range = R.Level > 0 ? R.Level*1500 : 0;
            CalculateCD.Init();

            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            Q.Collision =
                !(Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Menu.GetBool("ComboQSmart") && CanShield);

            KillSteal.Init();
            Auto.Init();
 
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
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit.Init();
                    break;
            }
        }
    }
}
