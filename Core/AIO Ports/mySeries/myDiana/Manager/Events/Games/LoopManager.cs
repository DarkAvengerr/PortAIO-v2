using EloBuddy; 
using LeagueSharp.Common; 
namespace myDiana.Manager.Events.Games
{
    using Mode;
    using System;
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;
    

    internal class LoopManager : Logic
    {
        internal static void Init(EventArgs args)
        {
            qCd = Q.Level > 0
                ? (Q.Instance.CooldownExpires - Game.Time <= 0 ? 0 : (int) (Q.Instance.CooldownExpires - Game.Time))
                : -1;

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
                    if (Menu.GetKey("EFlash"))
                    {
                        EFlash.Init();
                    }
                    break;
            }
        }
    }
}
