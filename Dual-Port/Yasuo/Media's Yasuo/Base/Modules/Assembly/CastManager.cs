using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Base.Modules.Assembly
{
    #region Using Directives

    using System;

    using CommonEx;
    using CommonEx.Classes;

    using global::YasuoMedia.CommonEx.Utility;

    using LeagueSharp.Common;

    #endregion

    internal class CastManager : FeatureChild<Assembly>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CastManager" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public CastManager(Assembly parent)
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
        public override string Name => "Cast Manager (Priority System)";

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

            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

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
    }
}