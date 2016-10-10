using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksCassiopeia.Modes
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikksCassiopeia.Config;

    internal static class Harass
    {
        #region Methods

        internal static void Execute()
        {
            var target = TargetSelector.GetTarget(Spells.R.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget(Spells.Q.Range)
                || ObjectManager.Player.ManaPercent < Config.GetSliderValue("manaToHarass"))
            {
                return;
            }

            if (Config.IsChecked("useQInHarass") && Spells.Q.IsReady() && !target.HasBuffOfType(BuffType.Poison)
                && (!ObjectManager.Player.Spellbook.IsAutoAttacking))
            {
                var qPred = Spells.Q.GetPrediction(target);
                if (qPred.Hitchance >= HitChance.High)
                {
                    Spells.Q.Cast(qPred.CastPosition);
                }
            }

            if (Config.IsChecked("useWInHarass") && Spells.W.IsReady() && target.IsValidTarget(Spells.W.Range)
                && (!ObjectManager.Player.Spellbook.IsAutoAttacking))
            {
                if (Config.IsChecked("harassWonlyCD"))
                {
                    if (!Spells.Q.IsReady() && (Spells.QCasted - Game.Time) < -0.5f
                        && !target.HasBuffOfType(BuffType.Poison))
                    {
                        var wPred = Spells.W.GetPrediction(target);
                        if (wPred.CastPosition.Distance(ObjectManager.Player.Position) >= Spells.WMinRange
                            && wPred.Hitchance >= HitChance.High)
                        {
                            Spells.W.Cast(wPred.CastPosition);
                        }
                    }
                }
                else
                {
                    var wPred = Spells.W.GetPrediction(target);
                    if (wPred.Hitchance >= HitChance.High)
                    {
                        Spells.W.Cast(wPred.CastPosition);
                    }
                }
            }

            if (Config.IsChecked("useEInHarass") && Spells.E.IsReady() && target.IsValidTarget(Spells.E.Range)
                && (!Config.IsChecked("harassEonP") || target.HasBuffOfType(BuffType.Poison)))
            {
                if (Config.IsChecked("humanEInHarass"))
                {
                    var delay = Computed.RandomDelay(Config.GetSliderValue("humanDelay"));
                    LeagueSharp.Common.Utility.DelayAction.Add(delay, () => Spells.E.Cast(target));
                }
                else
                {
                    Spells.E.Cast(target);
                }
            }
        }

        #endregion
    }
}