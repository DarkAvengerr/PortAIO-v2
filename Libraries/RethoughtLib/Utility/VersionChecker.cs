using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Utility
{
    #region Using Directives

    using System;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    #endregion

    public class VersionChecker
    {
        #region Fields

        /// <summary>
        ///     force update
        /// </summary>
        public bool ForceUpdate;

        /// <summary>
        ///     The local version
        /// </summary>
        public Version LocalVersion = Assembly.GetCallingAssembly().GetName().Version;

        /// <summary>
        ///     update available
        /// </summary>
        public bool UpdateAvailable;

        #endregion

        #region Constructors and Destructors

        public VersionChecker(string githubPath)
        {
            this.GitHubPath = githubPath;
        }

        #endregion

        #region Public Properties

        public string AssemblyName { get; set; }

        /// <summary>
        ///     The git hub path
        /// </summary>
        public string GitHubPath { get; set; }

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
        ///     Gets the git hub version.
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

                            var version = new System.Version(major, minor, build, revision);

                            return version;
                        }
                    });

            return null;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Compares the specified versions.
        /// </summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        private void Compare(Version version1, Version version2)
        {
            var result = version1.CompareTo(version2);

#if DEBUG
            Console.WriteLine($"[{this}] Result of comparing: {result}");
#endif

            if (result < 0)
            {
                this.UpdateAvailable = true;
            }
        }

        #endregion
    }
}