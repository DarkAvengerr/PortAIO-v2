using EloBuddy; namespace RethoughtLib.UI.Notifications.Designs
{
    using global::RethoughtLib.UI.Core.Designs;

    #region Using Directives

    

    #endregion

    /// <summary>
    ///     Base NotificationDesign
    /// </summary>
    public abstract class NotificationDesign : Design
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the header.
        /// </summary>
        /// <value>
        ///     The header.
        /// </value>
        public virtual string Header { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Updates this instance.
        /// </summary>
        public abstract void Update<T>(T notification) where T : Notification;

        #endregion
    }
}