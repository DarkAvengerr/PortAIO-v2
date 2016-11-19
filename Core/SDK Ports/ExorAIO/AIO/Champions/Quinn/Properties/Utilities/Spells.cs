using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Quinn
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
            Vars.Q = new Spell(SpellSlot.Q, 1025f);
            Vars.W = new Spell(SpellSlot.W, 2100f);
            Vars.E = new Spell(SpellSlot.E, 675f + GameObjects.Player.BoundingRadius);
            Vars.R = new Spell(SpellSlot.R);
            Vars.Q.SetSkillshot(0.25f, 90f, 1550f, true, SkillshotType.SkillshotLine);
        }

        #endregion
    }
}