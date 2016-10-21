using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Guardians
{
    #region Using Directives

    using System;

    #endregion

    /// <summary>
    ///     Guardian
    /// </summary>
    public class GuardianBase
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="GuardianBase" /> is negated.
        /// </summary>
        /// <value>
        ///     <c>true</c> if negated; otherwise, <c>false</c>.
        /// </value>
        public bool Negated { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the function.
        /// </summary>
        /// <value>
        ///     The function.
        /// </value>
        protected Func<bool> Func { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Checks this instance.
        /// </summary>
        /// <returns></returns>
        public bool Check()
        {
            return this.Negated ? !this.Func.Invoke() : this.Func.Invoke();
        }

        #endregion
    }
}