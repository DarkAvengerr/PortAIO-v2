using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using Proseries.Utils;
using ProSeries.Utils;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ProSeries
{
    internal static class ProSeries
    {
        internal static Menu Config;
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static AIHeroClient Player = ObjectManager.Player;

        internal static void Load()
        {
            try
            {
                // Check if the champion is supported
                var type = Type.GetType("ProSeries.Champions." + Player.ChampionName);
                if (type != null)
                {
                    // Load the crosshair
                    // Crosshair.Load();

                    // Load the menu.
                    Config = new Menu("ProSeries", "ProSeries", true);

                    // Add the orbwalking.
                    Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));

                    // Create instance of the new type
                    DynamicInitializer.NewInstance(type);

                    // Load whitelist harass menu
                    var wList = new Menu("Harass Whitelist", "hwl");
                    foreach (var enemy in HeroManager.Enemies)
                    {
                        wList.AddItem(new MenuItem("hwl" + enemy.ChampionName, enemy.ChampionName))
                            .SetValue(TargetSelector.GetPriority(enemy) >= 2f);
                    }

                    Config.SubMenu("harass").AddSubMenu(wList);

                    // Add ADC items usage.
                    ItemManager.Load();

                    // Add the menu as main menu.
                    Config.AddToMainMenu();

                    // Print the welcome message
                    Chat.Print("<b>Pro Series</b>: " + Player.ChampionName + " Loaded!");
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("ProSeries: " + Player.ChampionName + " - not Supported!");
            }
        }

        internal static bool CanCombo()
        {
            // "usecombo" keybind required
            // "combomana" slider required
            return Config.Item("usecombo").GetValue<KeyBind>().Active &&
                   Player.Mana / Player.MaxMana * 100 > Config.Item("combomana").GetValue<Slider>().Value;
        }

        internal static bool CanHarass()

        {   // "harasscombo" keybind required
            // "harassmana" slider required
            return Config.Item("useharass").GetValue<KeyBind>().Active &&
                    Player.Mana/Player.MaxMana*100 > Config.Item("harassmana").GetValue<Slider>().Value;
           
        }

        internal static bool CanClear()
        {            
            // "clearcombo" keybind required
            // "clearmana" slider required
            return Config.Item("useclear").GetValue<KeyBind>().Active &&
                  Player.Mana / Player.MaxMana * 100 > Config.Item("clearmana").GetValue<Slider>().Value;               
        }

        internal static bool IsWhiteListed(AIHeroClient unit)
        {
            // "harass" submenu required
            return Config.SubMenu("harass").Item("hwl" + unit.ChampionName).GetValue<bool>();
        }

        internal static IEnumerable<Obj_AI_Minion> JungleMobsInRange(float range)
        {
            var names = new[]
            {
                // summoners rift
                "SRU_Razorbeak", "SRU_Krug", "Sru_Crab",
                "SRU_Baron", "SRU_Dragon", "SRU_Blue", "SRU_Red", "SRU_Murkwolf", "SRU_Gromp",

                // twisted treeline
                "TT_NGolem5", "TT_NGolem2", "TT_NWolf6", "TT_NWolf3",
                "TT_NWraith1", "TT_Spider"
            };

            var minions = from minion in ObjectManager.Get<Obj_AI_Minion>()
                where minion.LSIsValidTarget(range) && !minion.Name.Contains("Mini")
                where names.Any(name => minion.Name.StartsWith(name))
                select minion;

            return minions;
        }

        // Counts the number of units in path from the start position to the end position
        internal static int CountInPath(
            Vector3 startpos,  Vector3 endpos, 
            float width, float range, out List<Obj_AI_Base> units,  bool minion = false)
        {
            var end = endpos.LSTo2D();
            var start = startpos.LSTo2D();
            var direction = (end - start).LSNormalized();
            var endposition = start + direction * start.LSDistance(endpos);

            var objinpath = from unit in ObjectManager.Get<Obj_AI_Base>().Where(b => b.Team != Player.Team)
                    where Player.ServerPosition.LSDistance(unit.ServerPosition) <= range
                    where unit is AIHeroClient || unit is Obj_AI_Minion && minion
                    let proj = unit.ServerPosition.LSTo2D().LSProjectOn(start, endposition)
                    let projdist = unit.LSDistance(proj.SegmentPoint)
                    where unit.BoundingRadius + width > projdist
                    select unit;

            units = objinpath.ToList();
            return units.Count();
        }
    }
}