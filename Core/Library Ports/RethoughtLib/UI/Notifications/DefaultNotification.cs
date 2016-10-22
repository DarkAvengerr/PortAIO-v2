using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.UI.Notifications
{
    #region Using Directives

    using global::RethoughtLib.UI.Notifications.Designs;

    #endregion

    public class DefaultNotification : Notification
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultNotification"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="body">The body.</param>
        /// <param name="notificationDesign"></param>
        public DefaultNotification(string header, string body, NotificationDesign notificationDesign = null)
        {
            if (notificationDesign == null)
            {
                this.Design = new DefaultNotificationDesign();
            }

            this.Header = header;
            this.Body = body;
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string Body { get; set; }

        #endregion
    }
}