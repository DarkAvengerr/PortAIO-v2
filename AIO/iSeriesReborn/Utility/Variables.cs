using System;
using System.Collections.Generic;
using iSeriesReborn.Champions;
using iSeriesReborn.Champions.Jinx;
using iSeriesReborn.Champions.Kalista;
using iSeriesReborn.Champions.Tristana;
using iSeriesReborn.Champions.Vayne;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Utility
{
    /// <summary>
    /// The Variables class.
    /// </summary>
    class Variables
    {
        /// <summary>
        /// Gets or sets the assembly menu.
        /// </summary>
        /// <value>
        /// The menu.
        /// </value>
        public static Menu Menu { get; set; } = new Menu($"iSeries: Reborn - {ObjectManager.Player.ChampionName}", "iseriesr", true);

        /// <summary>
        /// Gets or sets the orbwalker.
        /// </summary>
        /// <value>
        /// The orbwalker.
        /// </value>
        public static Orbwalking.Orbwalker Orbwalker { get; set; }

        /// <summary>
        /// The Dictionary containing the champions.
        /// </summary>
        public static readonly Dictionary<string, Action> ChampList = new Dictionary<string, Action>()
        {
            { "Kalista", () => { CurrentChampion = new Kalista(); CurrentChampion.OnLoad(); } },
            { "Jinx", () => { CurrentChampion = new Jinx(); CurrentChampion.OnLoad(); } },
            { "Vayne", () => { CurrentChampion = new Vayne(); CurrentChampion.OnLoad(); } },
            { "Tristana", () => { CurrentChampion = new Tristana(); CurrentChampion.OnLoad(); } },
            { "Twitch", () => { CurrentChampion = new Twitch(); CurrentChampion.OnLoad(); } },
            { "Ezreal", () => { CurrentChampion = new Ezreal(); CurrentChampion.OnLoad(); } },
            { "Lucian", () => { CurrentChampion = new Lucian(); CurrentChampion.OnLoad(); } },
        };

        /// <summary>
        /// Gets or sets the current champion.
        /// </summary>
        /// <value>
        /// The current champion.
        /// </value>
        public static ChampionBase CurrentChampion { get; set; }

        /// <summary>
        /// Gets whether a champion assembly is loaded or not.
        /// </summary>
        public static bool IsLoaded => CurrentChampion != null;

        public static Dictionary<SpellSlot, Spell> spells
            => IsLoaded ? CurrentChampion.GetSpells() : new Dictionary<SpellSlot, Spell>();

        public static Spell qExtended
        {
            get
            {
                var champion = CurrentChampion as Lucian;
                if (champion != null)
                {
                    return champion.qExtended;
                }
                return new Spell(SpellSlot.Q, -1f);
            }
        } 
    }
}
