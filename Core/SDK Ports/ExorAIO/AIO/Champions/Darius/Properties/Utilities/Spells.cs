using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Darius
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
            Vars.Q = new Spell(SpellSlot.Q, 425f);
            Vars.W = new Spell(SpellSlot.W);
            Vars.E = new Spell(SpellSlot.E, 550f);
            Vars.R = new Spell(SpellSlot.R, 460f);
            Vars.E.SetSkillshot(0.25f, (float)(80f * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);
        }

        #endregion
    }
}