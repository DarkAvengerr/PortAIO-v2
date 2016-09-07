using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Ashe
{
    using System;

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
            Vars.Q = new Spell(SpellSlot.Q);
            Vars.W = new Spell(SpellSlot.W, GameObjects.Player.BoundingRadius + 1200f);
            Vars.E = new Spell(SpellSlot.E, 2000f);
            Vars.R = new Spell(SpellSlot.R, 2000f);

            Vars.W.SetSkillshot(0.25f, (float)(67.5f * Math.PI / 180), 1500f, true, SkillshotType.SkillshotCone);
            Vars.E.SetSkillshot(0.25f, 130f, 1600f, false, SkillshotType.SkillshotLine);
            Vars.R.SetSkillshot(0.25f, 130f, 1600f, false, SkillshotType.SkillshotLine);
        }

        #endregion
    }
}