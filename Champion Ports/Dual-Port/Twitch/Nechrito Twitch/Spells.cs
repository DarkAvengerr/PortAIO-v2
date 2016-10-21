using LeagueSharp.Common;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Twitch
{
    internal class Spells
    {
        public static Spell Q { get; set; }
        public static Spell W { get; set; }
        public static Spell E { get; set; }
        public static Spell R { get; set; }

        public static void Initialise()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 1200);
            R = new Spell(SpellSlot.R, 900);

            W.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotCircle);
        }
    }
}
