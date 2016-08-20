using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.UI.Notifications
{
    #region Using Directives

    using LeagueSharp.Common;

    using global::RethoughtLib.Transitions;
    using global::RethoughtLib.Transitions.Abstract_Base;
    using global::RethoughtLib.Transitions.Implementations;
    using global::RethoughtLib.UI.Core;
    using global::RethoughtLib.UI.Notifications.Designs;

    using SharpDX;

    #endregion

    /// <summary>
    ///     A Notification
    /// </summary>
    public abstract class Notification : Element
    {
        #region Fields

        /// <summary>
        ///     Whether moving or not
        /// </summary>
        internal bool Moving;

        /// <summary>
        ///     The start position
        /// </summary>
        internal Vector2 StartPosition;

        /// <summary>
        ///     The transition
        /// </summary>
        internal Transition Transition = new ExpoEaseInOut(0.5);

        /// <summary>
        ///     The design
        /// </summary>
        private NotificationDesign design;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the design.
        /// </summary>
        /// <value>
        ///     The design.
        /// </value>
        internal new virtual NotificationDesign Design
        {
            get
            {
                return this.design;
            }
            set
            {
                this.design = value;

                this.design.Update(this);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Removes the notification
        /// </summary>
        public virtual void Dispose()
        {
            if (this.Moving)
            {
                this.Design.Transition.Start(this.Position, this.Position.Extend(this.StartPosition, this.Design.Width));
            }

            this.Position = this.Design.Transition.GetPosition();
        }

        /// <summary>
        ///     Draws this instance.
        /// </summary>
        public override void Draw()
        {
            
        }

        #endregion
    }
}