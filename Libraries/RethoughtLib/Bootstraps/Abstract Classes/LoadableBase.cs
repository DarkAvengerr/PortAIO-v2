using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Bootstraps.Abstract_Classes
{
    #region Using Directives

    using System.Collections.Generic;

    using LeagueSharp.Common;

    using RethoughtLib.Classes.General_Intefaces;

    #endregion

    /// <summary>
    ///     Class that represents something loadable
    /// </summary>
    /// <seealso cref="RethoughtLib.Classes.General_Intefaces.ILoadable" />
    /// <seealso cref="RethoughtLib.Classes.General_Intefaces.ITagable" />
    public abstract class LoadableBase : ILoadable, ITagable
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the name that will get displayed.
        /// </summary>
        /// <value>
        ///     The name.
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
        /// Gets or sets the root menu.
        /// </summary>
        /// <value>
        /// The root menu.
        /// </value>
        public Menu RootMenu { get; set; }

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