using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Diana
{
    class Spells
    {
        public static SpellSlot Ignite, Flash;
        public static Spell Q, W, E, R;
        private static AIHeroClient Player = ObjectManager.Player;
        public static void Initialise()
        {
            Q = new Spell(SpellSlot.Q, 825f);
            Q.SetSkillshot(0.25f, 185, 1640, false, SkillshotType.SkillshotCone);

            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 450);
            R = new Spell(SpellSlot.R, 825);

            Ignite = Player.GetSpellSlot("SummonerDot");
            Flash = Player.GetSpellSlot("SummonerFlash");
        }
    }
}
