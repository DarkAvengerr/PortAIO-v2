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
    internal partial class Core
    {
        #region Constants

        public const string IsFirstR = "RivenFengShuiEngine";

        public const string IsSecondR = "RivenIzunaBlade";

        #endregion

        #region Static Fields

        public static Orbwalking.Orbwalker Orbwalker;

        public static int Qstack = 1;

        #endregion

        #region Public Properties

        public static AIHeroClient Player => ObjectManager.Player;

        #endregion
    }
}