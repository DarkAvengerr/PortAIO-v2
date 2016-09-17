using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksTwistedFate.Modes
{
    using System.Linq;
    using System.Windows.Input;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikksTwistedFate.Config;

    internal static class Automated
    {
        #region Methods

        internal static void Execute()
        {
            if (Config.IsChecked("autoQonCC"))
            {
                AutoCcq();
            }

            var qMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;
            if (Config.IsChecked("qKillsteal") && ObjectManager.Player.Mana >= qMana && Spells.Q.IsReady())
            {
                var entKs =
                    HeroManager.Enemies.FirstOrDefault(
                        h =>
                        !h.IsDead && h.IsValidTarget(Spells.Q.Range)
                        && h.Health < ObjectManager.Player.GetSpellDamage(h, SpellSlot.Q));
                if (entKs != null)
                {
                    Spells.Q.Cast(entKs);
                }
            }

            if (Config.IsChecked("autoQ") && ObjectManager.Player.Mana >= qMana
                && ObjectManager.Player.ManaPercent >= Config.GetSliderValue("manaToAHarass") && Spells.Q.IsReady()
                && Orbwalking.CanAttack())
            {
                var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);
                if (target.IsValidTarget(Spells.Q.Range))
                {
                    var qPred = Spells.Q.GetPrediction(target);
                    if (qPred.Hitchance >= HitChance.High)
                    {
                        Spells.Q.Cast(qPred.CastPosition);
                    }
                }
            }
        }

        private static void AutoCcq()
        {
            if (!Spells.Q.IsReady())
            {
                return;
            }

            foreach (var qPred in
                HeroManager.Enemies.Where(m => !m.IsDead && m.IsValidTarget(Spells.Q.Range))
                    .Select(enemy => Spells.Q.GetPrediction(enemy))
                    .Where(qPred => qPred.Hitchance == HitChance.Immobile))
            {
                Spells.Q.Cast(qPred.CastPosition);
            }
        }

        #endregion
    }
}