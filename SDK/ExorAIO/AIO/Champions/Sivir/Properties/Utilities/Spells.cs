using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Sivir
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
            Vars.Q = new Spell(SpellSlot.Q, 1200f);
            Vars.W = new Spell(SpellSlot.W, Vars.AaRange);
            Vars.E = new Spell(SpellSlot.E);
            Vars.Q.SetSkillshot(0.25f, 90f, 1350f, false, SkillshotType.SkillshotLine);
        }

        #endregion
    }
}