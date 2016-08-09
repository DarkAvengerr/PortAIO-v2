using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Soraka_HealBot.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Soraka_HealBot.Extensions;

    using Config = Soraka_HealBot.Config;

    internal static class AutoHarass
    {
        #region Methods

        internal static void AutoE()
        {
            if (!Spells.E.IsReady() || !Orbwalking.CanAttack() || !ObjectManager.Player.CanCast
                || ObjectManager.Player.HasBuff("Recall")
                || ObjectManager.Player.ManaPercent <= Config.GetSliderValue("manaAutoHarass"))
            {
                return;
            }

            var target = TargetSelector.GetTarget(Spells.E.Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (Config.IsChecked("dontAutoHarassTower") && target.Position.UnderEnemyTurret()
                && ObjectManager.Player.Position.UnderEnemyTurret())
            {
                return;
            }

            if (Config.IsChecked("dontHarassInBush"))
            {
                var bush =
                    ObjectManager.Get<GrassObject>()
                        .OrderBy(br => br.Position.Distance(ObjectManager.Player.Position))
                        .FirstOrDefault();
                if (bush != null && ObjectManager.Player.Position.Distance(bush.Position) <= bush.BoundingRadius)
                {
                    return;
                }
            }

            if (Config.IsChecked("autoEHarassOnlyCC"))
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

        internal static void AutoQ()
        {
            if (!Spells.Q.IsReady() || !ObjectManager.Player.CanCast || ObjectManager.Player.HasBuff("Recall")
                || ObjectManager.Player.ManaPercent <= Config.GetSliderValue("manaAutoHarass"))
            {
                return;
            }

            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (Config.IsChecked("dontAutoHarassTower") && target.Position.UnderEnemyTurret()
                && ObjectManager.Player.Position.UnderEnemyTurret())
            {
                return;
            }

            if (Config.IsChecked("dontHarassInBush"))
            {
                var bush =
                    ObjectManager.Get<GrassObject>()
                        .OrderBy(br => br.Position.Distance(ObjectManager.Player.Position))
                        .FirstOrDefault();
                if (bush != null && ObjectManager.Player.Position.Distance(bush.Position) <= bush.BoundingRadius)
                {
                    return;
                }
            }

            var qPred = Spells.E.GetPrediction(target, true);
            if (qPred.Hitchance >= Spells.GetHitChance("pred.hitchance.q"))
            {
                Spells.Q.Cast(qPred.CastPosition);
            }
        }

        #endregion
    }
}