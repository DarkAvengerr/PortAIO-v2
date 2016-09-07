using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcBlitzcrank
{
    internal static class SpellManager
    {
        internal static readonly Spell Q, W, E, R;

        static SpellManager()
        {
            Q = new Spell(SpellSlot.Q, Config.Misc.QRange.QMaxRange)
            {
                MinHitChance = Config.Hitchance.QHitChance
            };
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 550f);

            Q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.SkillshotLine);
        }

        internal static void Initialize() { }
    }
}
