using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Sona
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
            Vars.Q = new Spell(SpellSlot.Q, 825f);
            Vars.W = new Spell(SpellSlot.W, 1000f);
            Vars.E = new Spell(SpellSlot.E, 400f + GameObjects.Player.BoundingRadius / 2);
            Vars.R = new Spell(SpellSlot.R, 900f);
            Vars.R.SetSkillshot(0.25f, 140f, 2400f, false, SkillshotType.SkillshotLine);
        }

        #endregion
    }
}