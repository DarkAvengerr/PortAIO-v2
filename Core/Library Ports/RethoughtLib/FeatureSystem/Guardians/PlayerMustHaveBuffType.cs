using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Guardians
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;

    #endregion

    public class PlayerMustHaveBuffType : GuardianBase
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerMustHaveBuffType" /> class.
        /// </summary>
        /// <param name="buffType">Type of the buff.</param>
        public PlayerMustHaveBuffType(BuffType buffType)
        {
            this.Func = () => ObjectManager.Player.HasBuffOfType(buffType);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerMustHaveBuffType" /> class.
        /// </summary>
        /// <param name="bufftypes">The bufftypes.</param>
        public PlayerMustHaveBuffType(IEnumerable<BuffType> bufftypes)
        {
            this.Func = () => bufftypes.All(buffType => ObjectManager.Player.HasBuffOfType(buffType));
        }

        #endregion
    }
}