using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Implementations
{
    #region Using Directives

    using System;

    using global::RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    public sealed class SuperParent : SuperParentBase
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperParent"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public SuperParent(string name)
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

        /// <summary>
        ///     Raises the <see cref="E:ChildAddInvoker" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="ParentBase.ParentBaseEventArgs" /> instance containing the event data.</param>
        protected override void OnChildAddInvoker(ParentBaseEventArgs eventArgs)
        {
            if (eventArgs.Child is SuperParent)
            {
                throw new InvalidOperationException("A Super-Parent can't add other Super-Parents");
            }

            base.OnChildAddInvoker(eventArgs);
        }
    }
}