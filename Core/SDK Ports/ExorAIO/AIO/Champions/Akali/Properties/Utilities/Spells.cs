using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Akali
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
        ///     Initializes the spells.
        /// </summary>
        public static void Initialize()
        {
            Vars.Q = new Spell(SpellSlot.Q, 600f);
            Vars.W = new Spell(SpellSlot.W, 700f);
            Vars.E = new Spell(SpellSlot.E, 325f);
            Vars.R = new Spell(SpellSlot.R, 700f);
            Vars.W.SetSkillshot(0.25f, 400f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        #endregion
    }
}