using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace LCS_LeBlanc.Extensions
{
    static class Spells
    {
        /// <summary>
        /// Thats provide Q spell
        /// </summary>
        public static Spell Q;
        /// <summary>
        /// Thats provide W spell
        /// </summary>
        public static Spell W;
        /// <summary>
        /// Thats provide E spell
        /// </summary>
        public static Spell E;
        /// <summary>
        /// Thats provide R spell
        /// </summary>
        public static Spell R;

        /// <summary>
        /// Initialize all spells
        /// </summary>
        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 710);
            W = new Spell(SpellSlot.W, 750);
            E = new Spell(SpellSlot.E, 950);

            E.SetSkillshot(0.25f, 70, 1600, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0, 70, 1500, false, SkillshotType.SkillshotLine);

            if (Utilities.UltimateKey() == "Q")
            {
                R = new Spell(SpellSlot.R, 710);
            }
            else if (Utilities.UltimateKey() == "W")
            {
                R = new Spell(SpellSlot.R, 750);
                R.SetSkillshot(0, 70, 1500, false, SkillshotType.SkillshotLine);
            }
            else if (Utilities.UltimateKey() == "E")
            {
                R = new Spell(SpellSlot.R, 950);
                R.SetSkillshot(0.25f, 70, 1600, true, SkillshotType.SkillshotLine);
            }
            else if (Utilities.UltimateKey() == "none")
            {
                R = new Spell(SpellSlot.R, 0);
            }
        }
        /// <summary>
        /// Thats provide selected spell damage
        /// </summary>
        /// <param name="spell">Spell</param>
        /// <param name="unit">Target</param>
        /// <returns></returns>
        public static int GetDamage(this Spell spell, AIHeroClient unit)
        {
            return (int)spell.GetDamage(unit);
        }
    }
}