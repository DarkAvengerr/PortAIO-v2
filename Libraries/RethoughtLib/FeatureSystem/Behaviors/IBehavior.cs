using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Behaviors
{
    #region Using Directives

    using System;

    #endregion

    public interface IBehavior
    {
        #region Public Properties

        Action Action { get; set; }

        #endregion
    }
}