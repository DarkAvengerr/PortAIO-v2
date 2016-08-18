using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Base
{
    #region Using Directives

    using System;

    using CommonEx.Classes;

    #endregion

    /// <summary>
    ///     Default Implementation of IChampion
    /// </summary>
    /// <seealso cref="Yasuo.CommonEx.Classes.IChampion" />
    class BaseChampion : IChampion
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; } = "base";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            Console.WriteLine(
                string.Format(
                    GlobalVariables.DisplayName
                    + " does not support this champion. No Modules, except base Modules, will be loaded."));
        }

        #endregion
    }
}