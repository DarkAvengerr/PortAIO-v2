using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Varus_God
{
    internal class Spells
    {
        public static Spell Q { get; private set; }
        public static Spell W { get; private set; }
        public static Spell E { get; private set; }
        public static Spell R { get; private set; }
        public static SpellSlot Ignite { get; private set; }

        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 1625);
            Q.SetSkillshot(Q.Instance.SData.SpellCastTime, Q.Instance.SData.LineWidth, Q.Instance.SData.MissileSpeed,
                false, SkillshotType.SkillshotLine);
            Q.SetCharged("VarusQ", "VarusQ", 925, 1625, 1.0f);
            //Q.MinHitChance = HitChance.VeryHigh;
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 925);
            E.SetSkillshot(.50f, 250, 1400, false, SkillshotType.SkillshotCircle);
            R = new Spell(SpellSlot.R, 1075);
            R.SetSkillshot(.25f, 120, 1950, false, SkillshotType.SkillshotLine);

            Ignite = ObjectManager.Player.GetSpellSlot("summonerdot");
        }
    }
}