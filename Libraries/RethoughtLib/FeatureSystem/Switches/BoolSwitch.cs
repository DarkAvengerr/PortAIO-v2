using EloBuddy; namespace RethoughtLib.FeatureSystem.Switches
{
    #region Using Directives

    using LeagueSharp.Common;

    #endregion

    internal class BoolSwitch : SwitchBase
    {
        /// <summary>
        /// Gets or sets the name of the bool.
        /// </summary>
        /// <value>
        /// The name of the bool.
        /// </value>
        public string BoolName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [bool value].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [bool value]; otherwise, <c>false</c>.
        /// </value>
        public bool BoolValue { get; set; }

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BoolSwitch"/> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="boolName">Name of the bool.</param>
        /// <param name="boolValue">if set to <c>true</c> [bool value].</param>
        public BoolSwitch(Menu menu, string boolName, bool boolValue)
            : base(menu)
        {
            this.BoolName = boolName;
            this.BoolValue = boolValue;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Setups this instance.
        /// </summary>
        public override void Setup()
        {
            this.Menu.AddItem(new MenuItem(this.BoolName, this.BoolName).SetValue(this.BoolValue)).ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs args)
                    {
                        if (args.GetNewValue<bool>())
                        {
                            this.OnOnEnableEvent();
                        }
                        else
                        {
                            this.OnOnDisableEvent();
                        }
                    };

            this.Enabled = this.Menu.Item(this.BoolName).GetValue<bool>();
        }

        #endregion
    }
}