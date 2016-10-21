using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksCassiopeia.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikksCassiopeia.Config;

    internal static class Combo
    {
        #region Methods

        internal static void Execute()
        {
            var target = TargetSelector.GetTarget(Spells.R.Range + 250, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (Config.IsChecked("useRInCombo") && Spells.R.IsReady())
            {
                var enemiesAroundTarget =
                    HeroManager.Enemies.Count(en => en.Distance(target.Position) <= 1000 && en.Name != target.Name);
                if (Config.IsChecked("comboFlashR") && target.IsFacing(ObjectManager.Player)
                    && (target.Distance(ObjectManager.Player) > Spells.R.Range
                        && target.Distance(ObjectManager.Player) <= Spells.R.Range + 400)
                    && (Spells.Flash != null && Spells.Flash.IsReady())
                    && Computed.ComboDmg(target) * Spells.ComboDmgMod > target.Health
                    && enemiesAroundTarget <= Config.GetSliderValue("maxEnFlash"))
                {
                    Spells.FlashR = true;
                    var relPos = target.Position.Shorten(ObjectManager.Player.Position, -300);
                    Spells.R.Cast(relPos);
                    LeagueSharp.Common.Utility.DelayAction.Add(
                        Mainframe.RDelay.Next(300, 400), 
                        () => ObjectManager.Player.Spellbook.CastSpell(Spells.Flash.Slot, target.Position));
                }

                var countFace =
                    HeroManager.Enemies.Count(h => h.IsValidTarget(Spells.R.Range) && h.IsFacing(ObjectManager.Player) && Spells.R.WillHit(h, ObjectManager.Player.Position));
                if (countFace >= Config.GetSliderValue("comboMinR") && target.IsValidTarget(Spells.R.Range) && target.IsFacing(ObjectManager.Player))
                {
                    Spells.R.Cast(target);
                }
            }

            if (Config.IsChecked("useQInCombo") && Spells.Q.IsReady() && !target.HasBuffOfType(BuffType.Poison))
            {
                var qPred = Spells.Q.GetPrediction(target);
                if (qPred.Hitchance >= HitChance.VeryHigh)
                {
                    Spells.Q.Cast(qPred.CastPosition);
                }
            }

            if (Config.IsChecked("useWInCombo") && Spells.W.IsReady() && target.IsValidTarget(Spells.W.Range))
            {
                if (Config.IsChecked("comboWonlyCD"))
                {
                    if (!Spells.Q.IsReady() && (Spells.QCasted - Game.Time) < -0.5f
                        && !target.HasBuffOfType(BuffType.Poison))
                    {
                        var wPred = Spells.W.GetPrediction(target);
                        if (wPred.CastPosition.Distance(ObjectManager.Player.Position) >= Spells.WMinRange
                            && wPred.Hitchance >= HitChance.VeryHigh)
                        {
                            Spells.W.Cast(wPred.CastPosition);
                        }
                    }
                }
                else
                {
                    var wPred = Spells.W.GetPrediction(target);
                    if (wPred.CastPosition.Distance(ObjectManager.Player.Position) >= Spells.WMinRange && wPred.Hitchance >= HitChance.VeryHigh)
                    {
                        Spells.W.Cast(wPred.CastPosition);
                    }
                }
            }

            if (Config.IsChecked("useEInCombo") && Spells.E.IsReady() && target.IsValidTarget(Spells.E.Range)
                && (!Config.IsChecked("comboEonP") || target.HasBuffOfType(BuffType.Poison)))
            {
                if (Config.IsChecked("humanEInCombo"))
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