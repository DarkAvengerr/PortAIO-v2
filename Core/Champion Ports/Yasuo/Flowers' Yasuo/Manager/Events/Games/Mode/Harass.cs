using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Yasuo.Manager.Events.Games.Mode
{
    using Spells;
    using LeagueSharp.Common;
    using System.Linq;

    internal class Harass : Logic
    {
        internal static void Init()
        {
            if (!IsDashing)
            {
                if (SpellManager.HaveQ3)
                {
                    var target = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Physical);

                    if (Menu.Item("HarassE", true).GetValue<bool>() && E.IsReady() && Q3.IsReady() &&
                        target.IsValidTarget(1200) &&
                        Utils.TickCount - lastHarassTime > 5000)
                    {
                        SpellManager.EGapMouse(target, Menu.Item("HarassTower", true).GetValue<bool>(), 250);
                    }

                    if (Menu.Item("HarassQ3", true).GetValue<bool>() && Q3.IsReady() &&
                        target.IsValidTarget(1200))
                    {
                        if (Menu.Item("HarassTower", true).GetValue<bool>() || !Me.UnderTurret(true))
                        {
                            var q3Pred = Q3.GetPrediction(target, true);

                            if (q3Pred.Hitchance >= HitChance.VeryHigh)
                            {
                                Q3.Cast(q3Pred.CastPosition, true);
                            }
                        }
                    }
                }
                else
                {
                    if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady())
                    {
                        if (Menu.Item("HarassTower", true).GetValue<bool>() || !Me.UnderTurret(true))
                        {
                            if (Me.CountEnemiesInRange(Q.Range) == 0)
                            {
                                var minions =
                                    MinionManager
                                        .GetMinions(Me.Position, Q.Range)
                                        .FirstOrDefault(x => x.Health < Q.GetDamage(x));

                                if (minions != null && minions.IsValidTarget(Q.Range))
                                {
                                    Q.Cast(minions, true);
                                }
                            }

                            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

                            if (target.IsValidTarget(Q.Range))
                            {
                                var qPred = Q.GetPrediction(target, true);

                                if (qPred.Hitchance >= HitChance.VeryHigh)
                                {
                                    Q.Cast(qPred.CastPosition, true);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (Menu.Item("HarassQ3", true).GetValue<bool>() && Q3.IsReady() && SpellManager.HaveQ3 &&
                    lastEPos.CountEnemiesInRange(220) > 0)
                {
                    if (Q3.Cast(Me.Position, true))
                    {
                        lastHarassTime = Utils.TickCount;
                    }
                }
            }
        }
    }
}
