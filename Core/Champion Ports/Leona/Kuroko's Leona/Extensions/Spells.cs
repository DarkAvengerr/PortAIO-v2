using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Kuroko_s_Leona.Extensions
{
    internal static class Spells
    {
        public static Spell Q, W, E, R;

        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 175f);
            W = new Spell(SpellSlot.W, 275f);
            E = new Spell(SpellSlot.E, 875f);
            
            R = new Spell(SpellSlot.R, 1200f);

            E.SetSkillshot(0.25f,70f, 2000f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);

        }

    }
}
