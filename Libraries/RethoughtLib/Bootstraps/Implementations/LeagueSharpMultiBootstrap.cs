using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Bootstraps.Implementations
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using global::RethoughtLib.Bootstraps.Abstract_Classes;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    public class LeagueSharpMultiBootstrap : PlaySharpBootstrapBase
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LeagueSharpMultiBootstrap" /> class.
        /// </summary>
        public LeagueSharpMultiBootstrap(IEnumerable<LoadableBase> modules = null, IEnumerable<string> additionalStrings = null)
        {
            if (modules != null)
            {
                this.Modules = new List<LoadableBase>(modules);
            }

            if (additionalStrings != null)
            {
                this.Strings = new List<string>(additionalStrings);
            }

            CustomEvents.Game.OnGameLoad +=
                delegate(EventArgs args) { this.AddString(ObjectManager.Player.ChampionName); };
        }

        #endregion
    }
}