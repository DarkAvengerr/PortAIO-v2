using EloBuddy; 
using LeagueSharp.Common; 
namespace myViktor.Manager.Events.Games.Mode
{
    using System.Linq;
    using myCommon;
    using LeagueSharp.Common;

    internal class Auto : Logic
    {
        internal static void Init()
        {
            if (Menu.GetBool("AutoR") && Me.HasBuff("ViktorChaosStormTimer"))
            {
                if (Utils.TickCount - rMoveMentTick > 300)
                {
                    if (Me.CountEnemiesInRange(1800) > 0)
                    {
                        foreach (
                            var target in HeroManager.Enemies.Where(x => x.IsValidTarget(1800)).OrderBy(x => x.Health))
                        {
                            R.Cast(target.Position, true);
                            rMoveMentTick = Utils.TickCount;
                        }
                    }
                    else if (MinionManager.GetMinions(Me.Position, 1300, MinionTypes.All, MinionTeam.NotAlly).Any())
                    {
                        var Minions = MinionManager.GetMinions(Me.Position, 1300, MinionTypes.All, MinionTeam.NotAlly);
                        var minion = Minions.FirstOrDefault();

                        if (minion != null)
                        {
                            R.Cast(minion.Position, true);
                            rMoveMentTick = Utils.TickCount;
                        }
                    }
                    else
                    {
                        R.Cast(Me.Position, true);
                        rMoveMentTick = Utils.TickCount;
                    }
                }
            }

            if (Menu.GetBool("AutoW") && W.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && !x.CanMoveMent()))
                {
                    if (target.IsValidTarget(W.Range))
                    {
                        W.Cast(target.Position, true);
                    }
                }
            }
        }
    }
}
