using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Amumu
{
    using LeagueSharp;

    /// <summary>
    ///     The methods class.
    /// </summary>
    internal class Methods
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Initializes the methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Amumu.OnUpdate;
        }

        #endregion
    }
}