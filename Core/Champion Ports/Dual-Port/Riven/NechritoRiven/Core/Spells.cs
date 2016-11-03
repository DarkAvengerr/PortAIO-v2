using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Core
{
    #region

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class Spells : Core
    {
        #region Static Fields

        public static SpellSlot Flash;

        public static SpellSlot Ignite;

        #endregion

        #region Public Properties

        public static Spell E { get; set; }

        public static Spell Q { get; set; }

        public static Spell R { get; set; }

        public static Spell W { get; set; }

        #endregion

        #region Public Methods and Operators

        public static void Load()
        {
            Q = new Spell(SpellSlot.Q, 260);
            W = new Spell(SpellSlot.W, Player.AttackRange);
            E = new Spell(SpellSlot.E, 270);
            R = new Spell(SpellSlot.R, 900);

            Q.SetSkillshot(0.25f, 100f, 2200f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, (float)(45 * 0.5), 1600, false, SkillshotType.SkillshotCone);

            Ignite = Player.GetSpellSlot("SummonerDot");
            Flash = Player.GetSpellSlot("SummonerFlash");
        }

        #endregion
    }
}