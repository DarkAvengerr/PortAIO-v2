using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksTwistedFate.Modes
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikksTwistedFate.Config;

    internal static class Harass
    {
        #region Methods

        internal static void Execute()
        {
            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget(Spells.Q.Range)
                || ObjectManager.Player.ManaPercent < Config.GetSliderValue("manaToHarass"))
            {
                return;
            }

            var qMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;
            var wMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
            var wManaP = wMana / ObjectManager.Player.Mana;
            if (target.Distance(ObjectManager.Player) <= ObjectManager.Player.AttackRange + 100
                && ObjectManager.Player.Mana >= wMana && Spells.W.IsReady() && Config.IsChecked("useWHarass"))
            {
                switch (Config.GetStringListValue("wModeH"))
                {
                    case 0:
                        CardSelector.StartSelecting(
                            ObjectManager.Player.ManaPercent < Config.GetSliderValue("manaToHarass") + 10 + wManaP
                                ? Cards.Blue
                                : Cards.Yellow);
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

            if (Config.IsChecked("useQHarass") && Spells.Q.IsReady())
            {
                var qPred = Spells.Q.GetPrediction(target);
                if (ObjectManager.Player.Mana >= qMana && qPred.Hitchance >= HitChance.High)
                {
                    Spells.Q.Cast(qPred.CastPosition);
                }
            }
        }

        #endregion
    }
}