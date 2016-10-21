using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Veigar
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
        ///     Sets the spells.
        /// </summary>
        public static void Initialize()
        {
            Vars.Q = new Spell(SpellSlot.Q, 950f);
            Vars.W = new Spell(SpellSlot.W, 900f);
            Vars.E = new Spell(SpellSlot.E, 700f);
            Vars.R = new Spell(SpellSlot.R, 650f);
            Vars.Q.SetSkillshot(0.25f, 70f, 2000f, false, SkillshotType.SkillshotLine);
            Vars.W.SetSkillshot(1.25f, 112.5f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Vars.E.SetSkillshot(0.5f, 375f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        #endregion
    }
}