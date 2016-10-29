using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Caitlyn
{
    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;

    /// <summary>
    ///     The settings class.
    /// </summary>
    internal class Spells
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Initializes the spells.
        /// </summary>
        public static void Initialize()
        {
            Vars.Q = new Spell(SpellSlot.Q, 1250f);
            Vars.Q2 = new Spell(SpellSlot.Q, 1250f);
            Vars.W = new Spell(SpellSlot.W, 800f);
            Vars.E = new Spell(SpellSlot.E, 950f);
            Vars.R = new Spell(SpellSlot.R, 1500f + 500f * GameObjects.Player.Spellbook.GetSpell(SpellSlot.R).Level);
            Vars.Q.SetSkillshot(0.65f, 60f, 2200f, false, SkillshotType.SkillshotLine);
            Vars.Q2.SetSkillshot(0.65f, Vars.Q.Width * 2, 2200f, false, SkillshotType.SkillshotLine);
            Vars.W.SetSkillshot(1.35f, 67.5f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Vars.E.SetSkillshot(0.30f, 70f, 2000f, true, SkillshotType.SkillshotLine);
        }

        #endregion
    }
}