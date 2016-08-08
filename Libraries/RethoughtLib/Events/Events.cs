using EloBuddy; namespace RethoughtLib.Events
{
    #region Using Directives

    using System;

    using global::RethoughtLib.Classes.General_Intefaces;

    using LeagueSharp;

    #endregion

    public delegate void EventHandler(EventArgs args);

    /// <summary>
    ///     Class that offers additional events
    /// </summary>
    public class Events : ILoadable
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

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; } = "Events";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
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