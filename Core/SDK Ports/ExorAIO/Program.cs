
#pragma warning disable 1587

using EloBuddy;
using LeagueSharp.SDK;
namespace ExorAIO
{
    using LeagueSharp.SDK;

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
            ///     Loads the AIO.
            /// </summary>
            Aio.OnLoad();
        }

        #endregion
    }
}