using EloBuddy;
using LeagueSharp.Common;
namespace HandicapEzreal
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using HandicapEzreal.Components;
    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The program.
    /// </summary>
    public static class Program
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the orbwalker.
        /// </summary>
        internal static Orbwalking.Orbwalker Orbwalker { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The EntryPoint of the solution.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        public static void Main()
        {
            if (ObjectManager.Player.ChampionName.Equals("Ezreal"))
            {
                Bootstrap();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The bootstrapping method for the components.
        /// </summary>
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "They would not be used.")]
        private static void Bootstrap()
        {
            try
            {
                new MyMenu();
                new SpellManager();
            }
            catch (Exception e)
            {
                Console.WriteLine("@Program.cs: Can not bootstrap components - {0}", e);
                throw;
            }
        }

        #endregion
    }
}