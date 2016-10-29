
#pragma warning disable 1587

using EloBuddy;
using LeagueSharp.SDK;
namespace NabbActivator
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
        private static void Main()
        {
            /// <summary>
            ///     Loads the Bootstrap.
            /// </summary>
            Bootstrap.Init();

            /// <summary>
            ///     Loads the activator index.
            /// </summary>
            Index.OnLoad();

            /// <summary>
            ///     Tells the player the assembly has been loaded.
            /// </summary>
            Chat.Print(
                "[SDK]<b><font color='#FF0000'>Nabb</font></b>Activator: <font color='#FF0000'>Ultima</font> - Loaded!");
        }

        #endregion
    }
}