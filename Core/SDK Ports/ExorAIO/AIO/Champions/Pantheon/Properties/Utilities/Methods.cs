using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Pantheon
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
            Game.OnUpdate += Pantheon.OnUpdate;
            Events.OnInterruptableTarget += Pantheon.OnInterruptableTarget;
            Variables.Orbwalker.OnAction += Pantheon.OnAction;
        }

        #endregion
    }
}