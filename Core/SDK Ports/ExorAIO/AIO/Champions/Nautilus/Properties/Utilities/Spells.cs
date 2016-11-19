using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Nautilus
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
            Vars.W = new Spell(SpellSlot.W);
            Vars.E = new Spell(SpellSlot.E, 600f);
            Vars.R = new Spell(SpellSlot.R, 825f);
            Vars.Q.SetSkillshot(0.25f, 90f, 2000f, true, SkillshotType.SkillshotLine);
        }

        #endregion
    }
}