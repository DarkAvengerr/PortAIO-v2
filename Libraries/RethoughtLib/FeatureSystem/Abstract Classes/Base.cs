using EloBuddy; namespace RethoughtLib.FeatureSystem.Abstract_Classes
{
    #region Using Directives

    using System;

    using global::RethoughtLib.FeatureSystem.Implementations;
    using global::RethoughtLib.FeatureSystem.Switches;

    using LeagueSharp.Common;

    #endregion

    public abstract class Base
    {
        #region Fields

        public SwitchBase Switch;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Base" /> class.
        /// </summary>
        protected Base()
        {
            this.OnInitializeEvent += this.OnCoreInitialize;
            this.OnInitializeEvent += this.OnInitialize;
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when [on initialize event].
        /// </summary>
        public event EventHandler<FeatureBaseEventArgs> OnInitializeEvent;

        /// <summary>
        ///     Occurs when [on load event].
        /// </summary>
        public event EventHandler<FeatureBaseEventArgs> OnLoadEvent;

        /// <summary>
        ///     Occurs when [on refresh event].
        /// </summary>
        public event EventHandler<FeatureBaseEventArgs> OnRefreshEvent;

        /// <summary>
        ///     Occurs when [on initialize event].
        /// </summary>
        public event EventHandler<FeatureBaseEventArgs> OnTerminateEvent;

        /// <summary>
        ///     Occurs when [on unload event].
        /// </summary>
        public event EventHandler<FeatureBaseEventArgs> OnUnLoadEvent;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Base" /> is initialized.
        /// </summary>
        /// <value>
        ///     <c>true</c> if initialized; otherwise, <c>false</c>.
        /// </value>
        public bool Initialized { get; protected internal set; } = false;

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Base" /> is loaded.
        /// </summary>
        /// <value>
        ///     <c>true</c> if loaded; otherwise, <c>false</c>.
        /// </value>
        public bool Loaded { get; protected internal set; } = false;

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public abstract string Name { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Called when [on initialize event].
        /// </summary>
        public virtual void OnInitializeInvoker()
        {
            if (this.Initialized || this.Loaded)
            {
                return;
            }

            Console.WriteLine($"{this.Name} OnInitializeEvent invoked");

            this.Initialized = true;

            this.OnInitializeEvent?.Invoke(null, new FeatureBaseEventArgs(this));
        }

        /// <summary>
        ///     Called when [on load event].
        /// </summary>
        public virtual void OnLoadInvoker()
        {
            if (!this.Initialized)
            {
                throw new InvalidOperationException(
                    $"{this}, can't invoke OnLoadEvent if {this} it has not been initialized.");
            }

            if (this.Loaded)
            {
                throw new InvalidOperationException(
                    $"{this}, can't invoke OnLoadEvent if {this} it has already been loaded.");
            }

            Console.WriteLine($"{this.Name} OnLoadEvent invoked");

            this.Loaded = true;

            this.OnLoadEvent?.Invoke(null, new FeatureBaseEventArgs(this));
        }

        /// <summary>
        ///     Called when [uninitialize].
        /// </summary>
        public virtual void OnTerminate(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.OnUnLoadInvoker();

            this.Menu = null;
            this.Loaded = false;
            this.Initialized = false;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [on disable event].
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// </exception>
        internal virtual void OnDisableInvoker()
        {
            if (!this.Initialized)
            {
                throw new InvalidOperationException(
                    $"{this}, can't invoke OnDisableEvent if {this} was not initialized yet.");
            }

            if (!this.Loaded)
            {
                throw new InvalidOperationException(
                    $"{this}, can't invoke OnDisableEvent if {this} was not loaded yet.");
            }

            Console.WriteLine($"{this.Name} OnDisableEvent invoked");

            this.Switch.OnOnDisableEvent();
        }

        /// <summary>
        ///     Called when [on enable event].
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// </exception>
        internal virtual void OnEnableInvoker()
        {
            if (!this.Initialized)
            {
                throw new InvalidOperationException(
                    $"{this}, can't invoke OnEnableEvent if {this} was not initialized yet.");
            }

            if (!this.Loaded)
            {
                throw new InvalidOperationException($"{this}, can't invoke OnEnableEvent if {this} was not loaded yet.");
            }

            Console.WriteLine($"{this.Name} OnEnableEvent invoked");

            this.Switch.OnOnEnableEvent();
        }

        /// <summary>
        ///     Called when [on refresh event].
        /// </summary>
        internal virtual void OnRefreshInvoker()
        {
            if (!this.Initialized)
            {
                throw new InvalidOperationException(
                    $"{this}, can't invoke OnRefreshEvent if {this} it has not been initialized.");
            }

            Console.WriteLine($"{this.Name} OnRefreshEvent invoked");
            this.OnRefreshEvent?.Invoke(null, new FeatureBaseEventArgs(this));
        }

        /// <summary>
        ///     Called when [terminate event].
        /// </summary>
        /// <exception cref="System.InvalidOperationException"></exception>
        internal virtual void OnTerminateInvoker()
        {
            if (!this.Initialized)
            {
                throw new InvalidOperationException(
                    $"{this}, can't invoke OnTerminateEvent if {this} it has not been initialized.");
            }

            this.OnTerminateEvent?.Invoke(null, new FeatureBaseEventArgs(this));
        }

        /// <summary>
        ///     Called when [on unload event].
        /// </summary>
        internal virtual void OnUnLoadInvoker()
        {
            Console.WriteLine($"{this.Name} OnUnloadEvent invoked");
            this.OnUnLoadEvent?.Invoke(null, new FeatureBaseEventArgs(this));
        }

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected virtual void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Console.WriteLine($"{this} OnDisable triggered");
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        protected virtual void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Console.WriteLine($"{this} OnEnable triggered");
        }

        /// <summary>
        ///     Called when [initialize].
        /// </summary>
        protected virtual void OnInitialize(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Console.WriteLine($"{this.Name} OnInitialize triggered");
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected virtual void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Console.WriteLine($"{this.Name} OnLoad triggered");
        }

        /// <summary>
        ///     Called when [refresh].
        /// </summary>
        protected virtual void OnRefresh(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.OnTerminateInvoker();
            this.OnInitializeInvoker();

            Console.WriteLine($"{this.Name} OnRefresh triggered");
        }

        /// <summary>
        ///     Called when [unload].
        /// </summary>
        protected virtual void OnUnload(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.OnDisableInvoker();
            Console.WriteLine($"{this.Name} OnUnload triggered");
        }

        /// <summary>
        ///     Initializes the menu, overwrite this method to change the menu.
        /// </summary>
        protected virtual void SetMenu()
        {
            this.Menu = new Menu(this.Name, this.Name);
        }

        /// <summary>
        ///     Called when [core initialize].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FeatureBaseEventArgs" /> instance containing the event data.</param>
        private void OnCoreInitialize(object sender, FeatureBaseEventArgs e)
        {
            this.SetMenu();

            if (this.Switch == null)
            {
                this.Switch = new BoolSwitch(this.Menu, "Enabled", true);
            }

            this.Switch.Setup();

            this.Switch.OnDisableEvent += this.OnDisable;
            this.Switch.OnEnableEvent += this.OnEnable;
            this.OnLoadEvent += this.OnLoad;
            this.OnUnLoadEvent += this.OnUnload;
            this.OnRefreshEvent += this.OnRefresh;
            this.OnTerminateEvent += this.OnTerminate;
        }

        #endregion

        public class FeatureBaseEventArgs : EventArgs
        {
            #region Constructors and Destructors

            public FeatureBaseEventArgs(Base sender)
            {
                this.Sender = sender;
            }

            #endregion

            #region Public Properties

            public Base Sender { get; set; }

            #endregion
        }
    }
}