using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Guardians
{
    #region Using Directives

    using LeagueSharp;

    #endregion

    public class PlayerMustNotBeWindingUp : GuardianBase
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerMustNotBeWindingUp" /> class.
        /// </summary>
        public PlayerMustNotBeWindingUp()
        {
            this.Func = () => ObjectManager.Player.Spellbook.IsAutoAttacking;
        }

        #endregion
    }
}