using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Ezreal
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
            Vars.Q = new Spell(SpellSlot.Q, 1150f);
            Vars.W = new Spell(SpellSlot.W, 1000f);
            Vars.E = new Spell(SpellSlot.E, 475f);
            Vars.R = new Spell(SpellSlot.R, 1500f);
            Vars.Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);
            Vars.W.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotLine);
            Vars.R.SetSkillshot(1.1f, 160f, 2000f, false, SkillshotType.SkillshotLine);
        }

        #endregion
    }
}