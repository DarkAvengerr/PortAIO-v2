using System;
using System.Collections.Generic;
using DZAIO_Reborn.Plugins.Champions.Ahri;
using DZAIO_Reborn.Plugins.Champions.Bard;
using DZAIO_Reborn.Plugins.Champions.Ezreal;
using DZAIO_Reborn.Plugins.Champions.Kalista;
using DZAIO_Reborn.Plugins.Champions.Orianna;
using DZAIO_Reborn.Plugins.Champions.Sivir;
using DZAIO_Reborn.Plugins.Champions.Trundle;
using DZAIO_Reborn.Plugins.Champions.Veigar;
using DZAIO_Reborn.Plugins.Champions.Vladimir;
using DZAIO_Reborn.Plugins.Interface;
using LeagueSharp;
using LeagueSharp.Common;
using Menu = LeagueSharp.Common.Menu;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Core
{
    class Variables
    {
        public static Bootstrap BootstrapInstance;

        public static Menu AssemblyMenu = new Menu("DZAio: Reborn","dzaio", true);

        public static Orbwalking.Orbwalker Orbwalker = new Orbwalking.Orbwalker(AssemblyMenu);

        public static Dictionary<String, Func<IChampion>> ChampList = new Dictionary<string, Func<IChampion>>
        {
            {"Trundle", () => new  Trundle()},
            {"Veigar", () => new Veigar()},
            {"Ahri", ()=> new Ahri()},
            {"Bard", ()=> new Bard()},
            {"Ezreal", () => new Ezreal()},
            {"Kalista", () => new Kalista()},
            {"Orianna", () => new DZAIO_Reborn.Plugins.Champions.Orianna.Orianna()},
            {"Sivir", () => new Sivir()},
            {"Vladimir", () => new Vladimir()},
        };

        public static IChampion CurrentChampion { get; set; }

        public static Dictionary<SpellSlot, Spell> Spells
            => CurrentChampion?.GetSpells();
    }
}
