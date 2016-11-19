using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Anivia
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
        ///     Initializes the methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Anivia.OnUpdate;
            GameObject.OnCreate += Anivia.OnCreate;
            GameObject.OnDelete += Anivia.OnDelete;
            Events.OnGapCloser += Anivia.OnGapCloser;
            Events.OnInterruptableTarget += Anivia.OnInterruptableTarget;
        }

        #endregion
    }
}