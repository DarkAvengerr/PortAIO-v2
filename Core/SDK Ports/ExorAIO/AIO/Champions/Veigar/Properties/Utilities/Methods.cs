using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Veigar
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
            Game.OnUpdate += Veigar.OnUpdate;
            Events.OnGapCloser += Veigar.OnGapCloser;
            Events.OnInterruptableTarget += Veigar.OnInterruptableTarget;
            Variables.Orbwalker.OnAction += Veigar.OnAction;
        }

        #endregion
    }
}