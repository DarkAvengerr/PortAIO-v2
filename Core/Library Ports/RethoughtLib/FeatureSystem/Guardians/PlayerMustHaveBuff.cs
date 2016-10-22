using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Guardians
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;

    #endregion

    public class PlayerMustHaveBuff : GuardianBase
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerMustHaveBuff" /> class.
        /// </summary>
        /// <param name="buff">The buff.</param>
        public PlayerMustHaveBuff(string buff)
        {
            this.Func = () => ObjectManager.Player.HasBuff(buff);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerMustHaveBuff" /> class.
        /// </summary>
        /// <param name="buffs">The buffs.</param>
        public PlayerMustHaveBuff(IEnumerable<string> buffs)
        {
            this.Func = () => buffs.All(buff => ObjectManager.Player.HasBuff(buff));
        }

        #endregion
    }
}