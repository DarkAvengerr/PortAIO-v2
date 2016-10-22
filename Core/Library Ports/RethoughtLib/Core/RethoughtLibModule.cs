using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Core
{
    using global::RethoughtLib.FeatureSystem.Abstract_Classes;
    using global::RethoughtLib.VersionChecker.Implementations;

    internal sealed class RethoughtLibModule : ParentBase
    {
        public RethoughtLibModule(VersionCheckerModule versionCheckerModule = null)
        {
            var versionChecker = versionCheckerModule ?? new VersionCheckerModule("", this.Name);

            versionChecker.GithubPath =
                "https://github.com/MediaGithub/LeagueSharpDev/tree/master/LeagueSharp/RethoughtLib";

            this.Add(versionChecker);
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "RethoughtLib";

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);
        }
    }
}
