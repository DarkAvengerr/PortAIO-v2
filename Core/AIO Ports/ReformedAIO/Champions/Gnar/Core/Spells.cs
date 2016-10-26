namespace ReformedAIO.Champions.Gnar.Core
{
    using EloBuddy;
    using LeagueSharp.Common;

    internal sealed class Spells
    {
        public static Spell Q { get; set; }

        public static Spell Q2 { get; set; }

        public static Spell W2 { get; set; }

        public static Spell E { get; set; }

        public static Spell E2 { get; set; }

        public static Spell R2 { get; set; }

        public void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 1100);
            Q2 = new Spell(SpellSlot.Q, 1100);

            W2 = new Spell(SpellSlot.W, 525f);

            E = new Spell(SpellSlot.E, 475);
            E2 = new Spell(SpellSlot.E);

            R2 = new Spell(SpellSlot.R, 590);

            Q.SetSkillshot(0.25f, 60, 1200, true, SkillshotType.SkillshotLine);
            Q2.SetSkillshot(0.25f, 80, 1200, true, SkillshotType.SkillshotLine);

            W2.SetSkillshot(0.25f, 80, int.MaxValue, false, SkillshotType.SkillshotLine);

            E.SetSkillshot(0.5f, 150, int.MaxValue, false, SkillshotType.SkillshotCircle);
            E2.SetSkillshot(0.5f, 150, int.MaxValue, false, SkillshotType.SkillshotCircle);

            R2.Delay = 0.25f;
        }
    }
}
