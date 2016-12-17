using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Events
{
    using System;
    using Games.Mode;
    using LeagueSharp.Common;


    internal class LoopManager : Logic
    {
        internal static void Init(EventArgs args)
        {
            QADelay.Init();

            if (W.Level > 0)
            {
                W.Range = Me.HasBuff("RivenFengShuiEngine") ? 330 : 260;
            }

            if (Me.IsDead)
            {
                qStack = 0;
                return;
            }

            if (qStack != 0 && Utils.TickCount - lastQTime > 3800)
            {
                qStack = 0;
            }

            if (Me.IsRecalling())
            {
                return;
            }

            KeepQ.Init();
            KillSteal.Init();

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo.Init();
                    break;
                case Orbwalking.OrbwalkingMode.Burst:
                    Burst.Init();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass.Init();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear.Init();
                    JungleClear.Init();
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    Flee.Init();
                    break;
            }
        }
    }
}