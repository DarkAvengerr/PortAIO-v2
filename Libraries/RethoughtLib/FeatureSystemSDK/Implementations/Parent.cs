using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystemSDK.Implementations
{
    #region Using Directives

    using global::RethoughtLib.FeatureSystemSDK.Abstract_Classes;

    #endregion

    #region Using Directives

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

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; }

        #endregion
    }
}