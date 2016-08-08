using EloBuddy; namespace RethoughtLib.UI.Core
{
    #region Using Directives

    using global::RethoughtLib.UI.Core.Designs;

    using SharpDX;

    #endregion

    public abstract class Element
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the design.
        /// </summary>
        /// <value>
        ///     The design.
        /// </value>
        public Design Design { get; set; }

        /// <summary>
        ///     Gets or sets the position.
        /// </summary>
        /// <value>
        ///     The position.
        /// </value>
        public Vector2 Position { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Draws this instance.
        /// </summary>
        public abstract void Draw();

        #endregion
    }
}