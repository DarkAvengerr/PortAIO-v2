using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.Modules.Drawings
{
    using System;
    using System.Drawing;

    using CommonEx.Classes;
    using global::YasuoMedia.Yasuo.LogicProvider;

    using LeagueSharp;
    using LeagueSharp.Common;

    // TODO: Draw on minimap & menu settings (min units left, line thinkess, only if enemies around, color)
    internal class Flow : FeatureChild<Drawings>
    {
        #region Fields

        /// <summary>
        ///     The Flow logicprovider
        /// </summary>
        public FlowLogicProvider ProviderP;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Flow" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public Flow(Drawings parent)
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
        public override string Name => "Flow";

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable()
        {
            Drawing.OnDraw -= this.OnDraw;
            base.OnDisable();
        }

        /// <summary>
        ///     Called when [enable].
        /// </summary>
        protected override void OnEnable()
        {
            Drawing.OnDraw += this.OnDraw;
            base.OnEnable();
        }

        /// <summary>
        ///     Called when [initialize].
        /// </summary>
        protected override void OnInitialize()
        {
            this.ProviderP = new FlowLogicProvider();
            base.OnInitialize();
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected sealed override void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);

            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        private void OnDraw(EventArgs args)
        {
            if (GlobalVariables.Player.IsDead || (int)GlobalVariables.Player.ManaPercent == 100)
            {
                return;
            }

            if (this.ProviderP.GetRemainingUnits() > 0)
            {
                Render.Circle.DrawCircle(
                    GlobalVariables.Player.Position,
                    this.ProviderP.GetRemainingUnits(),
                    Color.White);
            }
        }

        #endregion
    }
}