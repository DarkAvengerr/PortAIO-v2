using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Bootstraps.Abstract_Classes
{
    #region Using Directives

    using System.Collections.Generic;

    using global::RethoughtLib.Classes.General_Intefaces;

    #endregion

    public abstract class LoadableBase : ILoadable, ITagable
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the name that will get displayed.
        /// </summary>
        /// <value>
        ///     The name of the displaying.
        /// </value>
        public abstract string DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the internal name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public abstract string InternalName { get; set; }

        /// <summary>
        ///     Gets or sets the tags.
        /// </summary>
        /// <value>
        ///     The tags.
        /// </value>
        public abstract IEnumerable<string> Tags { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public abstract void Load();

        #endregion
    }
}