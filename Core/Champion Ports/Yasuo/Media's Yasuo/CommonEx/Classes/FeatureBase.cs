using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Classes
{
    using System;

    using global::YasuoMedia.CommonEx.Extensions;

    using LeagueSharp.Common;

    public abstract class FeatureBase
    {
        #region Public Events

        /// <summary>
        /// Occurs when [on disabled].
        /// </summary>
        public event EventHandler OnDisabled;

        /// <summary>
        /// Occurs when [on enabled].
        /// </summary>
        public event EventHandler OnEnabled;

        /// <summary>
        /// Occurs when [on initialized].
        /// </summary>
        public event EventHandler OnInitialized;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="FeatureBase"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public abstract bool Enabled { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FeatureBase"/> is initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if initialized; otherwise, <c>false</c>.
        /// </value>
        public bool Initialized { get; protected set; }

        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        /// <value>
        /// The menu.
        /// </value>
        public Menu Menu { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FeatureBase"/> is unloaded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if unloaded; otherwise, <c>false</c>.
        /// </value>
        public bool Unloaded { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Called when [disable].
        /// </summary>
        protected virtual void OnDisable()
        {
            if (this.Initialized && this.Enabled && !this.Unloaded)
            {
                this.OnDisabled.RaiseEvent(null, null);
            }
        }

        /// <summary>
        /// Called when [enable].
        /// </summary>
        protected virtual void OnEnable()
        {
            if (this.Unloaded)
            {
                return;
            }

            if (!this.Initialized)
            {
                this.OnInitialize();
            }

            if (this.Initialized && !this.Enabled)
            {
                this.OnEnabled.RaiseEvent(null, null);
            }
        }

        /// <summary>
        /// Called when [initialize].
        /// </summary>
        protected virtual void OnInitialize()
        {
            if (this.Initialized || this.Unloaded)
            {
                return;
            }

            this.OnInitialized.RaiseEvent(this, null);

            this.Initialized = true;
        }

        /// <summary>
        /// Called when [unload].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="UnloadEventArgs"/> instance containing the event data.</param>
        protected virtual void OnUnload(object sender, UnloadEventArgs args)
        {
            if (this.Unloaded)
            {
                return;
            }

            this.OnDisable();

            if (args != null && args.Final)
            {
                this.Unloaded = true;
            }
        }

        #endregion

        public class UnloadEventArgs : EventArgs
        {
            #region Fields

            public bool Final;

            #endregion

            #region Constructors and Destructors

            public UnloadEventArgs(bool final = false)
            {
                this.Final = final;
            }

            #endregion
        }
    }
}