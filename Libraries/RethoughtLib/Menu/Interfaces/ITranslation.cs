using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Menu.Interfaces
{
    #region Using Directives

    using System.Collections.Generic;

    #endregion

    /// <summary>
    ///     Interface for Menu translations
    /// </summary>
    internal interface ITranslation
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        string Name { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns a dictionary containing the translations.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> Strings();

        #endregion
    }
}