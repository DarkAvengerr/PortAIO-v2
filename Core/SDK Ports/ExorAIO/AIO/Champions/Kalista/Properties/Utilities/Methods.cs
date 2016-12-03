using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Kalista
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
            Game.OnUpdate += Kalista.OnUpdate;
            Variables.Orbwalker.OnAction += Kalista.OnAction;
        }

        #endregion
    }
}