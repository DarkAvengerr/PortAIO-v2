using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Core
{
    internal class Spells : Core
    {
        public static SpellSlot Ignite, Flash;
        public static Spell Q { get; set; }
        public static Spell W { get; set; }
        public static Spell E { get; set; }
        public static Spell R { get; set; }

        public static void Load()
        {
            Q = new Spell(SpellSlot.Q, 260f);
            W = new Spell(SpellSlot.W, 250f);
            E = new Spell(SpellSlot.E, 270);
            R = new Spell(SpellSlot.R, 900);

            Q.SetSkillshot(0.25f, 100f, 2200f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, (float)(45 * 0.5), 1600, false, SkillshotType.SkillshotCone);

            Ignite = Player.GetSpellSlot("SummonerDot");
            Flash = Player.GetSpellSlot("SummonerFlash");
        }
    }
}
