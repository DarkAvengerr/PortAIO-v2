using EloBuddy; 
using LeagueSharp.Common; 
namespace myKatarina.Manager.Events.Games
{
    using Mode;
    using System;
    using myCommon;
    using Spells;
    using LeagueSharp.Common;
    

    internal class LoopManager : Logic
    {
        internal static void Init(EventArgs args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            KillSteal.Init();

            if (SpellManager.isCastingUlt)
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);

                if (Menu.GetBool("AutoCancel"))
                {
                    SpellManager.CancelUlt();
                }

                return;
            }

            Orbwalker.SetAttack(true);
            Orbwalker.SetMovement(true);

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
                case Orbwalking.OrbwalkingMode.Flee:
                    Flee.Init();
                    break;
            }
        }
    }
}
