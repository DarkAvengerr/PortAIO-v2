using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.DamageCalculator.DamageTypes
{
    public class DamageType
    {
        #region Fields

        /// <summary>
        /// The DMG type
        /// </summary>
        private IDamageType dmgType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageType"/> class.
        /// </summary>
        /// <param name="dmgType">Type of the DMG.</param>
        public DamageType(IDamageType dmgType)
        {
            this.dmgType = dmgType;
        }

        #endregion
    }
}