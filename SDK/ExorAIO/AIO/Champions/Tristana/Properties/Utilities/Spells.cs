using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Tristana
{
    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;

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
            Vars.Q = new Spell(SpellSlot.Q);
            Vars.W = new Spell(SpellSlot.W, 900f);
            Vars.E = new Spell(SpellSlot.E, Vars.AaRange);
            Vars.R = new Spell(SpellSlot.R, Vars.AaRange);
        }

        #endregion
    }
}