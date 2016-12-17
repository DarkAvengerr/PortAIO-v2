using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Events.Games.Mode
{
    using myCommon;
    using System.Linq;
    using LeagueSharp.Common;

    internal class LaneClear : Logic
    {
        internal static void Init()
        {
            if (Me.UnderTurret(true))
            {
                return;
            }

            if (Menu.GetBool("LaneClearQ") && Q.IsReady() && qStack == 0 && Utils.TickCount - lastQTime > 1500)
            {
                var minions = MinionManager.GetMinions(Me.ServerPosition, Q.Range + Me.BoundingRadius);

                if (minions.Any())
                {
                    var qFram = Q.GetCircularFarmLocation(minions);

                    if (qFram.MinionsHit >= 2)
                    {
                        Q.Cast(qFram.Position, true);
                    }
                }
            }

            if (Menu.GetBool("LaneClearW") && W.IsReady())
            {
                var minions = MinionManager.GetMinions(Me.ServerPosition, W.Range);

                if (minions.Count >= 3)
                {
                    W.Cast(true);
                }
            }
        }
    }
}
