using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Switches
{
    #region Using Directives

    using System;

    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    /// <summary>
    ///     Switch that displays a bool that can get named
    /// </summary>
    /// <seealso cref="RethoughtLib.FeatureSystem.Switches.SwitchBase" />
    public class KeybindSwitch : SwitchBase
    {
        #region Fields

        /// <summary>
        ///     The owner
        /// </summary>
        private readonly Base owner;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoolSwitch" /> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="boolName">Name of the bool.</param>
        /// <param name="owner">The owner.</param>
        public KeybindSwitch(Menu menu, string boolName, char key, Base owner)
            : base(menu)
        {
            this.BoolName = boolName;
            this.Key = key;
            this.owner = owner;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name of the bool.
        /// </summary>
        /// <value>
        ///     The name of the bool.
        /// </value>
        public string BoolName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [bool value].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [bool value]; otherwise, <c>false</c>.
        /// </value>
        public char Key { get; set; }

        private Base.FeatureBaseEventArgs Cache;

        /// <summary>
        ///     Raises the <see cref="E:OnDisableEvent" /> event.
        /// </summary>
        /// <param name="e">The <see cref="Base.FeatureBaseEventArgs" /> instance containing the event data.</param>
        public override void Disable(Base.FeatureBaseEventArgs e)
        {
            if (e.Sender == this.owner)
            {
                if (this.Cache != null)
                {
                    e = this.Cache;
                    this.Cache = null;
                }

                base.Disable(e);
                return;
            }

            this.Cache = e;

            var keybind = new KeyBind(this.Key, KeyBindType.Toggle) { Active = false };

            this.Menu.Item(this.owner.Path + "." + this.BoolName).SetValue(keybind);
        }

        /// <summary>
        ///     Raises the <see cref="E:OnEnableEvent" /> event.
        /// </summary>
        /// <param name="e">The <see cref="Base.FeatureBaseEventArgs" /> instance containing the event data.</param>
        public override void Enable(Base.FeatureBaseEventArgs e)
        {
            if (e.Sender == this.owner)
            {
                if (this.Cache != null)
                {
                    e = this.Cache;
                    this.Cache = null;
                }

                base.Enable(e);
                return;
            }

            this.Cache = e;

            var keybind = new KeyBind(this.Key, KeyBindType.Toggle) { Active = true };

            this.Menu.Item(this.owner.Path + "." + this.BoolName).SetValue(keybind);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Setups this instance.
        /// </summary>
        public override void Setup()
        {
            this.Menu.AddItem(new MenuItem(this.owner.Path + "." + this.BoolName, this.BoolName).SetValue(new KeyBind(this.Key, KeyBindType.Toggle)))
                .ValueChanged += delegate(object sender, OnValueChangeEventArgs args)
                    {
                        if (args.GetNewValue<KeyBind>().Active)
                        {
                            this.Enable(new Base.FeatureBaseEventArgs(this.owner));
                        }
                        else
                        {
                            this.Disable(new Base.FeatureBaseEventArgs(this.owner));
                        }
                    };

            if (this.Menu.Item(this.owner.Path +"."+ this.BoolName).GetValue<KeyBind>().Active)
            {
                this.Enable(new Base.FeatureBaseEventArgs(this.owner));
            }
            else
            {
                this.Disable(new Base.FeatureBaseEventArgs(this.owner));
            }
        }

        #endregion
    }
}