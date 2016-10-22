using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Amumu
{
    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;

    /// <summary>
    ///     The spells class.
    /// </summary>
    internal class Spells
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Initializes the spells.
        /// </summary>
        public static void Initialize()
        {
            Vars.Q = new Spell(SpellSlot.Q, 1100f);
            Vars.W = new Spell(SpellSlot.W, 300f);
            Vars.E = new Spell(SpellSlot.E, 350f);
            Vars.R = new Spell(SpellSlot.R, 550f);
            Vars.Q.SetSkillshot(0.25f, 90f, 2000f, true, SkillshotType.SkillshotLine);
            Vars.W.SetSkillshot(0f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Vars.R.SetSkillshot(0.25f, 550f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        #endregion
    }
}