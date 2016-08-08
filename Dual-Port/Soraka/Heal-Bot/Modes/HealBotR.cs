using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Soraka_HealBot.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Soraka_HealBot.Extensions;

    using Config = Soraka_HealBot.Config;

    internal static class HealBotR
    {
        #region Methods

        internal static void Execute()
        {
            if (!Config.IsChecked("cancelBase") && ObjectManager.Player.LSHasBuff("Recall"))
            {
                return;
            }

            var alliesToCheck =
                HeroManager.Allies.Where(
                    ally =>
                    !ally.IsMe && !ally.IsDead && !ally.IsZombie && !ally.LSInShop() && !ally.LSHasBuff("Recall")
                    && Config.IsChecked("autoR_" + ally.ChampionName)
                    && ally.HealthPercent <= Config.GetSliderValue("autoRHP"));
            foreach (var ally in alliesToCheck)
            {
                if (ally.LSCountEnemiesInRange(950) < 1)
                {
                    continue;
                }

                if (ally.Health > ally.GetEnemiesDamageNearAlly(1.2f, 950)
                    || ally.Health + Spells.GetUltHeal(ally) < ally.GetEnemiesDamageNearAlly(0.8f, 950))
                {
                    continue;
                }

                if (!Spells.R.LSIsReady())
                {
                    return;
                }

                var delay = OtherUtils.RDelay.Next(20, 50);
                LeagueSharp.Common.Utility.DelayAction.Add(delay, () => ObjectManager.Player.Spellbook.CastSpell(SpellSlot.R));
            }
        }

        #endregion
    }
}