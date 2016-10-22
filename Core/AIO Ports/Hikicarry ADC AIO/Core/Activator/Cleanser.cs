using System;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry.Core.Activator
{
    internal class Cleanser
    {
        internal static ItemData.Item QuickSilverSash => ItemData.Quicksilver_Sash;
        internal static ItemData.Item Mercurial => ItemData.Mercurial_Scimitar;
        internal static Random Random;

        public Cleanser()
        {
            Console.WriteLine("HikiCarry: Cleanser Initalized");
            Game.OnUpdate += CleanseOnUpdate;
        }

        private void CleanseOnUpdate(EventArgs args)
        {
            if (Initializer.Activator.Item("use.cleanse").GetValue<bool>())
            {
                Cleanse();
            }
        }

        public static int OwnedItemCheck()
        {
            if (QuickSilverSash.GetItem().IsOwned() && QuickSilverSash.GetItem().IsReady())
            {
                return 1;
            }

            if (Mercurial.GetItem().IsOwned() && Mercurial.GetItem().IsReady())
            {
                return 2;
            }

            return 0;
        }

        public static void CleanseMe()
        {
            switch (OwnedItemCheck())
            {
                case 1:
                    LeagueSharp.Common.Utility.DelayAction.Add(Random.Next(1, Initializer.Activator.Item("cleanse.delay").GetValue<Slider>().Value), 
                        () => QuickSilverSash.GetItem().Cast());

                    break;
                case 2:
                    LeagueSharp.Common.Utility.DelayAction.Add(Random.Next(1, Initializer.Activator.Item("cleanse.delay").GetValue<Slider>().Value),
                       () => Mercurial.GetItem().Cast());
                    break;
                case 0:
                    return;
            }
        }

        public static void Cleanse()
        {
            if (OwnedItemCheck() != 0)
            {
                if ((ObjectManager.Player.HasBuffOfType(BuffType.Charm) &&
                     Initializer.Activator.Item("qss.charm").GetValue<bool>())
                    ||
                    (ObjectManager.Player.HasBuffOfType(BuffType.Flee) &&
                     Initializer.Activator.Item("qss.flee").GetValue<bool>())
                    ||
                    (ObjectManager.Player.HasBuffOfType(BuffType.Snare) &&
                     Initializer.Activator.Item("qss.snare").GetValue<bool>())
                    ||
                    (ObjectManager.Player.HasBuffOfType(BuffType.Polymorph) &&
                     Initializer.Activator.Item("qss.polymorph").GetValue<bool>())
                    ||
                    (ObjectManager.Player.HasBuffOfType(BuffType.Stun) &&
                     Initializer.Activator.Item("qss.stun").GetValue<bool>())
                    ||
                    (ObjectManager.Player.HasBuffOfType(BuffType.Suppression) &&
                     Initializer.Activator.Item("qss.suppression").GetValue<bool>())
                    ||
                    (ObjectManager.Player.HasBuffOfType(BuffType.Taunt) &&
                     Initializer.Activator.Item("qss.taunt").GetValue<bool>())
                    )
                {
                    CleanseMe();
                }

            }
        }
    }
}
