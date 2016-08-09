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
            if (!Config.IsChecked("assCancelBase") && ObjectManager.Player.HasBuff("Recall"))
            {
                return;
            }

            var alliesToConsider =
                HeroManager.Allies.Where(
                    ally =>
                    !ally.IsMe && !ally.IsDead && !ally.InShop() && !ally.IsZombie && !ally.HasBuff("Recall")
                    && ally.Distance(ObjectManager.Player) > 2000);
            foreach (var ally in alliesToConsider)
            {
                var enemiesAroundAlly =
                    HeroManager.Enemies.Where(
                        enemy =>
                        enemy.Distance(ally) <= 1000 && enemy.Distance(ObjectManager.Player) > 2000 && !enemy.IsDead
                        && !enemy.IsZombie && enemy.IsHPBarRendered && enemy.IsValid);
                foreach (var enemy in enemiesAroundAlly)
                {
                    if (!Spells.R.IsReady())
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