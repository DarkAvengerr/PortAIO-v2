using EloBuddy;
using LeagueSharp.Common;
namespace AlqoholicKarthus
{
    #region Using Directives

    using System;
    using System.Diagnostics.CodeAnalysis;

    using AlqoholicKarthus.Menu;
    using AlqoholicKarthus.Spells;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class Program
    {
        #region Properties

        internal static Orbwalking.Orbwalker Orbwalker { get; set; }

        #endregion

        #region Methods

        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "They would not be used.")]
        private static void Bootstrap()
        {
            try
            {
                new AlqoholicMenu();
                new SpellManager();
                new DrawingManager();
            }
            catch (Exception e)
            {
                Console.WriteLine("@Program.cs: Cannot bootstrap components - {0}", e);
            }
        }

        public static void Main()
        {
            if (ObjectManager.Player.ChampionName.Equals("Karthus"))
            {
                Bootstrap();
            }
        }

        #endregion
    }
}