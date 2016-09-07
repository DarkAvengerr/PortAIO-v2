using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DicasteAshe.Handlers
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Data;
    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;

    using SpellDatabase = LeagueSharp.Data.DataTypes.SpellDatabase;

    internal static class SpellHandler
    {
        internal static Spell E { get; set; }

        internal static Spell Q { get; set; }

        internal static Spell R { get; set; }

        internal static Spell W { get; set; }

        private static AIHeroClient Player { get; } = GameObjects.Player;

        internal static void Init()
        {
            Q = InitializeSpell(SpellSlot.Q);
            W = InitializeSpell(SpellSlot.W);
            E = InitializeSpell(SpellSlot.E);
            R = InitializeSpell(SpellSlot.R);
        }

        private static SpellDatabaseEntry GetSpellData(SpellSlot spellSlot)
        {
            var championSpells =
                Data.Get<SpellDatabase>().Spells.Where(entry => entry.ChampionName == Player.ChampionName).ToList();

            return championSpells.Single(entry => entry.Slot.Equals(spellSlot));
        }

        private static Spell InitializeSpell(SpellSlot spellSlot)
        {
            var spell = new Spell(spellSlot, GetSpellData(spellSlot).Range);

            var collision =
                GetSpellData(spellSlot)
                    .CollisionObjects.Any(
                        objects =>
                        objects.Equals(CollisionableObjects.Minions) || objects.Equals(CollisionableObjects.Heroes));

            switch (GetSpellData(spellSlot).SpellType)
            {
                case SpellType.SkillshotCircle:
                case SpellType.SkillshotMissileCircle:
                    SetSkillShotCircle(spellSlot, spell, collision);
                    break;

                case SpellType.SkillshotLine:
                case SpellType.SkillshotMissileLine:
                    SetSkillShotLine(spellSlot, spell, collision);
                    break;

                case SpellType.SkillshotCone:
                case SpellType.SkillshotMissileCone:
                    SetSkillShotCone(spellSlot, spell, collision);
                    break;

                case SpellType.Targeted:
                case SpellType.TargetedMissile:
                    SetSkillShotTargeted(spellSlot, spell);
                    break;
            }

            return spell;
        }

        private static void SetSkillShotCircle(SpellSlot spellSlot, Spell spell, bool collision)
        {
            spell.SetSkillshot(
                GetSpellData(spellSlot).Delay, 
                GetSpellData(spellSlot).Width, 
                GetSpellData(spellSlot).MissileSpeed, 
                collision, 
                SkillshotType.SkillshotCircle);
        }

        private static void SetSkillShotCone(SpellSlot spellSlot, Spell spell, bool collision)
        {
            spell.SetSkillshot(
                GetSpellData(spellSlot).Delay, 
                GetSpellData(spellSlot).Width, 
                GetSpellData(spellSlot).MissileSpeed, 
                collision, 
                SkillshotType.SkillshotCone);
        }

        private static void SetSkillShotLine(SpellSlot spellSlot, Spell spell, bool collision)
        {
            spell.SetSkillshot(
                GetSpellData(spellSlot).Delay, 
                GetSpellData(spellSlot).Width, 
                GetSpellData(spellSlot).MissileSpeed, 
                collision, 
                SkillshotType.SkillshotLine);
        }

        private static void SetSkillShotTargeted(SpellSlot spellslot, Spell spell)
        {
            spell.SetTargetted(GetSpellData(spellslot).Delay, GetSpellData(spellslot).MissileSpeed);
        }
    }
}