using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Tryndamere
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
            Vars.Q = new Spell(SpellSlot.Q);
            Vars.W = new Spell(SpellSlot.W, 400f);
            Vars.E = new Spell(SpellSlot.E, 660f);
            Vars.R = new Spell(SpellSlot.R);
            Vars.E.SetSkillshot(0.25f, 93f, 1300f, false, SkillshotType.SkillshotLine);
        }

        #endregion
    }
}