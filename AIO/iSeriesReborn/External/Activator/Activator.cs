using System;
using System.Collections.Generic;
using System.Linq;
using DZLib.Logging;
using iSeriesReborn.External.Activator.ActivatorSpells;
using iSeriesReborn.External.Activator.Items;
using iSeriesReborn.Utility;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.External.Activator
{
    class Activator
    {
        public static List<ISRItem> ActivatorItems = new List<ISRItem> 
        {   
            new _BOTRK(), 
            new _Cutlass(), 
            new _Youmuu() 
        };

        public static List<ISRSpell> ActivatorSpells = new List<ISRSpell> 
        {   
            new Heal(),
            new Barrier(),
            new Ignite()
        };

        private static float _lastCycle;

        public static void OnLoad()
        {
            foreach (var item in ActivatorItems)
            {
                item.OnLoad();
            }

            foreach (var spell in ActivatorSpells)
            {
                spell.OnLoad();
            }
        }

        public static void LoadMenu()
        {
            var RootMenu = Variables.Menu;
            var ActivatorMenu = new Menu("[iSR] Activator","iseriesr.activator");
            {
                var OffensiveMenu = new Menu("Offensive","iseriesr.activator.offensive");
                {
                    foreach (var item in ActivatorItems.Where(h => h.GetItemType() == ISRItemType.Offensive))
                    {
                        item.BuildMenu(OffensiveMenu);
                    }

                    ActivatorMenu.AddSubMenu(OffensiveMenu);
                }

                var DefensiveMenu = new Menu("Defensive", "iseriesr.activator.defensive");
                {
                    foreach (var item in ActivatorItems.Where(h => h.GetItemType() == ISRItemType.Defensive))
                    {
                        item.BuildMenu(DefensiveMenu);
                    }

                    ActivatorMenu.AddSubMenu(DefensiveMenu);
                }

                var SpellsMenu = new Menu("Spells", "iseriesr.activator.spells");
                {
                    foreach (var spell in ActivatorSpells)
                    {
                        spell.BuildMenu(SpellsMenu);
                    }

                    ActivatorMenu.AddSubMenu(SpellsMenu);
                }

                ActivatorMenu.AddKeybind("iseriesr.activator.onkey","Activator Key", new Tuple<uint, KeyBindType>(32, KeyBindType.Press));
                ActivatorMenu.AddBool("iseriesr.activator.always", "Always Enabled", true);

                RootMenu.AddSubMenu(ActivatorMenu);
            }
        }

        public static void OnUpdate()
        {

            if (!MenuExtensions.GetItemValue<KeyBind>("iseriesr.activator.onkey").Active &&
                !MenuExtensions.GetItemValue<bool>("iseriesr.activator.always"))
            {
                return;
            }

            foreach (var item in ActivatorItems.Where(item => item.ShouldRun()))
            {
                item.Run();
            }

            foreach (var spell in ActivatorSpells.Where(item => item.ShouldRun()))
            {
                spell.Run();
            }
        }
    }
}
