using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Soraka_HealBot.Modes
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = Soraka_HealBot.Config;

    internal static class Harass
    {
        #region Methods

        internal static void Execute()
        {
            var target = TargetSelector.GetTarget(Spells.E.Range, TargetSelector.DamageType.Magical);
            if (target == null || ObjectManager.Player.ManaPercent < Config.GetSliderValue("manaHarass"))
            {
                return;
            }

            if (Config.IsChecked("useQInHarass") && Spells.Q.CanCast(target))
            {
                var qPred = Spells.Q.GetPrediction(target, true);
                if (qPred.Hitchance >= Spells.GetHitChance("pred.hitchance.q"))
                {
                    Spells.Q.Cast(qPred.CastPosition);
                }
            }

            if (Config.IsChecked("useEInHarass") && Spells.E.CanCast(target))
            {
                if (Config.IsChecked("eOnlyCCHarass"))
                {
                    var ePredcc = Spells.E.GetPrediction(target);
                    if (ePredcc.Hitchance == HitChance.Immobile)
                    {
                        Spells.E.Cast(ePredcc.CastPosition);
                    }

                    return;
                }

                var ePred = Spells.E.GetPrediction(target, true);
                if (ePred.Hitchance >= Spells.GetHitChance("pred.hitchance.e"))
                {
                    Spells.E.Cast(ePred.CastPosition);
                }
            }
        }

        #endregion
    }
}