using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Cassiopeia
{
    using LeagueSharp;
    using LeagueSharp.SDK;

    /// <summary>
    ///     The methods class.
    /// </summary>
    internal class Methods
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Sets the methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Cassiopeia.OnUpdate;
            Events.OnGapCloser += Cassiopeia.OnGapCloser;
            Spellbook.OnCastSpell += Cassiopeia.OnCastSpell;
            Events.OnInterruptableTarget += Cassiopeia.OnInterruptableTarget;
            Variables.Orbwalker.OnAction += Cassiopeia.OnAction;
        }

        #endregion
    }
}