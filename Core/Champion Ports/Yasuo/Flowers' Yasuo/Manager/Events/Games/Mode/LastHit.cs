using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Yasuo.Manager.Events.Games.Mode
{
    using Spells;
    using System.Linq;
    using LeagueSharp.Common;
    using Orbwalking = Orbwalking;

    internal class LastHit : Logic
    {
        internal static void Init()
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed || IsDashing)
            {
                return;
            }

            var minions = MinionManager.GetMinions(Me.Position, Q3.Range);

            foreach (
                var minion in
                minions.Where(x => !x.IsDead && HealthPrediction.GetHealthPrediction(x, 2500) > 0)
                    .OrderByDescending(m => m.Health))
            {
                if (Menu.Item("LastHitQ", true).GetValue<bool>() && Q.IsReady() && !SpellManager.HaveQ3)
                {
                    if (minion.IsValidTarget(Q.Range) && minion.Health < Q.GetDamage(minion))
                    {
                        Q.Cast(minion, true);
                    }
                }

                if (Menu.Item("LastHitQ3", true).GetValue<bool>() && Q.IsReady() && SpellManager.HaveQ3)
                {
                    if (minion.IsValidTarget(Q3.Range) && minion.Health < Q3.GetDamage(minion))
                    {
                        var qPred = Q3.GetPrediction(minion, true);

                        if (qPred.Hitchance >= HitChance.VeryHigh)
                        {
                            Q3.Cast(qPred.CastPosition, true);
                        }
                    }
                }

                if (Menu.Item("LastHitE", true).GetValue<bool>() && E.IsReady())
                {
                    if (minion.IsValidTarget(E.Range) && minion.Health < E.GetDamage(minion)
                        && SpellManager.CanCastE(minion) &&
                        (Menu.Item("LastHitETurret", true).GetValue<bool>() || !UnderTower(PosAfterE(minion))))
                    {
                        E.CastOnUnit(minion, true);
                    }
                }
            }
        }
    }
}
