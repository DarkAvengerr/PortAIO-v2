using EloBuddy; namespace RethoughtLib.CastManager
{
    #region Using Directives

    using System;

    using global::RethoughtLib.Events;
    using global::RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class CastManagerMenu : ChildBase
    {
        #region Public Properties

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "Cast Manager";

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Events.OnPostUpdate -= OnPostUpdate;
        }

        /// <summary>
        ///     Called when [enable].
        /// </summary>
        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Events.OnPostUpdate += OnPostUpdate;
        }

        /// <summary>
        ///     Raises the <see cref="E:PostUpdate" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnPostUpdate(EventArgs args)
        {
            CastManager.Instance.Process();
        }

        #endregion
    }
}