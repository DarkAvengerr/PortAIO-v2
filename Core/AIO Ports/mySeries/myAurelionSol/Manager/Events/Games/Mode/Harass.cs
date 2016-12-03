using EloBuddy; 
using LeagueSharp.Common; 
namespace myAurelionSol.Manager.Events.Games.Mode
{
    using Spells;
    using myCommon;
    using LeagueSharp.Common;

    internal class Harass : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                if (Menu.GetBool("HarassQ") && Q.IsReady())
                {
                    var target = TargetSelector.GetTarget(Q.Range*2, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget(Q.Range))
                    {
                        var qPred = Q.GetPrediction(target, true);

                        if (qPred.Hitchance >= HitChance.VeryHigh || qPred.Hitchance == HitChance.Immobile)
                        {
                            Q.Cast(qPred.CastPosition, true);
                        }
                    }
                }

                if (Menu.GetBool("HarassW") && SpellManager.HavePassive && W.IsReady())
                {
                    var WTTT = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

                    if (!SpellManager.IsWActive)
                    {
                        if (WTTT != null && WTTT.IsValidTarget(W.Range) && !WTTT.IsValidTarget(420))
                        {
                            W.Cast();
                        }
                    }
                    else if (SpellManager.IsWActive)
                    {
                        if (!(WTTT.Distance(Me.ServerPosition) < 840) || WTTT.IsValidTarget(420))
                        {
                            W.Cast();
                        }
                    }
                }
            }
        }
    }
}
