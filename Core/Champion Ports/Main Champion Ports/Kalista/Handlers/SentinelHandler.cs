using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using S_Plus_Class_Kalista.Libaries;
using S_Plus_Class_Kalista.Structures;
using EloBuddy;

namespace S_Plus_Class_Kalista.Handlers
{
    class SentinelHandler : Core
    {

        public const string _MenuNameBase = ".Sentinel Menu";
        public const string _MenuItemBase = ".Sentinel.";

        private static short _selectedLocation = (short)SentinelLocationEnum.Dragon;
        
        private static float LastKeyCheckTick { get; set; } = Core.Time.TickCount;
        private static float LastSendCheckTick { get; set; } = Core.Time.TickCount;

        private enum SentinelLocationEnum
        {
            Dragon = 0,
            Baron = 1,
            OrderRed = 2,
            OrderBlue = 3,
            ChaosRed = 4,
            ChaosBlue = 5
        }

        public static void Load()
        {
            if (Game.MapId != GameMapId.SummonersRift) return;

            SMenu.AddSubMenu(_Menu());
            Game.OnUpdate += OnUpdate;
        }

        private static Menu _Menu()
        {
            var menu = new Menu(_MenuNameBase, "sentinelMenu");
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.UseSentinel", "Use Sentinal").SetValue(true));
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.UseSentinel.ModeSwap", "Swap Between Locations").SetValue(new KeyBind('T', KeyBindType.Press)));
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.UseSentinel.KeyPress", "Send Sentinel To Location").SetValue(new KeyBind('G', KeyBindType.Press)));
            //var menuSub = new Menu("Sentinel Locations", "sentinelMenu");
            //menuSub.AddItem(new MenuItem(_MenuItemBase + "Boolean.UseSentinel.OnDragon", "Use On Dragon").SetValue(true));
            //menuSub.AddItem(new MenuItem(_MenuItemBase + "Boolean.UseSentinel.OnBaron", "Use On Baron").SetValue(true));
            //menuSub.AddItem(new MenuItem(_MenuItemBase + "Boolean.UseSentinel.OnRed", "Use On Red's").SetValue(true));
            //menuSub.AddItem(new MenuItem(_MenuItemBase + "Boolean.UseSentinel.OnBlue", "Use On Blue's").SetValue(true));
            //menuSub.AddItem(new MenuItem(_MenuItemBase + "Boolean.UseSentinel.OnMid", "Use On Mid").SetValue(true));
            //menu.AddSubMenu(menuSub);
            return menu;
        }


        public static string GetSentinelSelected()
        {
            switch (_selectedLocation)
            {
                case (short)SentinelLocationEnum.Dragon:
                    return "Dragon";
                case (short)SentinelLocationEnum.Baron:
                    return "Baron";
                case (short)SentinelLocationEnum.OrderRed:
                    return "Order.Red";
                case (short)SentinelLocationEnum.OrderBlue:
                    return "Order.Blue";
                case (short)SentinelLocationEnum.ChaosRed:
                    return "Chaos.Red";
                case (short)SentinelLocationEnum.ChaosBlue:
                    return "Chaos.Blue";
            }

            return "NA";
        }

        private static void SwapLocations()
        {
            if (_selectedLocation < (short) SentinelLocationEnum.ChaosBlue)
                _selectedLocation++;
            else // it is at Chaos.blue reset it
                _selectedLocation = (short) SentinelLocationEnum.Dragon;
        }



        private static bool SentSentinel()
        {
            if (!Champion.W.IsReady()) return false;
            Vector2 location = location = Monster.MonsterLocations["Neutral.Dragon"];

            switch (_selectedLocation)
            {
                //case (short)SentinelLocationEnum.Dragon:
                //    location = Monster.MonsterLocations["Neutral.Dragon"];
                //    break;

                case (short)SentinelLocationEnum.Baron:
                    location = Monster.MonsterLocations["Neutral.Baron"];
                    break;

                case (short)SentinelLocationEnum.OrderRed:
                    location = Monster.MonsterLocations["Order.Red"];
                    break;

                case (short)SentinelLocationEnum.OrderBlue:
                    location = Monster.MonsterLocations["Order.Blue"];
                    break;

                case (short)SentinelLocationEnum.ChaosRed:
                    location = Monster.MonsterLocations["Chaos.Red"];
                    break;
                case (short)SentinelLocationEnum.ChaosBlue:
                    location = Monster.MonsterLocations["Chaos.Blue"];
                    break;

            }

            if (!(ObjectManager.Player.Distance(location) <= Champion.W.Range)) return false; // not in range

            Champion.W.Cast(location);
                    return true;
        }
        private static void OnUpdate(EventArgs args)
        {
            if (SMenu.Item(_MenuItemBase + "Boolean.UseSentinel").GetValue<bool>())
            {
                if (Core.Time.TickCount > LastKeyCheckTick + 1000) // every 1 second
                {
                    if (SMenu.Item(_MenuItemBase + "Boolean.UseSentinel.ModeSwap").GetValue<KeyBind>().Active)
                        SwapLocations();
                    LastKeyCheckTick = Time.TickCount;
                }
                if (Core.Time.TickCount > LastSendCheckTick + 3000) // every 3 second
                {
                    if (SMenu.Item(_MenuItemBase + "Boolean.UseSentinel.KeyPress").GetValue<KeyBind>().Active)
                        if(SentSentinel())
                            LastSendCheckTick = Time.TickCount;
                }
            }
        }
    }
}


