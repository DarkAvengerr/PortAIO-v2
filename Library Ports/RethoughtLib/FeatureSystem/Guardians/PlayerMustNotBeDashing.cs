using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Guardians
{
    #region Using Directives

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    public class PlayerMustBeDashing : GuardianBase
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerMustBeDashing" /> class.
        /// </summary>
        public PlayerMustBeDashing()
        {
            this.Func = () => !ObjectManager.Player.IsDashing();
        }

        #endregion
    }
}