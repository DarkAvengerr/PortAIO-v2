using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Akali.Utility
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using System;
    using System.Linq;

    internal class Potions
    {
        private static AIHeroClient Me => Program.Me;
        private static Menu Menu => Tools.Menu;

        internal static void Inject()
        {
            var PotionsMenu = Menu.Add(new Menu("Potions", "Potions"));
            {
                PotionsMenu.Add(new MenuBool("Enable", "Enabled", false));
                PotionsMenu.Add(new MenuSlider("Hp", "When Player HealthPercent <= %", 50));
            }

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead || Me.InFountain() || Me.Buffs.Any(x => x.Name.ToLower().Contains("recall") || x.Name.ToLower().Contains("teleport")))
            {
                return;
            }

            if (Menu["Potions"]["Enable"] && Menu["Potions"]["Hp"].GetValue<MenuSlider>().Value >= Me.HealthPercent)
            {
                if (Me.Buffs.Any(x => x.Name.Equals("ItemCrystalFlask", StringComparison.OrdinalIgnoreCase) ||
                    x.Name.Equals("ItemCrystalFlaskJungle", StringComparison.OrdinalIgnoreCase) ||
                    x.Name.Equals("ItemDarkCrystalFlask", StringComparison.OrdinalIgnoreCase) ||
                    x.Name.Equals("RegenerationPotion", StringComparison.OrdinalIgnoreCase) ||
                    x.Name.Equals("ItemMiniRegenPotion", StringComparison.OrdinalIgnoreCase) ||
                    x.Name.Equals("ItemMiniRegenPotion", StringComparison.OrdinalIgnoreCase)))
                {
                    return;
                }

                if (Items.HasItem(2003) && Items.UseItem(2003)) //Health_Potion 
                {
                    return;
                }

                if (Items.HasItem(2009) && Items.UseItem(2009)) //Total_Biscuit_of_Rejuvenation 
                {
                    return;
                }

                if (Items.HasItem(2010) && Items.UseItem(2010)) //Total_Biscuit_of_Rejuvenation2 
                {
                    return;
                }

                if (Items.HasItem(2031) && Items.UseItem(2031)) //Refillable_Potion 
                {
                    return;
                }

                if (Items.HasItem(2032) && Items.UseItem(2032)) //Hunters_Potion 
                {
                    return;
                }

                if (Items.HasItem(2033) && Items.UseItem(2033)) //Corrupting_Potion 
                {
                    return;
                }
            }
        }
    }
}