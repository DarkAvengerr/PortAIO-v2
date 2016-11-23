//     Copyright (C) 2016 Rethought
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
//     Created: 04.10.2016 1:05 PM
//     Last Edited: 04.10.2016 1:44 PM

using EloBuddy; 
using LeagueSharp.Common; 
 namespace RethoughtLib.VersionChecker.Implementations
{
    #region Using Directives

    using System;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    #endregion

    public class VersionChecker : IVersionChecker
    {
        #region Fields

        /// <summary>
        ///     The local version
        /// </summary>
        public Version LocalVersion = Assembly.GetCallingAssembly().GetName().Version;

        #endregion

        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="VersionChecker" /> class.
        /// </summary>
        /// <param name="githubPath">The github path.</param>
        public VersionChecker(string githubPath)
        {
            this.GitHubPath = githubPath;
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name of the assembly.
        /// </summary>
        /// <value>
        ///     The name of the assembly.
        /// </value>
        public string AssemblyName { get; set; }

        /// <summary>
        ///     The git hub path
        /// </summary>
        public string GitHubPath { get; set; }

        /// <summary>
        ///     Gets the relative result of comparison.
        /// </summary>
        /// <value>
        ///     The relative result.
        /// </value>
        public int RelativeResult { get; private set; }

        /// <summary>
        ///     update available
        /// </summary>
        public bool UpdateAvailable { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Checks this instance for an update.
        /// </summary>
        public void Check()
        {
            var gitHubVersion = this.GetGitHubVersion().Result;

            this.Compare(this.LocalVersion, gitHubVersion);
        }

        /// <summary>
        ///     Compares the specified versions.
        /// </summary>
        /// <param name="version1">The first version.</param>
        /// <param name="version2">The second version.</param>
        /// <value>
        ///     0 if equal, 1 if first version is higher, -1 if second version is higher
        /// </value>
        public void Compare(Version version1, Version version2)
        {
            this.RelativeResult = version1.CompareTo(version2);

            if (this.RelativeResult < 0) this.UpdateAvailable = true;
        }

        /// <summary>
        ///     Gets the GitHub version.
        /// </summary>
        /// <returns></returns>
        public async Task<Version> GetGitHubVersion()
        {
            await Task.Run(
                () =>
                    {
                        using (var client = new WebClient())
                        {
                            var major = 0;
                            var minor = 0;
                            var build = 0;
                            var revision = 0;

                            var data =
                                client.DownloadString(
                                    $@"https://raw.githubusercontent.com/{this.GitHubPath}/Properties/AssemblyInfo.cs");

                            var match =
                                new Regex(
                                        @"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]")
                                    .Match(data);

                            if (!match.Success) return new System.Version(major, minor, build, revision);

                            int.TryParse(match.Groups[1].Value, out major);
                            int.TryParse(match.Groups[2].Value, out minor);
                            int.TryParse(match.Groups[3].Value, out build);
                            int.TryParse(match.Groups[4].Value, out revision);

                            return new System.Version(major, minor, build, revision);
                        }
                    });

            return null;
        }

        #endregion
    }
}