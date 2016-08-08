using EloBuddy; namespace RethoughtLib.FeatureSystem.Abstract_Classes
{
    #region Using Directives

    using System;

    using LeagueSharp.Common;

    #endregion

    public abstract class SuperParentBase : ParentBase
    {
        #region Methods

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.Menu.AddToMainMenu();

            base.OnLoad(sender, featureBaseEventArgs);
        }

        /// <summary>
        /// Initializes the menu, overwrite this method to change the menu type. Do not overwrite if you only want to change
        /// the menu content.
        /// </summary>
        protected override void SetMenu()
        {
            this.Menu = new Menu(this.Name, this.Name, true);

            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            this.Menu.Item(this.Name + "Enabled").ValueChanged += delegate (object sender, OnValueChangeEventArgs args)
            {
                if (args.GetNewValue<bool>())
                {
                    this.OnEnableInvoker();
                }
                else
                {
                    this.OnDisableInvoker();
                }
            };
        }

        /// <summary>
        ///     Called when [unload].
        /// </summary>
        protected override void OnUnload(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu.Remove(this.Menu);

            foreach (var child in this.Children)
            {
                child.Key.OnUnLoadInvoker();
            }
        }

        #endregion
    }
}