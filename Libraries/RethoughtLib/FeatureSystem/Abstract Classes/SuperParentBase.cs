using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Abstract_Classes
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
            base.OnLoad(sender, featureBaseEventArgs);

            this.Menu.AddToMainMenu();
        }

        /// <summary>
        ///     Initializes the menu, overwrite this method to change the menu type. Do not overwrite if you only want to change
        ///     the menu content.
        /// </summary>
        protected override void SetMenu()
        {
            this.Menu = new Menu(this.Path, this.Name, true);
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return "SuperParent " + this.Name;
        }

        #endregion
    }
}