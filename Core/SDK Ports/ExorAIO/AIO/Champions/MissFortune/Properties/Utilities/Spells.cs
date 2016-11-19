using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.MissFortune
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
            Vars.Q = new Spell(SpellSlot.Q, 650f + GameObjects.Player.BoundingRadius);
            Vars.Q2 = new Spell(SpellSlot.Q, Vars.Q.Range + 500f - GameObjects.Player.BoundingRadius);
            Vars.W = new Spell(SpellSlot.W);
            Vars.E = new Spell(SpellSlot.E, 1000f);
            Vars.R = new Spell(SpellSlot.R, 1400f);
            Vars.Q.SetTargetted(0.5f, float.MaxValue);
            Vars.E.SetSkillshot(0.5f, 200f, 1600f, false, SkillshotType.SkillshotCircle);
        }

        #endregion
    }
}