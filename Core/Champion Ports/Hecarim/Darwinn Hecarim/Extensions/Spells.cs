using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Darwinn_s.Extensions
{
    internal static class Spells
    {
        public static Spell Q, W, E, R;

        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 350);

            W = new Spell(SpellSlot.W);

            E = new Spell(SpellSlot.E, 600);

            R = new Spell(SpellSlot.R, 1000);
            R.SetSkillshot(0.25f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);

        }

    }
}