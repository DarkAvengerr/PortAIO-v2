using EloBuddy;
using LeagueSharp.Common;
namespace AlqoholicLissandra
{
    #region Using Directives

    using System;
    using System.Diagnostics.CodeAnalysis;

    using AlqoholicLissandra.Managers;
    using AlqoholicLissandra.Menu;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class Program
    {
        #region Properties

        internal static Orbwalking.Orbwalker Orbwalker { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Bootstrap
        /// </summary>
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "They would not be used.")]
        private static void Bootstrap()
        {
            new AlqoholicMenu();
            new SpellManager();
            new DrawingManager();
        }

        public static void Main()
        {
            if (ObjectManager.Player.ChampionName.Equals("Lissandra"))
            {
                Bootstrap();
            }
        }

        #endregion
    }
}