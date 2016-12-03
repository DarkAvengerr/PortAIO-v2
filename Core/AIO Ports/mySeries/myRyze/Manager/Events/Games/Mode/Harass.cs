using EloBuddy; 
using LeagueSharp.Common; 
namespace myRyze.Manager.Events.Games.Mode
{
    using System.Linq;
    using Spells;
    using myCommon;
    using LeagueSharp.Common;

    internal class Harass : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                if (Menu.GetBool("HarassQ") && Q.IsReady() && !SpellManager.FullStack)
                {
                    var minion =
                        MinionManager
                            .GetMinions(
                                Me.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                            .FirstOrDefault(x => x.HasBuff("ryzee") && x.Health < Q.GetDamage(x) && Q.CanCast(x) &&
                                                 HeroManager.Enemies.Any(a => a.Distance(x) <= 290));

                    if (minion != null)
                    {
                        Q.Cast(minion, true);
                    }
                    else
                    {
                        var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                        if (target.Check(Q.Range))
                        {
                            var qPred = Q.GetPrediction(target);

                            if (qPred.Hitchance >= HitChance.High)
                            {
                                Q.Cast(qPred.CastPosition, true);
                            }
                        }
                    }
                }

                if (Menu.GetBool("HarassE") && E.IsReady() && !SpellManager.HalfStack)
                {
                    var minion =
                        MinionManager
                            .GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.NotAlly)
                            .FirstOrDefault(
                                x =>
                                    x.Health < E.GetDamage(x) &&
                                    HeroManager.Enemies.Any(a => a.Distance(x.Position) <= 290));

                    if (minion != null)
                    {
                        E.CastOnUnit(minion, true);
                    }
                    else
                    {
                        var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

                        if (target.Check(E.Range))
                        {
                            E.CastOnUnit(target, true);
                        }
                    }
                }

                if (Menu.GetBool("HarassW") && W.IsReady() && !SpellManager.HalfStack)
                {
                    var target =
                        HeroManager.Enemies.FirstOrDefault(
                            x => x.IsValidTarget(W.Range) && !x.HasBuff("ryzee"));

                    if (target.Check(W.Range))
                    {
                        W.CastOnUnit(target, true);
                    }
                }
            }
        }
    }
}
