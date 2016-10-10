using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksTwistedFate.Modes
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikksTwistedFate.Config;

    internal static class Combo
    {
        #region Methods

        internal static void Execute()
        {
            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget(Spells.Q.Range))
            {
                return;
            }

            var qMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;
            var wMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
            if (target.Distance(ObjectManager.Player)
                <= ObjectManager.Player.AttackRange + 100 + Config.GetSliderValue("combo.w.extrarange")
                && ObjectManager.Player.Mana >= wMana && Spells.W.IsReady() && Config.IsChecked("useWCombo"))
            {
                switch (Config.GetStringListValue("wModeC"))
                {
                    case 0:
                        CardSelector.StartSelecting(
                            ObjectManager.Player.Mana < qMana + wMana ? Cards.Blue : Cards.Yellow);
                        break;
                    case 1:
                        CardSelector.StartSelecting(Cards.Yellow);
                        break;
                    case 2:
                        CardSelector.StartSelecting(Cards.Blue);
                        break;
                    case 3:
                        CardSelector.StartSelecting(Cards.Red);
                        break;
                }
            }

            if (Config.IsChecked("useQCombo") && Spells.Q.IsReady())
            {
                var qPred = Spells.Q.GetPrediction(target);
                if (ObjectManager.Player.Mana >= qMana && qPred.Hitchance >= HitChance.High
                    && !Config.IsChecked("yellowIntoQ"))
                {
                    Spells.Q.Cast(qPred.CastPosition);
                }
            }
        }

        #endregion
    }
}