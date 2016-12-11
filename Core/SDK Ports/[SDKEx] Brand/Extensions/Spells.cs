using EloBuddy; 
using LeagueSharp.SDK; 
namespace Brand.Extensions
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;

    internal class Spells
    {
        public static Spell Q, W, E, R;

        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, 940);
            E = new Spell(SpellSlot.E, 625);
            R = new Spell(SpellSlot.R, 750);

            Q.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(1.15f, 230f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetTargetted(0.25f, 2000f);
        }
    }
}
