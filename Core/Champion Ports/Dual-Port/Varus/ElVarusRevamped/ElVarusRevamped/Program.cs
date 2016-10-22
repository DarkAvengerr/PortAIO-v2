namespace ElVarusRevamped
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using ElVarusRevamped.Components;
    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

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

        #endregion

        #region Methods

        /// <summary>
        ///     The bootstrapping method for the components.
        /// </summary>
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "They would not be used.")]
        public static void Bootstrap()
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