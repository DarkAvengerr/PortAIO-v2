using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Lux
{
    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;

    /// <summary>
    ///     The spell class.
    /// </summary>
    internal class Spells
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Sets the spells.
        /// </summary>
        public static void Initialize()
        {
            Vars.Q = new Spell(SpellSlot.Q, 1175f);
            Vars.W = new Spell(SpellSlot.W, 1075f);
            Vars.E = new Spell(SpellSlot.E, 1100f);
            Vars.R = new Spell(SpellSlot.R, 3340f);
            Vars.Q.SetSkillshot(0.50f, 90f, 1200f, true, SkillshotType.SkillshotLine);
            Vars.W.SetSkillshot(0.25f, 150f, 1200f, false, SkillshotType.SkillshotLine);
            Vars.E.SetSkillshot(0.75f, 275f, 1300f, false, SkillshotType.SkillshotCircle);
            Vars.R.SetSkillshot(1f, 190f, float.MaxValue, false, SkillshotType.SkillshotLine);
        }

        #endregion
    }
}