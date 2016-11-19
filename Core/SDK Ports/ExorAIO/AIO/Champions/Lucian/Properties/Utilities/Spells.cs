using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Lucian
{
    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;

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
            Vars.Q = new Spell(SpellSlot.Q, GameObjects.Player.BoundingRadius * 4 + 500f);
            Vars.Q2 = new Spell(SpellSlot.Q, 900f);
            Vars.W = new Spell(SpellSlot.W, 900f);
            Vars.E = new Spell(SpellSlot.E, GameObjects.Player.GetRealAutoAttackRange() + 475f);
            Vars.R = new Spell(SpellSlot.R, 1150f);
            Vars.Q2.SetSkillshot(0.35f, 65f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Vars.W.SetSkillshot(0.30f, 80f, 1600f, true, SkillshotType.SkillshotLine);
            Vars.R.SetSkillshot(0.25f, 110f, 2500f, false, SkillshotType.SkillshotLine);
        }

        #endregion
    }
}