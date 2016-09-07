#region

using System;
using LeagueSharp;
using LeagueSharp.Common;
using NechritoRiven.Core;
using NechritoRiven.Menus;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event
{
    internal class AlwaysUpdate : Core.Core
    {
        public static void Update(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
            {
                return;
            }

            if (Utils.GameTimeTickCount - LastQ >= 3650 && Qstack != 1 && !Player.InFountain() && MenuConfig.KeepQ && Player.HasBuff("RivenTriCleave") &&
              !Player.Spellbook.IsChanneling && Spells.Q.IsReady()) Spells.Q.Cast(Game.CursorPos);

            Modes.QMove();
            ForceSkill();
            
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Modes.Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Burst:
                    Modes.Burst();
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    Modes.Flee();
                    break;
                case Orbwalking.OrbwalkingMode.FastHarass:
                    Modes.FastHarass();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Modes.Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Modes.Jungleclear();
                    Modes.Laneclear();
                    break;
            }
        }
    }
}
