using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Presets
{
    #region Using Directives

    using System;

    using global::RethoughtLib.Events;
    using global::RethoughtLib.FeatureSystem.Abstract_Classes;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    // TODO: PRIORITY LOW > Notify with a sound and a notification banner

    internal class VersionChecker : ChildBase
    {
        #region Fields

        public global::RethoughtLib.Utility.VersionChecker ImplementationVersionChecker;

        /// <summary>
        ///     last checked
        /// </summary>
        private float lastChecked;

        /// <summary>
        ///     notified
        /// </summary>
        private bool notified;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="System.Version" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="githubPath"></param>
        /// <param name="assemblyName"></param>
        public VersionChecker(string githubPath, string assemblyName)
        {
            this.GithubPath = githubPath;
            this.AssemblyName = assemblyName;

            this.ImplementationVersionChecker = new global::RethoughtLib.Utility.VersionChecker(this.GithubPath);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name of the assembly.
        /// </summary>
        /// <value>
        ///     The name of the assembly.
        /// </value>
        public string AssemblyName { get; set; }

        /// <summary>
        ///     Gets or sets the github path.
        /// </summary>
        /// <value>
        ///     The github path.
        /// </value>
        public string GithubPath { get; set; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "VersionChecker";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Draws this instance.
        /// </summary>
        public void Draw()
        {
            // TODO: PRIORITY LOW
        }

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnUpdate(EventArgs args)
        {
            if (Game.Time - 300 < this.lastChecked)
            {
                return;
            }

            this.ImplementationVersionChecker.Check();

            if (this.ImplementationVersionChecker.UpdateAvailable)
            {
                if (this.ImplementationVersionChecker.ForceUpdate)
                {
                    Notifications.AddNotification(this.AssemblyName + "- IMPORTANT UPDATE!", dispose: false);
                }
                else
                {
                    if (this.Menu.Item(this.Name + "NotifyNewVersion").GetValue<bool>())
                    {
                        if (!this.notified)
                        {
                            Notifications.AddNotification(this.AssemblyName + "- Update Available!", 2500);

                            this.notified = true;
                            this.Draw();
                        }
                    }
                }
                this.Menu.Item(this.Name + "Version").DisplayName = "Version is outdated";
            }
            this.lastChecked = Game.Time;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Events.OnUpdate -= this.OnUpdate;
        }

        /// <summary>
        ///     Called when [enable].
        /// </summary>
        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Events.OnUpdate += this.OnUpdate;
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.Menu.AddItem(
                this.ImplementationVersionChecker.UpdateAvailable
                    ? new MenuItem(this.Name + "Version", "Version: " + 1337)
                    : new MenuItem(this.Name + "Version", "Version is outdated"));

            this.Menu.AddItem(
                new MenuItem(this.Name + "LiveCheck", "Check For new Version").SetValue(true)
                    .SetTooltip(
                        "If this is enabled, the assembly will look every few minutes if a newer Version is available"));

            this.Menu.AddItem(
                new MenuItem(this.Name + "NotifyNewVersion", "Notify if new Version available").SetValue(true)
                    .SetTooltip(
                        "If this is enabled, the assembly will notify you when a new version is available. It will always inform you about important updates."));

            this.lastChecked = Game.Time;
        }

        #endregion
    }
}