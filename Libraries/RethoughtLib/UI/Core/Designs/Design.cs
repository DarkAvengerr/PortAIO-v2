using EloBuddy; namespace RethoughtLib.UI.Core.Designs
{
    #region Using Directives

    using global::RethoughtLib.Transitions;
    using global::RethoughtLib.Transitions.Abstract_Base;

    #endregion

    public abstract class Design
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public abstract int Height { get; set; }

        /// <summary>
        ///     Gets or sets the transition.
        /// </summary>
        /// <value>
        ///     The transition.
        /// </value>
        public abstract Transition Transition { get; set; }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public abstract int Width { get; set; }

        #endregion
    }
}