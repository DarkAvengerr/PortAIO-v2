using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Nocturne
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
            Vars.W = new Spell(SpellSlot.W);
            Vars.E = new Spell(SpellSlot.E, 425f);
            Vars.R = new Spell(SpellSlot.R, 1750f + 750f * GameObjects.Player.Spellbook.GetSpell(SpellSlot.R).Level);

            Vars.Q.SetSkillshot(0.25f, 60f, 1400f, true, SkillshotType.SkillshotLine);
        }

        #endregion
    }
}