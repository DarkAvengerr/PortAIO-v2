using EloBuddy; namespace RethoughtLib.FeatureSystem.Switches
{
    #region Using Directives

    using System;

    using global::RethoughtLib.FeatureSystem.Abstract_Classes;

    using LeagueSharp.Common;

    #endregion

    public abstract class SwitchBase
    {
        #region Constructors and Destructors

        protected SwitchBase(Menu menu)
        {
            this.Menu = menu;
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when [on disable event].
        /// </summary>
        public event EventHandler<Base.FeatureBaseEventArgs> OnDisableEvent;

        /// <summary>
        ///     Occurs when [on enable event].
        /// </summary>
        public event EventHandler<Base.FeatureBaseEventArgs> OnEnableEvent;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Base" /> is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; set; }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Raises the <see cref="E:OnDisableEvent" /> event.
        /// </summary>
        /// <param name="e">The <see cref="Base.FeatureBaseEventArgs" /> instance containing the event data.</param>
        public virtual void OnOnDisableEvent(Base.FeatureBaseEventArgs e = null)
        {
            this.Enabled = false;

            this.OnDisableEvent?.Invoke(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="E:OnEnableEvent" /> event.
        /// </summary>
        /// <param name="e">The <see cref="Base.FeatureBaseEventArgs" /> instance containing the event data.</param>
        public virtual void OnOnEnableEvent(Base.FeatureBaseEventArgs e = null)
        {
            this.Enabled = true;

            this.OnEnableEvent?.Invoke(this, e);
        }

        /// <summary>
        ///     Setups this instance.
        /// </summary>
        public abstract void Setup();

        /// <summary>
        ///     Setups the specified menu.
        /// </summary>
        /// <param name="menu">The menu.</param>
        public void Setup(Menu menu)
        {
            this.Menu = menu;

            this.Setup();
        }

        #endregion
    }
}