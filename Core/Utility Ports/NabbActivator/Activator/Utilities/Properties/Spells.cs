using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace NabbActivator
{
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
            Vars.W = new Spell(SpellSlot.W);
            Vars.Smite = new Spell(SpellSlots.GetSmiteSlot(), 500f + GameObjects.Player.BoundingRadius);
        }

        #endregion
    }
}