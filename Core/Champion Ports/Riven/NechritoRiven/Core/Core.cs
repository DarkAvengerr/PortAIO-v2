using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Core
{
    #region

    using LeagueSharp;

    #endregion

    /// <summary>
    /// The core.
    /// </summary>
    internal class Core
    {
        #region Constants

        public const string IsFirstR = "RivenFengShuiEngine";

        public const string IsSecondR = "RivenIzunaBlade";

        #endregion

        #region Static Fields

        public static Orbwalking.Orbwalker Orbwalker;

        public static int Qstack = 1;

        public static float LastQ;

        public static bool IsGameObject = false;

        #endregion

        #region Public Properties

        public static AIHeroClient Player => ObjectManager.Player;

        #endregion
    }
}