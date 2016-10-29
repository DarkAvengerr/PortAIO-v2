
#pragma warning disable 1587

using EloBuddy;
using LeagueSharp.SDK;
namespace NabbTracker
{
    using LeagueSharp;
    using LeagueSharp.SDK;

    /// <summary>
    ///     The application class.
    /// </summary>
    internal class Program
    {
        #region Methods

        /// <summary>
        ///     The entry point of the application.
        /// </summary>
        public static void Main()
        {
            /// <summary>
            ///     Loads the Bootstrap.
            /// </summary>
            Bootstrap.Init();

            /// <summary>
            ///     Loads the assembly.
            /// </summary>
            Tracker.OnLoad();

            /// <summary>
            ///     Tells the player the assembly has been loaded.
            /// </summary>
            Chat.Print(
                "[SDK]<b><font color='#228B22'>Nabb</font></b>Tracker: <font color='#228B22'>Ultima</font> - Loaded!");
        }

        #endregion
    }
}