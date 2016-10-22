using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Dark_Star_Thresh.Core
{
    class Spells 
    {
        public static SpellSlot Ignite { get; set;}
        public static SpellSlot Flash { get; set; }

        public static Spell Q { get; set; }
        public static Spell W { get; set; }
        public static Spell E { get; set; }
        public static Spell R { get; set; }
        

        public static void Load()
        {
            Q = new Spell(SpellSlot.Q, 1100);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R, 450);

            Q.SetSkillshot(0.500f, 70, 1900f, true, SkillshotType.SkillshotLine);

            Ignite = ObjectManager.Player.GetSpellSlot("SummonerDot");
            Flash = ObjectManager.Player.GetSpellSlot("SummonerFlash");
        }
    }
}
