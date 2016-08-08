using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Soraka_HealBot.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Soraka_HealBot.Extensions;

    using Config = Soraka_HealBot.Config;

    internal static class AssistKs
    {
        #region Methods

        internal static void Execute()
        {
            if (!Config.IsChecked("assCancelBase") && ObjectManager.Player.LSHasBuff("Recall"))
            {
                return;
            }

            var alliesToConsider =
                HeroManager.Allies.Where(
                    ally =>
                    !ally.IsMe && !ally.IsDead && !ally.LSInShop() && !ally.IsZombie && !ally.LSHasBuff("Recall")
                    && ally.LSDistance(ObjectManager.Player) > 2000);
            foreach (var ally in alliesToConsider)
            {
                var enemiesAroundAlly =
                    HeroManager.Enemies.Where(
                        enemy =>
                        enemy.LSDistance(ally) <= 1000 && enemy.LSDistance(ObjectManager.Player) > 2000 && !enemy.IsDead
                        && !enemy.IsZombie && enemy.IsHPBarRendered && enemy.IsValid);
                foreach (var enemy in enemiesAroundAlly)
                {
                    if (!Spells.R.LSIsReady())
                    {
                        return;
                    }

                    if (enemy.Health <= ally.GetAlliesDamageNearEnemy())
                    {
                        ObjectManager.Player.Spellbook.CastSpell(SpellSlot.R);
                    }
                }
            }
        }

        #endregion
    }
}