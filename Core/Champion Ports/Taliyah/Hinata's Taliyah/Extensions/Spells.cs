using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Hinata_s_Taliyah.Extensions
{
    internal static class Spells
    {
        public static Spell Q, W, E, R;

        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 900f);
            W = new Spell(SpellSlot.W, 800f);
            E = new Spell(SpellSlot.E, 700f);

            Q.SetSkillshot(0f, 60f, Q.Instance.SData.MissileSpeed, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(1f, 50f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 150f, 2000f, false, SkillshotType.SkillshotLine);

        }

    }
}
