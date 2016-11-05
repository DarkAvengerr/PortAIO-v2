using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Base.Modules.Assembly
{
    #region Using Directives

    using System;

    using CommonEx;
    using CommonEx.Classes;

    using global::YasuoMedia.CommonEx.Utility;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using PortAIO.Properties;

    #endregion

    internal class Notifications : FeatureChild<Assembly>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Debug" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public Notifications(Assembly parent)
            : base(parent)
        {
            this.OnLoad();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name => "Notifications";

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable()
        {
            Events.OnPostUpdate -= OnPostUpdate;
            GlobalVariables.Debug = false;
            base.OnDisable();
        }

        /// <summary>
        ///     Called when [enable].
        /// </summary>
        protected override void OnEnable()
        {
            Events.OnPostUpdate += OnPostUpdate;
            base.OnEnable();
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected sealed override void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);

            GlobalVariables.CastManager = new CommonEx.CastManager.CastManager();

            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(false));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        /// <summary>
        /// Raises the <see cref="E:PostUpdate" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void OnPostUpdate(EventArgs args)
        {
            GlobalVariables.CastManager.Process();
            //GlobalVariables.CastManager.Clear();
        }

        #endregion

        /// <summary>
        ///     Method to display a banner and a text
        /// </summary>
        /// <param name="name">Name of the assembly</param>
        /// <param name="version">Version of the assembly</param>
        /// <param name="displayTime">Time of displaying</param>
        private static void DrawBanner(String name, int version, int displayTime)
        {
            LeagueSharp.Common.Notifications.AddNotification(
                String.Format("[{0}] {1} - loaded successfully!", name, version),
                displayTime,
                true);

            if (Game.Time <= 6000000)
            {
                var banner = new Render.Sprite(Resources.BannerLoading, new Vector2())
                                 {
                                     Scale =
                                         new Vector2(
                                         1 / (Drawing.Width / 3),
                                         1 / (Drawing.Height / 3))
                                         .Normalized()
                                 };

                // centered but a little above the screens center

                var position = new Vector2(
                    (Drawing.Width / 2) - banner.Width / 2,
                    (Drawing.Height / 2) - banner.Height / 2 - 50);
                banner.Position = position;

                banner.Add(0);

                banner.OnDraw();

                //LeagueSharp.Common.Utility.DelayAction.Add(displayTime, () => banner.Remove());
            }
        }
    }
}