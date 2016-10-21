using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Classes
{
    using LeagueSharp.Common;

    public abstract class FeatureChild<T> : FeatureBase, IFeatureChild
        where T : FeatureParent
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureFeatureChild{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        protected FeatureChild(T parent)
        {
            this.Parent = parent;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="FeatureFeatureChild{T}"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public override bool Enabled
        {
            get
            {
                return !this.Unloaded && this.Parent != null && this.Parent.Enabled
                       && this.Menu?.Item(this.Menu.Name + "Enabled") != null
                       && this.Menu.Item(this.Menu.Name + "Enabled").GetValue<bool>();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FeatureFeatureChild{T}"/> is handled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if handled; otherwise, <c>false</c>.
        /// </value>
        public bool Handled { get; protected set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public T Parent { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Handles the events.
        /// </summary>
        public void HandleEvents()
        {
            if (this.Parent?.Menu == null || this.Menu == null || this.Handled)
            {
                return;
            }

            this.Parent.Menu.Item(this.Parent.Name + "Enabled").ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs args)
                    {
                        if (!this.Unloaded && args.GetNewValue<bool>())
                        {
                            if (this.Menu.Item(this.Menu.Name + "Enabled").GetValue<bool>())
                            {
                                this.OnEnable();
                            }
                        }
                        else
                        {
                            this.OnDisable();
                        }
                    };

            this.Menu.Item(this.Menu.Name + "Enabled").ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs args)
                    {
                        if (!this.Unloaded && args.GetNewValue<bool>())
                        {
                            if (this.Parent.Menu.Item(this.Parent.Name + "Enabled").GetValue<bool>())
                            {
                                this.OnEnable();
                            }
                        }
                        else
                        {
                            this.OnDisable();
                        }
                    };

            if (this.Enabled)
            {
                this.OnEnable();
            }

            this.Handled = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when [load].
        /// </summary>
        protected abstract void OnLoad();

        #endregion
    }
}