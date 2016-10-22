using System.Linq;

using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

using Settings = xcKalista.Config.Modes.LaneClear;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcKalista.Modes
{
    internal sealed class LaneClear : ModeBase
    {
        internal override bool ShouldBeExecuted()
        {
            return Config.Keys.LaneClearActive;
        }

        internal override void Execute()
        {
            if (Settings.UseE.BValue && GameObjects.Player.ManaPercent > Settings.MinMana && GameObjects.EnemyMinions.Count(x => x.IsKillableWithE(true)) >= Settings.UseE.SValue)
            {
                E.Cast();
            }

            if (!Variables.Orbwalker.CanMove)
            {
                return;
            }

            if (Settings.UseQ.BValue && Q.IsReady() && GameObjects.Player.ManaPercent > Settings.MinMana)
            {
                foreach (var minion in GameObjects.EnemyMinions.Where(x => x.IsKillableWithQ(true)))
                {
                    var prediction = Q.GetPrediction(minion);
                    if (prediction.Hitchance < HitChance.High)
                    {
                        continue;
                    }

                    var killcount = 0;

                    foreach (var collisionMinion in prediction.CollisionObjects.OrderBy(x => x.Distance(GameObjects.Player)))
                    {
                        if (collisionMinion.IsKillableWithQ())
                        {
                            killcount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (killcount < Settings.UseQ.SValue)
                    {
                        continue;
                    }

                    if (Q.Cast(prediction.CastPosition))
                    {
                        break;
                    }
                }
            }
        }
    }
}
