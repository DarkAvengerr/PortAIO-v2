using EloBuddy; namespace RethoughtLib.Classes.General_Intefaces
{
    #region Using Directives

    using System.Collections.Generic;

    #endregion

    public interface ITagable
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the tags.
        /// </summary>
        /// <value>
        ///     The tags.
        /// </value>
        IEnumerable<string> Tags { get; set; }

        #endregion
    }
}