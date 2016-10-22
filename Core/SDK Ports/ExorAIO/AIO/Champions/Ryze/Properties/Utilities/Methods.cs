using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Ryze
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
        ///     The methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Ryze.OnUpdate;
            Events.OnGapCloser += Ryze.OnGapCloser;
            Variables.Orbwalker.OnAction += Ryze.OnAction;
        }

        #endregion
    }
}