using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Implementations
{
    #region Using Directives

    using System;

    using global::RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    public sealed class Parent : ParentBase
    {
        #region Constructors and Destructors

        public Parent(string name)
        {
            this.Name = name;

            this.OnInitializeInvoker();
        }

        #endregion

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; }
    }
}