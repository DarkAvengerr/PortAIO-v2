using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcAshe
{
    internal static class SpellManager
    {
        internal static readonly Spell Q, W, E, R;

        static SpellManager()
        {
            Q = new Spell(SpellSlot.Q, 100f);
            W = new Spell(SpellSlot.W, 1200f);
            E = new Spell(SpellSlot.E, 300f);
            R = new Spell(SpellSlot.R, 2500f);

            W.SetSkillshot(0.25f, 60f, 1500f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 130f, 1600f, false, SkillshotType.SkillshotLine);
        }

        internal static void Initialize() { }
    }
}
