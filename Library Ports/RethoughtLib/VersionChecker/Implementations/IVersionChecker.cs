using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.VersionChecker.Implementations
{
    #region Using Directives

    using System;
    using System.Threading.Tasks;

    #endregion

    public interface IVersionChecker
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the name of the assembly.
        /// </summary>
        /// <value>
        ///     The name of the assembly.
        /// </value>
        string AssemblyName { get; set; }

        /// <summary>
        ///     The GitHub path
        /// </summary>
        string GitHubPath { get; set; }

        /// <summary>
        ///     Gets the relative result of comparison.
        /// </summary>
        /// <value>
        ///     The relative result.
        /// </value>
        int RelativeResult { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether an update is available.
        /// </summary>
        /// <value>
        ///     <c>true</c> if update available; otherwise, <c>false</c>.
        /// </value>
        bool UpdateAvailable { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Checks this instance for an update.
        /// </summary>
        void Check();

        /// <summary>
        ///     Compares the specified versions.
        /// </summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        void Compare(Version version1, Version version2);

        /// <summary>
        ///     Gets the GitHub version.
        /// </summary>
        /// <returns></returns>
        Task<Version> GetGitHubVersion();

        #endregion
    }
}