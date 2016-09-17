using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hYasuo.Extensions
{
    internal static class Spells
    {
        // <summary>
        /// Thats provide Q spell
        /// </summary>
        public static Spell Q;
        // <summary>
        /// Thats provide Q spell
        /// </summary>
        public static Spell Q1;
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
            Q = new Spell(SpellSlot.Q, 450f);
            Q1 = new Spell(SpellSlot.Q, 1100f);
            W = new Spell(SpellSlot.W, 400f);
            E = new Spell(SpellSlot.E, 475f);
            R = new Spell(SpellSlot.R, 1250f);

            Q.SetSkillshot(0.4f, 20f, 1000f, false, SkillshotType.SkillshotLine);
            Q1.SetSkillshot(0.4f, 90f, 1000f, false, SkillshotType.SkillshotLine); 
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
