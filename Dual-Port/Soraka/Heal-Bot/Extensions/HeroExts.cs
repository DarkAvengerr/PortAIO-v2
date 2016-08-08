using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Soraka_HealBot.Extensions
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    public static class HeroExts
    {
        #region Public Methods and Operators

        public static double GetAlliesDamageNearEnemy(this AIHeroClient enemy, float percent = 0.8f, float range = 750)
        {
            var fullDmg = 0.0;
            var slots = new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
            foreach (var ally in HeroManager.Allies.Where(al => al.LSDistance(enemy) <= range))
            {
                fullDmg +=
                    ally.Spellbook.Spells.Where(spell => spell.LSIsReady() && slots.Contains(spell.Slot))
                        .Sum(spell => ally.LSGetSpellDamage(enemy, spell.Slot));
                fullDmg += ally.LSGetAutoAttackDamage(enemy);
            }

            return fullDmg * percent;
        }

        public static double GetEnemiesDamageNearAlly(this AIHeroClient ally, float percent = 0.8f, float range = 750)
        {
            var fullDmg = 0.0;
            var slots = new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
            foreach (var enemy in HeroManager.Enemies.Where(en => en.LSDistance(ally) <= range))
            {
                fullDmg +=
                    enemy.Spellbook.Spells.Where(spell => spell.LSIsReady() && slots.Contains(spell.Slot))
                        .Sum(spell => enemy.LSGetSpellDamage(ally, spell.Slot));
                fullDmg += enemy.LSGetAutoAttackDamage(ally);
            }

            return fullDmg * percent;
        }

        public static double GetTotalDamageFromLib(this AIHeroClient attacker, Obj_AI_Base defender)
        {
            var slots = new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
            var spellDmg =
                attacker.Spellbook.Spells.Where(spell => spell.LSIsReady() && slots.Contains(spell.Slot))
                    .Sum(spell => attacker.LSGetSpellDamage(defender, spell.Slot));
            var aaDmg = attacker.CanAttack ? attacker.LSGetAutoAttackDamage(defender) : 0f;
            var fulldmg = spellDmg + aaDmg;
            return fulldmg;
        }

        #endregion
    }
}