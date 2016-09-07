using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Warwick
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
            Game.OnUpdate += Warwick.OnUpdate;
            Events.OnInterruptableTarget += Warwick.OnInterruptableTarget;
        }

        #endregion
    }
}