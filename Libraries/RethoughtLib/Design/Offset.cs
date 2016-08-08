using EloBuddy; namespace RethoughtLib.Design
{
    public class Offset<T>
        where T : struct
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the bottom.
        /// </summary>
        /// <value>
        ///     The bottom.
        /// </value>
        public virtual T Bottom { get; set; } = default(T);

        /// <summary>
        ///     Gets or sets the left.
        /// </summary>
        /// <value>
        ///     The left.
        /// </value>
        public virtual T Left { get; set; } = default(T);

        /// <summary>
        ///     Gets or sets the right.
        /// </summary>
        /// <value>
        ///     The right.
        /// </value>
        public virtual T Right { get; set; } = default(T);

        /// <summary>
        ///     Gets or sets the top.
        /// </summary>
        /// <value>
        ///     The top.
        /// </value>
        public virtual T Top { get; set; } = default(T);

        #endregion
    }
}