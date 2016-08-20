using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystemSDK.Abstract_Classes
{
    using System;

    using LeagueSharp.SDK.UI;

    #region Using Directives


    #endregion

    public abstract class SuperParentBase : ParentBase
    {
        #region Methods

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.Menu.Attach();

            base.OnLoad(sender, featureBaseEventArgs);
        }

        /// <summary>
        /// Initializes the menu, overwrite this method to change the menu type. Do not overwrite if you only want to change
        /// the menu content.
        /// </summary>
        protected override void CreateMenu()
        {
            this.Menu = new Menu(this.Name, this.Name, true);

            var menuBool = new MenuBool(this.Name + "Enabled", "Enabled", true);

            this.Menu.Add(menuBool);

            menuBool.ValueChanged += delegate (object sender, EventArgs args)
            {
                var value = this.Menu[this.Name + "Enabled"].GetValue<MenuBool>();

                if (value.Value)
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
            this.Menu = null;

            foreach (var child in this.Children)
            {
                child.Key.OnUnLoadInvoker();
            }
        }

        #endregion
    }
}