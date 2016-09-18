using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Guardians
{
    #region Using Directives

    using System;

    #endregion

    public class Custom : GuardianBase
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Custom" /> class.
        /// </summary>
        /// <param name="func">The function.</param>
        public Custom(Func<bool> func)
        {
            this.Func = func;
        }

        #endregion
    }
}