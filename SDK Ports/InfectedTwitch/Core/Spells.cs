#region

using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Infected_Twitch.Core
{
    internal class Spells
    {
     //   public static SpellSlot Recall { get; set; }
        public static SpellSlot Ignite { get; set; }
        public static Spell Q { get; set; }
        public static Spell W { get; set; }
        public static Spell E { get; set; }
        public static Spell R { get; set; }

        public static void Load()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 1200);
            R = new Spell(SpellSlot.R, 900);

            W.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotCircle);

            Ignite = GameObjects.Player.GetSpellSlot("summonerDot");
        }
    }
}
