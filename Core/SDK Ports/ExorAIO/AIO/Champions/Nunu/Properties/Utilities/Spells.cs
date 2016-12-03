using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Nunu
{
    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;

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
            Vars.Q = new Spell(SpellSlot.Q, GameObjects.Player.BoundingRadius + 125f);
            Vars.W = new Spell(SpellSlot.W, 700f);
            Vars.E = new Spell(SpellSlot.E, 550f);
            Vars.R = new Spell(SpellSlot.R, 650f);
        }

        #endregion
    }
}