using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Utility
{
    #region Using Directives

    using System;

    using LeagueSharp;

    #endregion

    public delegate void EventHandler(EventArgs args);

    /// <summary>
    ///     Class that offers more events through the LeagueSharp.OnUpdate Event.
    /// </summary>
    internal class Events
    {
        #region Public Events

        /// <summary>
        ///     Occurs when [on post update].
        /// </summary>
        public static event EventHandler OnPostUpdate;

        /// <summary>
        ///     Occurs when [on pre update].
        /// </summary>
        public static event EventHandler OnPreUpdate;

        /// <summary>
        ///     Occurs when [on update].
        /// </summary>
        public static event EventHandler OnUpdate;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += OnGameUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Raises the <see cref="E:GameUpdate" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnGameUpdate(EventArgs args)
        {
            try
            {
                OnPreUpdate?.Invoke(args);

                OnUpdate?.Invoke(args);

                OnPostUpdate?.Invoke(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion
    }
}