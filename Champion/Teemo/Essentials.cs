using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

namespace PandaTeemo
{
    internal class Essentials
    {
        /// <summary>
        /// Spell Q
        /// </summary>
        public static Spell Q;

        /// <summary>
        /// Spell W
        /// </summary>
        public static Spell W;

        /// <summary>
        /// Spell E
        /// </summary>
        public static Spell E;

        /// <summary>
        /// Spell R
        /// </summary>
        public static Spell R;

        /// <summary>
        /// Initializes Shroom Positions
        /// </summary>
        public static ShroomTables ShroomPositions;

        /// <summary>
        /// Initializes FileHandler
        /// </summary>
        public static FileHandler FileHandler;

        /// <summary>
        /// Initializes Orbwalker
        /// </summary>
        public static Orbwalking.Orbwalker Orbwalker;

        /// <summary>
        /// Initializes the Menu
        /// </summary>
        public static Menu Config;

        /// <summary>
        /// Gets the player.
        /// </summary>
        public static AIHeroClient Player => ObjectManager.Player;

        /// <summary>
        /// Last time R was Used.
        /// </summary>
        public static int LastR;

        /// <summary>
        /// Checks if there is shroom in location
        /// </summary>
        /// <param name="position">The location of check</param>
        /// <returns>If that location has a shroom.</returns>
        public static bool IsShroomed(Vector3 position)
        {
            return
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(obj => obj.Name == "Noxious Trap")
                    .Any(obj => position.LSDistance(obj.Position) <= 250);
        }

        /// <summary>
        /// Gets the R Range.
        /// </summary>
        public static float RRange => 300*R.Level;

        /// <summary>
        /// Array of ADC Names
        /// </summary>
        public static readonly string[] Marksman =
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Jinx", "Kalista",
            "KogMaw", "Lucian", "MissFortune", "Quinn", "Sivir", "Teemo", "Tristana", "Twitch", "Urgot", "Varus",
            "Vayne"
        };
    }
}