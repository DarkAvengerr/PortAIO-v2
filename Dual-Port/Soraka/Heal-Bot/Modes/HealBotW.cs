using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Soraka_HealBot.Modes
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = Soraka_HealBot.Config;

    internal static class HealBotW
    {
        #region Methods

        internal static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < Config.GetSliderValue("manaToW")
                || ObjectManager.Player.HealthPercent < Config.GetSliderValue("playerHpToW")
                || ObjectManager.Player.IsRecalling())
            {
                return;
            }

            var validAlliesInRange =
                HeroManager.Allies.Where(
                    ally =>
                    !ally.IsMe && !ally.IsDead && !ally.IsZombie && ally.IsHPBarRendered && !ally.InShop()
                    && ally.Distance(ObjectManager.Player) <= Spells.W.Range && !ally.IsRecalling()
                    && Config.IsChecked("autoW_" + ally.ChampionName)).ToList();
            if (!validAlliesInRange.Any())
            {
                return;
            }

            IEnumerable<AIHeroClient> alliesToConsider;
            if (ObjectManager.Player.HasBuff("SorakaQRegen"))
            {
                alliesToConsider =
                    validAlliesInRange.Where(
                        ally => ally.HealthPercent <= Config.GetSliderValue("autoWBuff_HP_" + ally.ChampionName))
                        .ToList();
            }
            else
            {
                alliesToConsider =
                    validAlliesInRange.Where(
                        ally => ally.HealthPercent <= Config.GetSliderValue("autoW_HP_" + ally.ChampionName)).ToList();
            }

            if (!alliesToConsider.Any())
            {
                return;
            }

            AIHeroClient allyToHeal = null;
            switch (Config.GetStringListValue("wHealMode"))
            {
                case 0:
                    allyToHeal = alliesToConsider.OrderBy(x => x.Health).First();
                    break;
                case 1:
                    allyToHeal =
                        alliesToConsider.OrderByDescending(x => x.TotalAttackDamage).ThenBy(x => x.Health).First();
                    break;
                case 2:
                    allyToHeal =
                        alliesToConsider.OrderByDescending(x => x.TotalMagicalDamage).ThenBy(x => x.Health).First();
                    break;
                case 3:
                    allyToHeal =
                        alliesToConsider.OrderByDescending(x => x.TotalAttackDamage + x.TotalMagicalDamage)
                            .ThenBy(x => x.Health)
                            .First();
                    break;
                case 4:
                    allyToHeal =
                        alliesToConsider.OrderBy(x => x.Distance(ObjectManager.Player)).ThenBy(x => x.Health).First();
                    break;
                case 5:
                    allyToHeal =
                        alliesToConsider.OrderByDescending(x => Config.GetSliderValue("autoWPrio" + x.ChampionName))
                            .ThenBy(x => x.Health)
                            .First();
                    break;
            }

            Spells.W.Cast(allyToHeal);
        }

        #endregion
    }
}