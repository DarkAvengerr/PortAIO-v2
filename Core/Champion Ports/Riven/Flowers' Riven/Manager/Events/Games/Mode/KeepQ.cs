using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Events.Games.Mode
{
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class KeepQ : Logic
    {
        internal static void Init()
        {
            if (Menu.GetBool("KeepQALive") && !Me.UnderTurret(true) && Me.HasBuff("RivenTriCleave"))
            {
                if (Me.GetBuff("RivenTriCleave").EndTime - Game.Time < 0.3)
                {
                    Q.Cast(Me.Position.Extend(Game.CursorPos, 350f), true);
                }
            }
        }
    }
}
