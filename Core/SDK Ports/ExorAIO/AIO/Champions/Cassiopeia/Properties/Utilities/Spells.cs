using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Cassiopeia
{
    using System;

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
            Vars.Q = new Spell(SpellSlot.Q, 850f);
            Vars.W = new Spell(SpellSlot.W, 900f);
            Vars.E = new Spell(SpellSlot.E, 700f + GameObjects.Player.BoundingRadius);
            Vars.R = new Spell(SpellSlot.R, 800f);
            Vars.Q.SetSkillshot(0.7f, 75f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Vars.W.SetSkillshot(0.5f, 125f, 2500f, false, SkillshotType.SkillshotCircle);
            Vars.R.SetSkillshot(0.8f, (float)(80 * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);
        }

        #endregion
    }
}