using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Events.Games.Mode
{
    using System.Linq;
    using myCommon;
    using LeagueSharp.Common;

    internal class JungleClear : Logic
    {
        internal static void Init()
        {
            if (Menu.GetBool("JungleClearE") && E.IsReady())
            {
                var mobs = MinionManager.GetMinions(Me.Position, 500f, MinionTypes.All, MinionTeam.Neutral);

                if (!mobs.Any(x => x.DistanceToPlayer() < E.Range) && mobs.Any(x => x.DistanceToPlayer() <= 500f))
                {
                    var mob = mobs.FirstOrDefault();

                    if (mob != null)
                    {
                        E.Cast(mob.Position, true);
                    }
                }
            }

            if (Menu.GetBool("JungleClearWLogic") && W.IsReady())
            {
                var mobs = MinionManager.GetMinions(Me.Position, W.Range, MinionTypes.All, MinionTeam.Neutral);

                if (mobs.Any())
                {
                    if ((!Q.IsReady() && qStack == 0) || ((qStack == 1 || qStack == 2) && Q.IsReady()))
                    {
                        W.Cast(true);
                    }
                }
            }
        }
    }
}
