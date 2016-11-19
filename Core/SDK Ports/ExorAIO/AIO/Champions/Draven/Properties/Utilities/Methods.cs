using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Draven
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
            Game.OnUpdate += Draven.OnUpdate;
            Events.OnGapCloser += Draven.OnGapCloser;
            Events.OnInterruptableTarget += Draven.OnInterruptableTarget;
        }

        #endregion
    }
}