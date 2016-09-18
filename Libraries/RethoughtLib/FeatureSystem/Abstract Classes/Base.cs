using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Abstract_Classes
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Switches;
    using RethoughtLib.Menu;

    #endregion

    /// <summary>
    ///     Base class of Parent-Child System
    /// </summary>
    public abstract class Base
    {
        #region Fields

        /// <summary>
        ///     The switch
        /// </summary>
        public SwitchBase Switch;

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when [on load event].
        /// </summary>
        public event EventHandler<FeatureBaseEventArgs> OnLoadEvent;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Base" /> is initialized.
        /// </summary>
        /// <value>
        ///     <c>true</c> if initialized; otherwise, <c>false</c>.
        /// </value>
        public bool Initialized { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Base" /> is loaded.
        /// </summary>
        /// <value>
        ///     <c>true</c> if loaded; otherwise, <c>false</c>.
        /// </value>
        public bool Loaded { get; private set; }

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

        public string Path { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Disables the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void Disable(Base sender = null)
        {
            if (sender == null)
            {
                sender = this;
            }

            this.Switch.Disable(new FeatureBaseEventArgs(sender));
        }

        /// <summary>
        ///     Enables the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void Enable(Base sender = null)
        {
            if (sender == null)
            {
                sender = this;
            }

            this.Switch.Enable(new FeatureBaseEventArgs(sender));
        }

        /// <summary>
        ///     "Hides" this instance
        /// </summary>
        /// <param name="hideSubMenus">if set to <c>true</c> [hide sub menus].</param>
        public void Hide(bool hideSubMenus = true)
        {
            this.Menu.DisplayName += " (Hidden)";

            var currentMenu = this.Menu;

            if (!hideSubMenus) return;

            var todo = new List<Menu>();

            var index = 0;

            do
            {
                {
                    if (!currentMenu.Children.Any()) break;

                    todo.AddRange(currentMenu.Children);

                    index++;

                    currentMenu = todo[index];
                }
            }
            while (true);

            foreach (var menu in todo)
            {
                foreach (var menuItem in menu.Items)
                {
                    menuItem.Hide();
                }
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            if (this.Initialized)
            {
                return;
            }

            this.Initialized = true;

            this.Path = this.Name;

            this.OnLoadEvent += this.OnLoad;

            this.SetMenu();
            this.SetSwitch();
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public virtual void Load()
        {
            if (this.Loaded)
            {
                return;
            }

            if (!this.Initialized)
            {
                this.Initialize();
            }

            this.Switch.OnDisableEvent += this.OnDisable;
            this.Switch.OnEnableEvent += this.OnEnable;

            this.Switch.Setup();

            this.Loaded = true;

            this.OnLoadEvent?.Invoke(null, new FeatureBaseEventArgs(this));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected virtual void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
#if DEBUG
            Console.WriteLine($"{this}: OnDisable");
#endif
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        protected virtual void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
#if DEBUG
            Console.WriteLine($"{this}: OnEnable");
#endif
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected virtual void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
#if DEBUG
            Console.WriteLine($"{this}: OnLoad");

#endif
        }

        /// <summary>
        ///     Called when [unload].
        /// </summary>
        protected virtual void OnUnload(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.Switch.Disable(featureBaseEventArgs);
        }

        /// <summary>
        ///     Sets the menu
        /// </summary>
        protected virtual void SetMenu()
        {
#if DEBUG
            Console.WriteLine($"{this}: SetMenu");

#endif
            this.Menu = new Menu(this.Path, this.Name);
        }

        /// <summary>
        ///     Sets the switch.
        /// </summary>
        protected virtual void SetSwitch()
        {
#if DEBUG
            Console.WriteLine($"{this}: SetSwitch");

#endif
            this.Switch = new BoolSwitch(this.Menu, "Enabled", true, this);
        }

        #endregion

        /// <summary>
        ///     FeatureBaseEventArgs
        /// </summary>
        /// <seealso cref="System.EventArgs" />
        public class FeatureBaseEventArgs : EventArgs
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="FeatureBaseEventArgs" /> class.
            /// </summary>
            /// <param name="sender">The sender.</param>
            public FeatureBaseEventArgs(Base sender)
            {
                this.Sender = sender;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the sender.
            /// </summary>
            /// <value>
            ///     The sender.
            /// </value>
            public Base Sender { get; set; }

            #endregion
        }
    }
}