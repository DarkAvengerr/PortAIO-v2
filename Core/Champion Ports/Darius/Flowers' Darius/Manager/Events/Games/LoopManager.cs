using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Darius.Manager.Events
{
    using System;
    using Games.Mode;
    using LeagueSharp.Common;
    

    internal class LoopManager : Logic
    {
        internal static void Init(EventArgs Args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            if (!Me.HasBuff("dariusqcast"))
            {
                Orbwalker.SetMovement(true);
                Orbwalker.SetAttack(true);
            }

            Auto.Init();
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
            }
        }
    }
}