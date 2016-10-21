using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;
using LeagueSharp.Data.DataTypes;
using LeagueSharp.Data.Enumerations;
using LeagueSharp.SDK;
using SharpDX;
using Spell = LeagueSharp.SDK.Spell;


using EloBuddy; namespace ARAMDetFull.SpellsSDK
{
    //Big tnx to Trees ^^
    internal static class SpellUtility
    {
        public static bool IsSkillShot(this CastType[] castTypes)
        {
            var types = new List<CastType> { CastType.Direction, CastType.Position };
            return castTypes != null && types.Any(castTypes.Contains);
        }

        public static bool IsSkillShot(this SpellType type)
        {
            return type.Equals(SpellType.SkillshotCircle) || type.Equals(SpellType.SkillshotCone) ||
                   type.Equals(SpellType.SkillshotMissileCircle) || type.Equals(SpellType.SkillshotLine) ||
                   type.Equals(SpellType.SkillshotMissileLine);
        }

        public static LeagueSharp.Common.SkillshotType GetSkillshotType(this SpellType type)
        {
            switch (type)
            {
                case SpellType.SkillshotCircle:
                case SpellType.SkillshotMissileCircle:
                    return LeagueSharp.Common.SkillshotType.SkillshotCircle;
                case SpellType.SkillshotCone:
                    return LeagueSharp.Common.SkillshotType.SkillshotCone;
                default:
                    return LeagueSharp.Common.SkillshotType.SkillshotLine;
            }
        }

        public static bool IsCurrentSpell(this SpellDatabaseEntry entry)
        {
            var spell = ObjectManager.Player.Spellbook.GetSpell(entry.Slot);
            return string.Equals(spell.Name, entry.SpellName, StringComparison.CurrentCultureIgnoreCase);
        }
        
        public static void RenderCircle(Vector3 position, float radius, Color color)
        {
            Render.Circle.DrawCircle(position, radius, color.ToSystemColor());
        }
    }
}