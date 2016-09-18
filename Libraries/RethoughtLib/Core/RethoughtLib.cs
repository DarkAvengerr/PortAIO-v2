using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Core
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using global::RethoughtLib.Classes.General_Intefaces;

    using LeagueSharp.Common;

    #endregion

    // TODO REMOVE
    public class RethoughtLib : ILoadable
    {
        #region Fields

        /// <summary>
        /// The loadables
        /// </summary>
        private readonly List<ILoadable> loadables = new List<ILoadable>() { new Events.Events() };

        /// <summary>
        ///     The initialized
        /// </summary>
        private bool initialized;

        #endregion

        #region Constructors and Destructors

        static RethoughtLib()
        {
        }

        private RethoughtLib()
        {
        }

        #endregion

        #region Public Properties

        public static RethoughtLib Instance { get; } = new RethoughtLib();

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; } = "RethoughtLib";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            if (this.initialized) return;

            this.initialized = true;

            Console.WriteLine("Test");

            CustomEvents.Game.OnGameLoad += this.Game_OnGameLoad;
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        void ILoadable.Load()
        {
            this.Load();
        }

        #endregion

        #region Methods

        private void Game_OnGameLoad(EventArgs args)
        {
            foreach (var loadable in this.loadables)
            {
                loadable.Load();
            }
        }

        #endregion
    }
}