using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Orianna
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
            Vars.Q = new Spell(SpellSlot.Q, 825f);
            Vars.W = new Spell(SpellSlot.W, 250f);
            Vars.E = new Spell(SpellSlot.E, 1100f);
            Vars.R = new Spell(SpellSlot.R, 325f + GameObjects.Player.BoundingRadius);
            Vars.Q.SetSkillshot(0.35f, 175f, 1150f, false, SkillshotType.SkillshotLine);
        }

        #endregion
    }
}