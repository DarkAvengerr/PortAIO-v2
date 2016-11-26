using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Corki
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
        ///     Sets the spells.
        /// </summary>
        public static void Initialize()
        {
            Vars.Q = new Spell(SpellSlot.Q, 825f);
            Vars.E = new Spell(SpellSlot.E, 600f + GameObjects.Player.BoundingRadius);
            Vars.R = new Spell(SpellSlot.R, 1250f);
            Vars.Q.SetSkillshot(0.55f, 250f, 1000f, false, SkillshotType.SkillshotCircle);
            Vars.E.SetSkillshot(0.3f, (float)(35f * Math.PI / 180), 1500f, false, SkillshotType.SkillshotCone);
            Vars.R.SetSkillshot(0.25f, 40f, 2000f, true, SkillshotType.SkillshotLine);
        }

        #endregion
    }
}